using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Bandwidth.Net;
using Bandwidth.Net.Model;
using Microsoft.AspNet.Identity.Owin;
using TranscriptionApp.Models;

namespace TranscriptionApp.Lib
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
                    var pi = ev.GetType().GetProperty("CallId");
                    if (pi != null)
                    {
                        var call = new Call {Id = (string)pi.GetValue(ev)};
                        var activeCall =
                            filterContext.RequestContext.HttpContext.GetOwinContext()
                                .Get<ApplicationDbContext>()
                                .ActiveCalls.Find(call.Id);
                        if (activeCall != null)
                        {
                            filterContext.Controller.ViewBag.User = activeCall.User;
                        }
                        filterContext.Controller.ViewBag.Call = call;
                    }
                    filterContext.Controller.ViewBag.Event = ev;
                }
            }
            catch (Exception ex)
            {
                Debugger.Log(0, "Error", ex.Message);
                Trace.WriteLine(ex.Message, "Error");
                filterContext.Result = new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Malformed event");
                return;
            }
            base.OnActionExecuting(filterContext);
        }
    }
}