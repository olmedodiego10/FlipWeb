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

        public ActionResult DetallesOfertaCargaCliente(int? id)
        {
            if (id == null)
            {
                // return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            // OfertaCarga ofertaCarga = db.OfertasCarga.Include(o => o.of).FirstOrDefault(o => o.OfertaId == id);
            // OfertaCarga ofertaCarga db.Users.Include(u => u.ListaOfertasCargaCreadas).First(u => u.Id == id);
            OfertaCarga ofertaCarga = db.OfertasCarga.Include("ListaContactos").FirstOrDefault(o => o.OfertaId == id);
            if (ofertaCarga == null)
            {
                return HttpNotFound();
            }
            return View(ofertaCarga);
        }
        public ActionResult DetallesOfertaTransporteCliente(int? id)
        {
            if (id == null)
            {
               // return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OfertaTransporte ofertaTransporte = db.OfertasTransporte.Include("ListaContactos").FirstOrDefault(o => o.OfertaId == id);
            if (ofertaTransporte == null)
            {
                return HttpNotFound();
            }
            return View(ofertaTransporte);
        }
        public ActionResult CreateContacto(int idOferta)
        {
            //Duda: ESTO LO VE TODO EL MUNDO?
            Session.Add("idOferta", idOferta);
            return View();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateContacto([Bind(Include = "ContactoId,Calificacion,Comentario,FechaContacto")] Contacto contacto)
        {// Busco en la bd porque necesito traer ListaContactos y Ofertante
            int idAux = (int)Session["idOferta"]; //id validado en DetallesOfertaTransporteCliente / DetallesOfertaCargaCliente
            var OfertaCAux = db.OfertasCarga.Include(o => o.ListaContactos).FirstOrDefault(o => o.OfertaId == idAux);
            if (OfertaCAux == null)
            {
                var OfertaTAux = db.OfertasTransporte.Include(o => o.ListaContactos).FirstOrDefault(o => o.OfertaId == idAux);
                contacto.ContactoId = contacto.ContactoId++;
                db.Entry(OfertaTAux).State = EntityState.Modified;
            }
            else
            {
                contacto.ContactoId = contacto.ContactoId++;
                db.Entry(OfertaCAux).State = EntityState.Modified;
            }
            contacto.FechaContacto = DateTime.Now;
            contacto.Estado = "En progreso";
            contacto.IdOfertaContactada = (int)Session["idOferta"];
            var userId = User.Identity.GetUserId();
            contacto.IdContactante = userId;
            Session.Remove("idOferta");
            if (ModelState.IsValid)
            {
                db.Contactos.Add(contacto);
                db.SaveChanges();
                return RedirectToAction("DatosOfertante", new { id = contacto.IdOfertaContactada });
            }
            return RedirectToAction("Error", "Clientes");
        }

        public ActionResult DatosOfertante(int? id)
        {


            Oferta oferta = db.Ofertas.Find(id);
            var userAux = db.Users.Find(oferta.OfertanteId);


            if (userAux == null)
            {
                return HttpNotFound();
            }
            return View(userAux);
        }

        public ActionResult ListadoClientes()
        {
            return View(db.Users.ToList());
        }
    }
}