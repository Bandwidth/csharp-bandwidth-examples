using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Web.Mvc;
using Bandwidth.Net;

namespace DolphinApp.Lib
{
    public sealed class ExtractEventAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            try
            {
                using (var reader = new StreamReader(filterContext.RequestContext.HttpContext.Request.InputStream,
                    Encoding.UTF8))
                {
                    var json = reader.ReadToEnd();
                    var ev = BaseEvent.CreateFromString(json);
                    filterContext.Controller.ViewBag.Event = ev;
                }
            }
            catch (Exception ex)
            {
                Debugger.Log(0, "Error", ex.Message);
                filterContext.Result = new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Malformed event");
                return;
            }
            base.OnActionExecuting(filterContext);
        }
    }
}