using System;
using System.IO;
using System.Net;
using System.Web;
using System.Web.Http;

namespace NFine.Web.ReceiveReport {
    /// <summary>
    /// 后台接收文件的Web API
    /// </summary>
    public class FileController : ApiController {

        [HttpPost]
        public IHttpActionResult PostFile() {
            try {
                //根据文件类型 判断是啥报告
                var file = HttpContext.Current.Request.Files["myFile"]; //文件
                var fileType = HttpContext.Current.Request["fileType"]; //扩展名 标识文件类型
                var fileName = HttpContext.Current.Request["fileName"]; //文件名
                var fullPath = HttpContext.Current.Request["fullPath"]; //文件完整路径
                var _savePath = "~/App_Data/"; //保存的路径
              
                file?.SaveAs(HttpContext.Current.Server.MapPath(_savePath) + fileName);
                HttpContext.Current.Response.Flush();
                HttpContext.Current.Response.End();
                return Ok("success2");
            } catch (Exception ex) {
                return Content<string>(HttpStatusCode.Accepted, ex.Message);
            }
        }


    }
}
