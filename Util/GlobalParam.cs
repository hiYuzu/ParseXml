using System;
using System.Text;

namespace ParseXml.Util
{
    public class GlobalParam
    {
        /// <summary>
        /// 配置文件完整路径
        /// </summary>
        public static string CONFIG_PATH { get; set; }
        /// <summary>
        /// 服务端地址
        /// </summary>
        private static readonly string URL;

        /// <summary>
        /// 解析发送成功标识
        /// </summary>
        public static string SUCCESS { get { return "SUCCESS"; } }

        /// <summary>
        /// 待解析文件保存路径（本机）
        /// </summary>
        public static string TO_UPLOAD { get; private set; }

        /// <summary>
        /// 已解析文件保存路径（本机）
        /// </summary>
        public static string UPLOADED { get; private set; }

        /// <summary>
        /// 远程接口完整路径
        /// </summary>
        public static string UPLOAD_URL { get { return URL + "webservice/insertExchangeData/"; } }

        /// <summary>
        /// HTTP超时
        /// </summary>
        public static int TIME_OUT { get; private set; }

        /// <summary>
        /// 数据操作线程执行间隔
        /// </summary>
        public static int INTERNAL { get; private set; }

        static GlobalParam()
        {
            // 初始化默认值
            try
            {
                CONFIG_PATH = string.Format("{0}config\\config.ini", AppDomain.CurrentDomain.BaseDirectory);
                StringBuilder sb = new StringBuilder();
                _ = ConfigUtil.ReadConfigFile("TIMEOUT", "Timeout", "", sb, 255, CONFIG_PATH);
                TIME_OUT = Int32.Parse(sb.ToString());
                _ = sb.Clear();

                _ = ConfigUtil.ReadConfigFile("INTERNAL", "Internal", "", sb, 255, CONFIG_PATH);
                INTERNAL = Int32.Parse(sb.ToString());
                _ = sb.Clear();

                _ = ConfigUtil.ReadConfigFile("UPLOAD", "Upload", "", sb, 255, CONFIG_PATH);
                TO_UPLOAD = sb.ToString();
                _ = sb.Clear();

                _ = ConfigUtil.ReadConfigFile("UPLOADED", "Uploaded", "", sb, 255, CONFIG_PATH);
                UPLOADED = sb.ToString();
                _ = sb.Clear();

                _ = ConfigUtil.ReadConfigFile("URL", "Url", "", sb, 255, CONFIG_PATH);
                URL = sb.ToString();
                if (URL != "" && !URL.EndsWith("/"))
                {
                    URL += "/";
                }
                _ = sb.Clear();
            }
            catch (Exception exp)
            {
                Log4Net.Log4NetUtil.Error("配置文件加载失败：" + exp.Message);
                CONFIG_PATH = string.Empty;
                TO_UPLOAD = "D:/uploadFiles/upload/";
                UPLOADED = "D:/uploadFiles/uploaded/";
                URL = "http://127.0.0.1/";
                TIME_OUT = 3000;
                INTERNAL = 10000;
            }
        }
    }
}
