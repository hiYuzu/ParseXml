using ParseXml.Log4Net;
using System.IO;

namespace ParseXml.Util
{
    public class FileUtil
    {
        /// <summary>
        /// 移动文件（复制或剪切）
        /// </summary>
        /// <param name="oldFile">旧文件完整路径</param>
        /// <param name="newFile">目标文件完整路径</param>
        /// <param name="delSource">是否删除源文件</param>
        public static void MoveFile(string oldFile, string newFile, bool delSource)
        {
            try
            {
                if(delSource)
                {
                    File.Move(oldFile, newFile);
                }
                else
                {
                    File.Copy(oldFile, newFile);
                }
            }
            catch
            {
                string reNewFile = newFile.Substring(0, newFile.Length - 4);
                reNewFile += "_r.xml";
                Log4NetUtil.Warn("文件重名存在，重命名：" + newFile + "->" + reNewFile);
                MoveFile(oldFile, reNewFile, delSource);
            }
        }
    }
}
