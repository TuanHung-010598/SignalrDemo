using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SignalrChat.Controllers
{
    public class HomeController : Controller
    {
        ChatHub chatHub;
        [Route("api/get")]
        public string Send()
        {
            chatHub = new ChatHub();
            chatHub.Send("System admin", "Message from controller");
            return "susscess";

        }
    }
}