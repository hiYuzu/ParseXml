using System.Runtime.InteropServices;
using System.Text;

namespace ParseXml.Util
{
    public class ConfigUtil
    {
        #region 操作配置文件

        /// <summary>
        /// 写入配置文件
        /// </summary>
        /// <param name="section">section名：[NAME]</param>
        /// <param name="key">Key：section下的key值</param>
        /// <param name="val">Value：key值对应的value值</param>
        /// <param name="filePath">完整的ini文件名</param>
        /// <returns></returns>
        public static int WriteConfigFile(string section, string key, string val, string filePath)
        {
            return NativeMethods.WritePrivateProfileString(section, key, val, filePath);
        }

        /// <summary>
        /// 读取配置文件
        /// </summary>
        /// <param name="section">section名：[NAME]</param>
        /// <param name="key">Key：section下的key值</param>
        /// <param name="def">未找到上述参数时的默认值</param>
        /// <param name="retVal">接收值的缓存器</param>
        /// <param name="size">目的缓存器的大小</param>
        /// <param name="filePath">完整的ini文件名</param>
        /// <returns></returns>
        public static int ReadConfigFile(string section, string key, string def, StringBuilder retVal, int size, string filePath)
        {
            return NativeMethods.GetPrivateProfileString(section, key, def, retVal, size, filePath);
        }

        #endregion

        #region 加载dll
        internal static class NativeMethods
        {
            [DllImport("Kernel32", CharSet = CharSet.Unicode)]
            public static extern int LoadLibrary(string funcname);
            [DllImport("kernel32")]
            public static extern int WritePrivateProfileString(string section, string key, string val, string filePath);
            [DllImport("kernel32")]
            public static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);
        }
        #endregion
    }
}
