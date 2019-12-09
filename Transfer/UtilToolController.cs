
using System.Net;
using System.IO;
using System.Text;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Threading;

namespace Transfer
{
    #region 
    public class FileDTTool
    {



        public static void WatcherStrat(string path, string url = "", string filter = "")
        {
            try
            {
                FileSystemWatcher watcher = new FileSystemWatcher();
                watcher.Path = path;
                watcher.Filter = filter; //"*.txt    *.pdf  *.png  "
                                         //这个会调用两遍
                                         //watcher.Changed += new FileSystemEventHandler(OnProcess);
                                         //watcher.Created += new FileSystemEventHandler(OnProcess);
                                         //watcher.Deleted += new FileSystemEventHandler(OnProcess);
                                         //watcher.Renamed += new RenamedEventHandler(OnRenamed);

                watcher.Created += (sender, FileSystemEventArgs) =>
                {


                    if (FileSystemEventArgs.ChangeType == WatcherChangeTypes.Created)
                    {
                        _ = new FileDTTool().SendRequest(url, FileSystemEventArgs.FullPath);
                    }
                };
                watcher.EnableRaisingEvents = true;
                watcher.NotifyFilter = NotifyFilters.Attributes | NotifyFilters.CreationTime | NotifyFilters.DirectoryName | NotifyFilters.FileName | NotifyFilters.LastAccess
                                       | NotifyFilters.LastWrite | NotifyFilters.Security | NotifyFilters.Size;
                watcher.IncludeSubdirectories = true;
            }
            catch (Exception ex)
            {
                //Form1.sb.AppendLine("---------------------------------------");
                //Form1.sb.AppendLine(ex.Message+"请手动设置！");
                //Form1.RichTextBox.Text = Form1.sb?.ToString();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="url">API地址</param>
        /// <param name="path">文件完整路径</param>
        public async Task<string> SendRequest(string url, string path)
        {
            Thread.Sleep(100);
            string fileName = Path.GetFileName(path); //文件名
            string extensionName = Path.GetExtension(path); //扩展名
            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            MultipartFormDataContent mulContent = new MultipartFormDataContent("----WebKitFormBoundaryrXRBKlhEeCbfHIY");
            HttpContent fileContent = new StreamContent(fs);
            HttpClient client = new HttpClient();
            HttpStatusCode statusCode = HttpStatusCode.NotImplemented;
            //string Result = "   ";
            try
            {
                //Form1.sb.AppendFormat("发现新增文件: {0}，准备传输......\n", path);
                
                //https://www.cnblogs.com/TiestoRay/p/4877978.html
                client.MaxResponseContentBufferSize = 256000;
                client.DefaultRequestHeaders.Add("user-agent",
                    "User-Agent    Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; Touch; MALNJS; rv:11.0) like Gecko");//设置请求头
                HttpResponseMessage response;
                fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");//设置媒体类型
                mulContent.Add(fileContent, "myFile", fileName); //文件流  加 //文件名
                mulContent.Add(new StringContent(extensionName), "fileType"); //扩展名 标识文件类型
                mulContent.Add(new StringContent(fileName), "fileName"); //文件名
                mulContent.Add(new StringContent(path), "fullPath"); //文件完整路径
                response = await client.PostAsync(new Uri(url), mulContent);

                response.EnsureSuccessStatusCode();
                statusCode = response.StatusCode;
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    //Form1.sb.AppendFormat("已成功传输文件: {0}，响应状态码:{1}\n", fileName, response.StatusCode);

                    //Result = "传输成功!";
                }
                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                //Form1.sb.AppendFormat("出现错误，已终止传输: {0}，响应状态码:{1}\n", ex.Message, statusCode);
                //Result = "传输失败!";
                return ex.Message;
            }
            finally
            {
                mulContent.Dispose();
                fileContent.Dispose();
                fs.Dispose();
                fs.Close();
                client.Dispose();
              //  Form1.sb.AppendFormat("本次传输已结束，状态:{0} ，时间:{1}\n", Result, DateTime.Now.ToString());
              //  Form1.sb.AppendLine("---------------------------------------");
              //Form1.RichTextBox.Text = Form1.sb?.ToString();
            }
        }
    }
    #endregion
}