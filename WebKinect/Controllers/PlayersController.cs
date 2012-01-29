using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebKinect.Models;
namespace WebKinect.Controllers
{
    public class PlayersController : Controller
    {
        //
        // GET: /Players/1
        // GET: /Players/

        public JsonResult Index(int? id)
        {
            var players = new WebKinect.Models.Player[6];
            lock (Player.Players)
            {
                //int i = 0;
                //foreach(var player in Player.Players)
                //{
                //    players[i] = new Player(i);
                //}
                return Json(Player.Players, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
