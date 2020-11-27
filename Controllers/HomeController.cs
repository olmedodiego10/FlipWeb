using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FlipWeb.Domain;
using FlipWeb.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;

namespace FlipWeb.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private ApplicationUserManager _userManager;

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public ActionResult Index()
        {
            var cargas = db.OfertasCarga.ToList();
            var transporte = db.OfertasTransporte.ToList();
            MenuUsuariosViewModel vista = new MenuUsuariosViewModel() { ListadoOfertasTransporte = transporte, ListadoOfertasCarga = cargas };
            return View(vista);
        }

        public ActionResult MenuUsuarios()
        {
            var cargas = db.OfertasCarga.ToList();
            var transporte = db.OfertasTransporte.ToList();
            MenuUsuariosViewModel vista = new MenuUsuariosViewModel() { ListadoOfertasTransporte = transporte, ListadoOfertasCarga = cargas };
            return View(vista);
        }

        public ActionResult MenuAdmins()
        {
            var cargas = db.OfertasCarga.ToList();
            var transporte = db.OfertasTransporte.ToList();
            MenuUsuariosViewModel vista = new MenuUsuariosViewModel() { ListadoOfertasTransporte = transporte, ListadoOfertasCarga = cargas };
            return View(vista);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        // GET
        public ActionResult CreateOfertaCarga()
        {
            return View();
        }

        // POST:
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateOfertaCarga([Bind(Include = "OfertaId,Estado,Detalles,PaisPartida,CiudadPartida,DireccionPartida,PaisDestino,CiudadDestino,DireccionDestino,FechaOferta,FechaCreacion,DescripcionMercaderia,RequiereExclusividad")] OfertaCarga ofertaCarga)
        {
            if (ModelState.IsValid)
            {
                ofertaCarga.FechaCreacion = DateTime.Now.Date;
                ofertaCarga.Estado = "En progreso";
                db.OfertasCarga.Add(ofertaCarga);
                var userId = User.Identity.GetUserId();
                ofertaCarga.OfertanteId = userId;
                //var userAux = UserManager.FindById(User.Identity.GetUserId());
                var userAux = db.Users.Include(u => u.ListaOfertasCargaCreadas).First(u => u.Id == userId);
                userAux.ListaOfertasCargaCreadas.Add(ofertaCarga);
                db.Entry(userAux).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("MenuUsuarios", "Home");
            }

            return RedirectToAction("MenuUsuarios", "Home");
        }

        // GET
        public ActionResult CreateOfertaTransporte()
        {
            return View();
        }

        // POST
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateOfertaTransporte([Bind(Include = "OfertaId,Estado,Detalles,PaisPartida,CiudadPartida,DireccionPartida,PaisDestino,CiudadDestino,DireccionDestino,FechaOferta,FechaCreacion,MedidasCaja,TipoCaja,TipoCamion,ITV,HabilitacionBromatologica,Costo")] OfertaTransporte ofertaTransporte)
        {
            if (ModelState.IsValid)
            {
                ofertaTransporte.FechaCreacion = DateTime.Now.Date;
                ofertaTransporte.Estado = "En progreso";
                db.OfertasTransporte.Add(ofertaTransporte);
                var userId = User.Identity.GetUserId();
                ofertaTransporte.OfertanteId = userId;
                //var userAux = UserManager.FindById(User.Identity.GetUserId());
                var userAux = db.Users.Include(u => u.ListaOfertasTransporteCreadas).First(u => u.Id == userId);
                userAux.ListaOfertasTransporteCreadas.Add(ofertaTransporte);
                db.Entry(userAux).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("MenuUsuarios", "Home");
            }

            return RedirectToAction("MenuUsuarios", "Home");
        }
    }
}