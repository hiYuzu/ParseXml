using System.Collections.Generic;
using ParseXml.Log4Net;
using System.Threading;
using System;
using System.IO;
using ParseXml.Util;
using System.Xml;
using System.Diagnostics;

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
        /// 获取数据守护线程
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
        /// 操作获取数据线程
        /// </summary>
        private void DataThread()
        {
            while (isStart)
            {
                try
                {
                    ScanFiles();
                    Thread.Sleep(10000);
                }
                catch (ThreadAbortException)
                {
                    Log4NetUtil.Info("手动停止操作线程");
                    break;
                }
                catch (Exception ex)
                {
                    Log4NetUtil.Error(ex.GetType().ToString() + "：" + ex.Message);
                    Thread.Sleep(10000);
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

        private void ScanFiles()
        {
            files = Directory.EnumerateFiles(GlobalParam.TO_UPLOAD, "*.xml").GetEnumerator();
            while (files.MoveNext())
            {
                Parse(files.Current);
            }
        }


        private void Parse(string fileInfo)
        {
            XmlDocument xd = new XmlDocument();
            xd.Load(fileInfo);
            string xmlStr = xd.InnerXml;
            Log4NetUtil.Debug("开始上传：" + fileInfo);
            Upload(fileInfo.Substring(fileInfo.LastIndexOf("/") + 1), xmlStr);
        }

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
