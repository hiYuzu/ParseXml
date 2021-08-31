using Microsoft.Win32;
using System;
using System.Windows;
using ParseXml.Util;
using ParseXml.Log4Net;
using System.IO;
using ParseXml.Service;

namespace ParseXml
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string importDir = null;
        private UploadService uploadService = null;

        public MainWindow()
        {
            InitializeComponent();
            ExceptionUtil.Instance.LogEvent += new ExceptionDelegate(ShowLogMsg);
            ExceptionUtil.Instance.ExceptionEvent += new ExceptionDelegate(ShowExceptionMsg);
            InitDir();
            StartService();
        }

        /// <summary>
        /// 判断文件夹是否存在并创建
        /// </summary>
        private void InitDir()
        {
            try
            {
                if (!Directory.Exists(GlobalParam.TO_UPLOAD))
                {
                    _ = Directory.CreateDirectory(GlobalParam.TO_UPLOAD);
                }
                if (!Directory.Exists(GlobalParam.UPLOADED))
                {
                    _ = Directory.CreateDirectory(GlobalParam.UPLOADED);
                }
            }
            catch (Exception exp)
            {
                ExceptionUtil.Instance.ExceptionMethod("程序出错，请查看软件日志或联系开发人员：" + exp.Message, true);
                Application.Current.Shutdown();
            }

        }

        /// <summary>
        /// 开启后台线程，用于解析上传XML文件
        /// </summary>
        private void StartService()
        {
            if(uploadService == null)
            {
                uploadService = new UploadService();
            }
            uploadService.StartService();
        }

        private bool IsExistFile(string absFilePath)
        {
            return File.Exists(absFilePath);
        }

        /// <summary>
        /// 异常日志保存
        /// </summary>
        /// <param name="msg"></param>
        private void ShowExceptionMsg(string msg, bool isShow)
        {
            if (isShow)
            {
                MessageBox.Show(msg, "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            Log4NetUtil.Error(msg);
        }
        /// <summary>
        /// 显示日志
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="msg"></param>
        /// <param name="isShow"></param>
        private void ShowLogMsg(string msg, bool isShow)
        {
            if (isShow)
            {
                MessageBox.Show(msg, "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            Log4NetUtil.Debug(msg);
        }

        /// <summary>
        /// 上传按钮点击
        /// </summary>
        /// <param name="sender">默认参数：事件引发者</param>
        /// <param name="e">默认参数：事件</param>
        private void UploadBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog op = new OpenFileDialog
                {
                    InitialDirectory = importDir ?? Environment.CurrentDirectory,
                    RestoreDirectory = true,
                    Filter = "XML文档(*.xml)|*.xml"
                };
                if (op.ShowDialog() == true)
                {
                    importDir = op.FileName.Substring(0, op.FileName.LastIndexOf("\\"));
                    LoadFile(op.FileName);
                }
            }
            catch (Exception exp)
            {
                Log4NetUtil.Warn(exp.Message);
            }
        }

        /// <summary>
        /// 上传（移动）文件
        /// </summary>
        /// <param name="file">文件完整路径</param>
        private void LoadFile(string file)
        {
            try
            {
                string fileName = file.Substring(file.LastIndexOf("\\") + 1);
                string newFile = GlobalParam.TO_UPLOAD + fileName;
                FileUtil.MoveFile(file, newFile, false);
                ExceptionUtil.Instance.LogMethod("文件“" + fileName + "”上传成功！", true);
                StartService();
            }
            catch (Exception exp)
            {
                ExceptionUtil.Instance.ExceptionMethod("上传失败：" + exp.Message, true);
            }
        }
    }
}
