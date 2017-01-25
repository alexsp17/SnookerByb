using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Awpbs.Web.App.Controllers
{
    public class InvitesController : Controller
    {
        private readonly ApplicationDbContext db;

        public InvitesController()
        {
            db = new ApplicationDbContext();
        }

        // GET: Invites
        public ActionResult Invite(int id)
        {
            var gameHost = new GameHostsLogic(db).Get(id, 0);

            return View("Invite", gameHost);
        }
    }
}