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
            ofertaCarga.FechaCreacion = DateTime.Now.Date;
            if (ofertaCarga.FechaOferta < ofertaCarga.FechaCreacion)
            {
                TempData["errorFecha"] = "Debe indicar una fecha futura en la que su oferta debe realizarse. Dicha oferta aparecerá disponible mientras no haya pasado su fecha indicada.";
                return View(ofertaCarga);
            }
            if (ModelState.IsValid)
            {
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

            return View(ofertaCarga);
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
            ofertaTransporte.FechaCreacion = DateTime.Now.Date;
            if (ofertaTransporte.FechaOferta < ofertaTransporte.FechaCreacion)
            {
                TempData["errorFecha"] = "Debe indicar una fecha futura en la que su oferta debe realizarse. Dicha oferta aparecerá disponible mientras no haya pasado su fecha indicada.";
                return View(ofertaTransporte);
            }
            if (ModelState.IsValid)
            {
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

            return View(ofertaTransporte);
        }

        public ActionResult DetallesOfertaCargaCliente(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Error", "Home");
            }
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
                return RedirectToAction("Error", "Home");
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
            //Duda: ESTO LO VE TODO EL MUNDO? R: Aparentemente se almacena en el servidor y no en el navegador.
            Session.Add("idOferta", idOferta);
            return View();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //Este metodo debe impactar 3 cosas en la bd:
        //1) Crear contacto con IdOfertaContactada e IdContactante
        //2) Guardar contacto en lista de contactos de la Oferta (como cada oferta tiene el Id de su creador este podra acceder y ver quienes lo contactaron).
        //3) Guardar Contacto en la lista de contactados del User que es el Cliente "Contactante"
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateContacto([Bind(Include = "ContactoId,Calificacion,Comentario,FechaContacto")] Contacto contacto)
        {// Busco en la bd porque necesito traer ListaContactos y Ofertante
            int idOfertaAux = (int)Session["idOferta"]; //id obtenido en DetallesOfertaTransporteCliente / DetallesOfertaCargaCliente
            contacto.FechaContacto = DateTime.Now;
            contacto.Estado = "En progreso";
            contacto.IdOfertaContactada = idOfertaAux;
            var userId = User.Identity.GetUserId();
            contacto.IdContactante = userId;
            OfertaCarga OfertaCAux = db.OfertasCarga.Include(o => o.ListaContactos).FirstOrDefault(o => o.OfertaId == idOfertaAux);
            if (OfertaCAux == null)
            {
                OfertaTransporte OfertaTAux = db.OfertasTransporte.Include(o => o.ListaContactos).FirstOrDefault(o => o.OfertaId == idOfertaAux);
                if(OfertaTAux != null)
                {
                    OfertaTAux.ListaContactos.Add(contacto); //Agrego contacto a lista de contactos de la oferta
                    var userAux = db.Users.Include(o => o.ListaContactados).FirstOrDefault(u => u.Id == OfertaTAux.OfertanteId); //Obtengo User con su lista de contactados
                    userAux.ListaContactados.Add(contacto); //Agrego contacto a lista de contactados de User
                    db.Entry(OfertaTAux).State = EntityState.Modified;
                    db.Entry(userAux).State = EntityState.Modified;
                }
                else
                {
                    //entra si OfertaTAux == null y OfertaCAux == null
                    return RedirectToAction("Error", "Home");
                }
            }
            else
            {
                OfertaCAux.ListaContactos.Add(contacto); //Agrego contacto a lista de contactos de la oferta
                var userAux2 = db.Users.Include(o => o.ListaContactados).FirstOrDefault(u => u.Id == OfertaCAux.OfertanteId); //Obtengo User con su lista de contactados
                userAux2.ListaContactados.Add(contacto);//Agrego contacto a lista de contactados de User
                db.Entry(OfertaCAux).State = EntityState.Modified;
                db.Entry(userAux2).State = EntityState.Modified;
            }
            if (ModelState.IsValid)
            {//Si todo esta en orden debería poder guardar contacto en bd y correr SaveChanges();
                db.Contactos.Add(contacto);
                Session.Remove("idOferta");
                db.SaveChanges();
                return RedirectToAction("DatosOfertante", new { id = contacto.IdOfertaContactada });
            }
            return RedirectToAction("Error", "Home");
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