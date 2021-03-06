namespace ParseXml.Util
{

    /// <summary>
    /// 异常记录委托
    /// </summary>
    /// <param name="msg">信息</param>
    /// <param name="isShow">是否显示</param>
    public delegate void ExceptionDelegate(string msg, bool isShow);

    /// <summary>
    /// 异常通知类
    /// </summary>
    public class ExceptionUtil
    {
        public event ExceptionDelegate ExceptionEvent;
        public event ExceptionDelegate LogEvent;
        private static volatile ExceptionUtil instance = null;
        private static readonly object obj = new object();

        /// <summary>
        /// 单例
        /// </summary>
        public static ExceptionUtil Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (obj)
                    {
                        instance = new ExceptionUtil();
                    }
                }
                return instance;
            }
        }
        /// <summary>
        /// 异常信息
        /// </summary>
        /// <param name="msg">信息</param>
        /// <param name="isShow">是否显示</param>
        public void ExceptionMethod(string msg, bool isShow)
        {
            ExceptionEvent?.Invoke(msg, isShow);
        }
        /// <summary>
        /// 日志信息
        /// </summary>
        /// <param name="msg">信息</param>
        /// <param name="isShow">是否显示</param>
        public void LogMethod(string msg, bool isShow)
        {
            LogEvent?.Invoke(msg, isShow);
        }
    }
}
