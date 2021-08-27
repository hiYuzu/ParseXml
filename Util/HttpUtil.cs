using System;
using System.IO;
using System.Net;
using System.Text;

namespace ParseXml.Util
{
    public class HttpUtil
    {
        /// <summary>
        /// HTTP POST 请求
        /// </summary>
        /// <param name="url">地址</param>
        /// <param name="postData">数据</param>
        /// <returns></returns>
        public static string HttpPost(string url, string postData)
        {
            try
            {
                string result = "";
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
                req.Method = "POST";
                req.Timeout = 3000;
                req.ContentType = "application/json";
                byte[] data = Encoding.UTF8.GetBytes(postData);
                req.ContentLength = data.Length;
                using (Stream reqStream = req.GetRequestStream())
                {
                    reqStream.Write(data, 0, data.Length);
                    reqStream.Close();
                }
                HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
                Stream stream = resp.GetResponseStream();
                //获取响应内容
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                {
                    result = reader.ReadToEnd();
                }
                return result;
            }
            catch (Exception ex)
            {
                return "发送失败：" + ex.Message;
            }
        }
    }
}
