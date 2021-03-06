﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Bandwidth.Net;
using Bandwidth.Net.Model;
using CallApp.Models;
using Microsoft.AspNet.Identity.Owin;

namespace CallApp.Controllers
{
    public class HomeController : Controller
    {
        // GET / (and /Home)
        public ActionResult Index()
        {
            ViewBag.UserNumbers = DataProvider.GetUserNumbers();
            return View(DataProvider.GetContacts());
        }

        //POST /Home/Call
        [HttpPost, ActionName("Call")]
        public async Task<ActionResult> PostCall(MakeCall call)
        {
            try
            {
                if (string.IsNullOrEmpty(call.From)) throw new ArgumentException("Field From is missing");
                if (string.IsNullOrEmpty(call.To)) throw new ArgumentException("Field To is missing");
                var c = await Call.Create(Client, new Dictionary<string, object>
                {
                    {"from", PhoneNumberForCallbacks},
                    {"to", call.From},
                    {"callbackUrl", BaseUrl + Url.Action("CatapultFromCallback")},
                    {"tag", call.To}
                });
                Debug.WriteLine("Call Id to {0} is {1}", call.From, c.Id);
                return Json(new object());
            }
            catch (Exception ex)
            {
                return Json(new {error = ex.Message});
            }
        }

        //POST /Home/CatapultFromCallback (it's used for Catapult events for call to "from" number)
        [HttpPost, ActionName("CatapultFromCallback")]
        public async Task<ActionResult> PostCatapultFromCallback()
        {
            try
            {
                using (var reader = new StreamReader(Request.InputStream, Encoding.UTF8))
                {
                    var json = await reader.ReadToEndAsync();
                    Debug.WriteLine("CatapultFromCallback:" + json);
                    dynamic ev = BaseEvent.CreateFromString(json);
                    await ProcessCatapultFromEvent(ev);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format("CatapultFromCallback error: {0}", ex.Message));
            }
            return Json(new object());
        }

        //POST /Home/CatapultToCallback (it's used for Catapult events for call to "to" number)
        [HttpPost, ActionName("CatapultToCallback")]
        public async Task<ActionResult> PostCatapultToCallback()
        {
            try
            {
                using (var reader = new StreamReader(Request.InputStream, Encoding.UTF8))
                {
                    var json = await reader.ReadToEndAsync();
                    Debug.WriteLine("PostCatapultToCallback:" + json);
                    dynamic ev = BaseEvent.CreateFromString(json);
                    await ProcessCatapultToEvent(ev);
                    return Json(new object());
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format("CatapultToCallback error: {0}", ex.Message));
            }
            return Json(new object());
        }

        private async Task ProcessCatapultFromEvent(AnswerEvent ev)
        {
            // "from" number answered a call. speak him a message
            var call = new Call { Id = ev.CallId };
            call.SetClient(Client);
            var contact = DataProvider.FindContactByPhoneNumber(ev.Tag);
            if (contact == null)
            {
                throw new Exception("Missing contact for number " + ev.Tag);
            }
            await call.SpeakSentence(string.Format("We are connecting you to {0}", contact.Name), ev.Tag);
        }

        private async Task ProcessCatapultFromEvent(SpeakEvent ev)
        {
            if (ev.Status != "done") return;
            //a messages was spoken to "from" number. calling to "to" number
            var c = await Call.Create(Client, new Dictionary<string, object>
            {
                {"from", PhoneNumberForCallbacks},
                {"to", ev.Tag},
                {"callbackUrl", BaseUrl + Url.Action("CatapultToCallback")},
                {"tag", ev.CallId}
            });
            Debug.WriteLine("Call Id to {0} is {1}", ev.Tag, c.Id);
        }

        private Task ProcessCatapultFromEvent(BaseEvent ev)
        {
            //other events handler
            return Task.FromResult(0);
        }

        private async Task ProcessCatapultToEvent(AnswerEvent ev)
        {
            //"to" number answered a call. Making a bridge with "from" number's call
            var b = await Bridge.Create(Client, new[] {ev.CallId, ev.Tag}, true);
            Debug.WriteLine(string.Format("BridgeId is {0}", b.Id));
        }

        private Task ProcessCatapultToEvent(BaseEvent ev)
        {
            //other events handler
            return Task.FromResult(0);
        }

        private DataProvider _dataProvider;
        public DataProvider DataProvider
        {
            get
            {
                return _dataProvider ?? Request.GetOwinContext().Get<DataProvider>();
            }
            set { _dataProvider = value; }
        }

        private Client _client;
        public Client Client
        {
            get
            {
                return _client ?? Request.GetOwinContext().Get<Client>("catapultClient");
            }
            set { _client = value; }
        }

        private string _phoneNumberForCallbacks;
        public string PhoneNumberForCallbacks
        {
            get
            {
                return _phoneNumberForCallbacks ?? Request.GetOwinContext().Get<string>("phoneNumberForCallbacks");
            }
            set { _phoneNumberForCallbacks = value; }
        }
        
        private string _baseUrl;
        public string BaseUrl
        {
            get
            {
                return _baseUrl ?? Request.GetOwinContext().Get<string>("baseUrl");
            }
            set { _baseUrl = value; }
        }
    }
}