using System.Collections.Generic;
using ParseXml.Log4Net;
using System.Threading;
using System;
using System.IO;
using ParseXml.Util;
using System.Xml;

namespace ParseXml.Service
{
    public class UploadService
    {
        //数据操作守护线程
        private Thread dataGuardThread;
        //数据操作线程
        private Thread dataThread;
        //运行状态标志
        private bool isStart = false;
        //待上传文件
        private IEnumerator<string> files;

        /// <summary>
        /// 服务入口，开启守护线程
        /// </summary>
        public void StartService()
        {
            try
            {
                isStart = true;
                //启动获取数据守护线程
                if (dataGuardThread == null || !dataGuardThread.IsAlive)
                {
                    dataGuardThread = new Thread(new ThreadStart(GuardThread))
                    {
                        Name = "DataGuardThread",
                        IsBackground = true
                    };
                    dataGuardThread.Start();
                }
            }
            catch (Exception exp)
            {
                Log4NetUtil.Error("服务启动失败，原因：" + exp.Message);
            }
        }

        /// <summary>
        /// 守护线程
        /// </summary>
        private void GuardThread()
        {
            while (isStart)
            {
                try
                {
                    //启动获取数据线程
                    if (dataThread == null || !dataThread.IsAlive)
                    {
                        dataThread = new Thread(new ThreadStart(DataThread))
                        {
                            Name = "DataThread",
                            IsBackground = true
                        };
                        dataThread.Start();
                    }
                    Thread.Sleep(1000);
                }
                catch (ThreadAbortException)
                {
                    Log4NetUtil.Info("手动停止守护线程");
                    break;
                }
                catch (Exception ex)
                {
                    Log4NetUtil.Error("守护线程运行错误，信息为：" + ex.Message);
                    Thread.Sleep(1000);
                }
            }
        }

        /// <summary>
        /// 操作线程
        /// </summary>
        private void DataThread()
        {
            while (isStart)
            {
                try
                {
                    ScanFiles();
                    Thread.Sleep(GlobalParam.INTERNAL);
                }
                catch (ThreadAbortException)
                {
                    Log4NetUtil.Info("手动停止操作线程");
                    break;
                }
                catch (Exception ex)
                {
                    Log4NetUtil.Error(ex.GetType().ToString() + "：" + ex.Message);
                    Thread.Sleep(GlobalParam.INTERNAL);
                }
            }
        }

        /// <summary>
        /// 停止服务
        /// </summary>
        private void StopService()
        {
            isStart = false;
            if (dataGuardThread != null && dataGuardThread.IsAlive)
            {
                dataGuardThread.Abort();
            }
            if (dataThread != null && dataThread.IsAlive)
            {
                dataThread.Abort();
            }
        }

        /// <summary>
        /// 扫描待解析路径，若有，则解析
        /// </summary>
        private void ScanFiles()
        {
            files = Directory.EnumerateFiles(GlobalParam.TO_UPLOAD, "*.xml").GetEnumerator();
            while (files.MoveNext())
            {
                Parse(files.Current);
            }
        }

        /// <summary>
        /// 解析XML并上传
        /// </summary>
        /// <param name="fileInfo">完整的XML文件路径</param>
        private void Parse(string fileInfo)
        {
            XmlDocument xd = new XmlDocument();
            xd.Load(fileInfo);
            string xmlStr = xd.InnerXml;
            Log4NetUtil.Debug("开始上传：" + fileInfo);
            Upload(fileInfo.Substring(fileInfo.LastIndexOf("/") + 1), xmlStr);
        }

        /// <summary>
        /// 上传XML字符串
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="xmlStr">XML内容</param>
        private void Upload(string fileName, string xmlStr)
        {
            xmlStr = xmlStr.Replace("\"", "\\\"");
            string param = "{\"xmlStr\":\"" + xmlStr + "\"}";
            string result = HttpUtil.HttpPost(GlobalParam.UPLOAD_URL, param);
            Log4NetUtil.Debug("尝试上传结果：" + result);
            if (GlobalParam.SUCCESS.Equals(result))
            {
                string oldFile = GlobalParam.TO_UPLOAD + fileName;
                string newFile = GlobalParam.UPLOADED + fileName;
                File.Move(oldFile, newFile);
            }
        }
    }
}
