using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebKinect.Models;

namespace WebKinect.Controllers
{
    [HandleError]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var kinect = Kinect.FindByInstanceName("USB\\VID_0409&PID_005A\\6&5EA421D&0&4");

            if (null != kinect)
                ViewData["Message"] = kinect.InstanceName;

            return View();
        }

        public ActionResult About()
        {
            return View();
        }
    }
}
