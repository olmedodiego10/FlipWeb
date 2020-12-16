﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using FlipWeb.Domain;
using FlipWeb.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System.Drawing;
using System.Drawing.Imaging;

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
        public ActionResult CreateOfertaCarga([Bind(Include = "OfertaId,Estado,Detalles,PaisPartida,CiudadPartida,DireccionPartida,PaisDestino,CiudadDestino,DireccionDestino,FechaOferta,FechaCreacion,DescripcionMercaderia,RequiereExclusividad,Imagen1")] OfertaCarga ofertaCarga)
        {
            HttpPostedFileBase FileBase = Request.Files[0];
            if (FileBase.ContentLength == 0 && FileBase.FileName == "")
            {
                TempData["errorImagen"] = "El campo de imagen es obligatorio.";
                return View(ofertaCarga);
            }
            WebImage image = new WebImage(FileBase.InputStream);
            if(image.ImageFormat == "jpg" || image.ImageFormat == "jpeg")
            {
                ofertaCarga.Imagen1 = image.GetBytes();
            }
            else
            {
                TempData["errorImagen"] = "Su imagen debe estar en formato .jpg / .jpeg";
                return View(ofertaCarga);
            }

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
        public ActionResult EditOfertaCarga(int idOferta)
        {
            OfertaCarga ofertaCarga = db.OfertasCarga.Find(idOferta);
            TempData["FechaOriginalOferta"] = ofertaCarga.FechaOferta;
            return View(ofertaCarga);
        }


        // POST:
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditOfertaCarga([Bind(Include = "OfertaId,OfertanteId,Estado,Detalles,PaisPartida,CiudadPartida,DireccionPartida,PaisDestino,CiudadDestino,DireccionDestino,FechaOferta,FechaCreacion,DescripcionMercaderia,RequiereExclusividad,Imagen1")] OfertaCarga ofertaCarga)
        {
            HttpPostedFileBase FileBase = Request.Files[0];
            if (FileBase.ContentLength != 0 && FileBase.FileName != "")
            {
                WebImage image = new WebImage(FileBase.InputStream);
                if (image.ImageFormat == "jpg" || image.ImageFormat == "jpeg")
                {
                    ofertaCarga.Imagen1 = image.GetBytes();
                }
                else
                {
                    TempData["errorImagen"] = "Su imagen debe estar en formato .jpg / .jpeg";
                    return View(ofertaCarga);
                }
            }

            //HiddenFor para fecha oferta no se esta sobrescribiendo
            //por lo que lo quitamos y lo validamos en el controlador
            if (ofertaCarga.FechaOferta.ToString() == "01/01/0001 0:00:00")
            {
                //El usuario debe volver a confirmar manualmente la fecha de su oferta
                //de lo contrario no pasa las validaciones del modelo y no podemos quitar 
                //las validaciones del modelo sin hacer un view model
                ofertaCarga.FechaOferta = (DateTime)TempData["FechaOriginalOferta"];
                return View(ofertaCarga);
            }

            if (ofertaCarga.FechaOferta < DateTime.Now.Date)
            {
                TempData["errorFecha"] = "Debe indicar una fecha futura en la que su oferta debe realizarse. Dicha oferta aparecerá disponible mientras no haya pasado su fecha indicada.";
                return View(ofertaCarga);
            }
            if (ModelState.IsValid)
            {
                db.Entry(ofertaCarga).State = EntityState.Modified;
                db.SaveChanges();
                TempData["mensajeOk"] = "Oferta modificada.";
                return RedirectToAction("DetallesOfertaCargaPropia", "Home", new { id = ofertaCarga.OfertaId });
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
        public ActionResult CreateOfertaTransporte([Bind(Include = "OfertaId,OfertanteId,Estado,Detalles,PaisPartida,CiudadPartida,DireccionPartida,PaisDestino,CiudadDestino,DireccionDestino,FechaOferta,FechaCreacion,MedidasCaja,TipoCaja,TipoCamion,ITV,HabilitacionBromatologica,Costo,Imagen1")] OfertaTransporte ofertaTransporte)
        {
            HttpPostedFileBase FileBase = Request.Files[0];
            if (FileBase.ContentLength == 0 && FileBase.FileName == "")
            {
                TempData["errorImagen"] = "El campo de imagen es obligatorio.";
                return View(ofertaTransporte);
            }
            WebImage image = new WebImage(FileBase.InputStream);
            if (image.ImageFormat == "jpg" || image.ImageFormat == "jpeg")
            {
                ofertaTransporte.Imagen1 = image.GetBytes();
            }
            else
            {
                TempData["errorImagen"] = "Su imagen debe estar en formato .jpg / .jpeg";
                return View(ofertaTransporte);
            }

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
                var userAux = db.Users.Include(u => u.ListaOfertasTransporteCreadas).First(u => u.Id == userId);
                userAux.ListaOfertasTransporteCreadas.Add(ofertaTransporte);
                db.Entry(userAux).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("MenuUsuarios", "Home");
            }

            return View(ofertaTransporte);
        }

        // GET
        public ActionResult EditOfertaTransporte(int idOferta)
        {
            OfertaTransporte ofertaTransporte = db.OfertasTransporte.Find(idOferta);
            TempData["FechaOriginalOferta"] = ofertaTransporte.FechaOferta;
            return View(ofertaTransporte);
        }

        // POST
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditOfertaTransporte([Bind(Include = "OfertaId,OfertanteId,Estado,Detalles,PaisPartida,CiudadPartida,DireccionPartida,PaisDestino,CiudadDestino,DireccionDestino,FechaOferta,FechaCreacion,MedidasCaja,TipoCaja,TipoCamion,ITV,HabilitacionBromatologica,Costo,Imagen1")] OfertaTransporte ofertaTransporte)
        {
            HttpPostedFileBase FileBase = Request.Files[0];
            if (FileBase.ContentLength != 0 && FileBase.FileName != "")
            {
                WebImage image = new WebImage(FileBase.InputStream);
                if (image.ImageFormat == "jpg" || image.ImageFormat == "jpeg")
                {
                    ofertaTransporte.Imagen1 = image.GetBytes();
                }
                else
                {
                    TempData["errorImagen"] = "Su imagen debe estar en formato .jpg / .jpeg";
                    return View(ofertaTransporte);
                }
            }

            //HiddenFor para fecha oferta no se esta sobrescribiendo
            //por lo que lo quitamos y lo validamos en el controlador
            if (ofertaTransporte.FechaOferta.ToString() == "01/01/0001 0:00:00")
            {
                //El usuario debe volver a confirmar manualmente la fecha de su oferta
                //de lo contrario no pasa las validaciones del modelo y no podemos quitar 
                //las validaciones del modelo sin hacer un view model
                ofertaTransporte.FechaOferta = (DateTime)TempData["FechaOriginalOferta"];
                return View(ofertaTransporte);
            }

            if (ofertaTransporte.FechaOferta < DateTime.Now.Date)
            {
                TempData["errorFecha"] = "Debe indicar una fecha futura en la que su oferta debe realizarse. Dicha oferta aparecerá disponible mientras no haya pasado su fecha indicada.";
                return View(ofertaTransporte);
            }
            if (ModelState.IsValid)
            {
                db.Entry(ofertaTransporte).State = EntityState.Modified;
                db.SaveChanges();
                TempData["mensajeOk"] = "Oferta modificada.";
                return RedirectToAction("DetallesOfertaTransportePropia", "Home", new { id = ofertaTransporte.OfertaId });
            }

            return View(ofertaTransporte);
        }

        public ActionResult DetailsOfertaCargaUser(int? id)
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
            if (ofertaCarga.OfertanteId == User.Identity.GetUserId())
            {
                return RedirectToAction("DetallesOfertaCargaPropia", new {id = ofertaCarga.OfertaId });
         
            }
            return View(ofertaCarga);
        }

        public ActionResult DetallesOfertaCargaPropia(int? id)
        {
            OfertaCarga ofertaCarga = db.OfertasCarga.Include("ListaContactos").FirstOrDefault(o => o.OfertaId == id);

            if (ofertaCarga == null)
            {
                return HttpNotFound();
            }
            return View(ofertaCarga);
        }

        public ActionResult DetailsOfertaTransporteUser(int? id)
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
            if (ofertaTransporte.OfertanteId == User.Identity.GetUserId())
            {
                return RedirectToAction("DetallesOfertaTransportePropia", new { id = ofertaTransporte.OfertaId });

            }
            return View(ofertaTransporte);
        }
        
        public ActionResult DetallesOfertaTransportePropia(int? id)
        {
            OfertaTransporte ofertaTransporte = db.OfertasTransporte.Include("ListaContactos").FirstOrDefault(o => o.OfertaId == id);

            if (ofertaTransporte == null)
            {
                return HttpNotFound();
            }
            return View(ofertaTransporte);
        }

        public ActionResult FinalizarOferta(int idOferta)
        {
           return View(idOferta);
        }

        public ActionResult VolverADetallesOfertaPropia(int idOferta)
        {
            if(db.OfertasCarga.Any(o => o.OfertaId == idOferta))
                return RedirectToAction("DetallesOfertaCargaPropia", "Home", new { id = idOferta });
            else
                return RedirectToAction("DetallesOfertaTransportePropia", "Home", new { id = idOferta });
        }

        public ActionResult FinalizarOfertaConfirmado(int idOferta)
        {
            OfertaCarga OfertaCAux = db.OfertasCarga.Include(o => o.ListaContactos).FirstOrDefault(o => o.OfertaId == idOferta);
            if (OfertaCAux == null)
            {
                OfertaTransporte OfertaTAux = db.OfertasTransporte.Include(o => o.ListaContactos).FirstOrDefault(o => o.OfertaId == idOferta);
                if (OfertaTAux != null)
                {
                    OfertaTAux.Estado = "Finalizada";
                    db.Entry(OfertaTAux).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("DetallesOfertaTransportePropia", "Home", new { id = idOferta });
                }
                else
                {
                    //entra si OfertaTAux == null y OfertaCAux == null
                    return RedirectToAction("Error", "Home");
                }
            }
            OfertaCAux.Estado = "Finalizada";
            db.Entry(OfertaCAux).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("DetallesOfertaCargaPropia", "Home", new { id = idOferta });
        }

        public ActionResult CreateContacto(int idOferta)
        {
            //Duda: ESTO LO VE TODO EL MUNDO? R: Aparentemente se almacena en el servidor y no en el navegador.
            Session.Add("idOferta", idOferta);
            return View();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //Este método debe impactar 3 cosas en la bd:
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

        public ActionResult UsersList(string email = "")
        { 
            IEnumerable<ApplicationUser> usuarios = db.Users.Where(u => u.Email == email );
            if (usuarios != null)
            {
                return View(usuarios);
            }
            else
            {
                return View();
            }  
        }

        public ActionResult DetailsUsers(string id)
        {
            if (id == null)
            {
                return RedirectToAction("Error", "Home");
            }
            var user = db.Users.Find(id);
            if (user == null)
            {
                return RedirectToAction("Error", "Home");
            }
            return View(user);
        }

        public ActionResult AssignRoleAdministrador()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AssignRoleAdministrador(string id)
        {
            var user = db.Users.Find(id);

            if (user == null)
            {
                return HttpNotFound();

            }
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));
            roleManager.Create(new IdentityRole("Administrador"));
            UserManager.AddToRole(user.Id, "Administrador");
            UserManager.RemoveFromRole(user.Id, "Cliente");
            user.RolString = "Administrador";
            db.SaveChanges();

            return RedirectToAction("UsersList", "Home");
        }

        public ActionResult getImageOfertaCarga(int id)
        {

            OfertaCarga ofertaCarga = db.OfertasCarga.Find(id);
            byte[] byteImage = ofertaCarga.Imagen1;
            MemoryStream memoryStream = new MemoryStream(byteImage);
            System.Drawing.Image image = System.Drawing.Image.FromStream(memoryStream);
            memoryStream = new MemoryStream();
            image.Save(memoryStream, ImageFormat.Jpeg);
            memoryStream.Position = 0;
            return File(memoryStream, "image/jpg");
        }

        public ActionResult getImageOfertaTransporte(int id)
        {

            OfertaTransporte ofertaTransporte = db.OfertasTransporte.Find(id);
            byte[] byteImage = ofertaTransporte.Imagen1;
            MemoryStream memoryStream = new MemoryStream(byteImage);
            System.Drawing.Image image = System.Drawing.Image.FromStream(memoryStream);
            memoryStream = new MemoryStream();
            image.Save(memoryStream, ImageFormat.Jpeg);
            memoryStream.Position = 0;
            return File(memoryStream, "image/jpg");
        }
    }


}