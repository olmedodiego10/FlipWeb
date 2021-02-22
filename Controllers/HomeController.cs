using System;
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

        DateTime fechaActual = DateTime.Now;

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

        [AllowAnonymous]
        public ActionResult Index()
        {
           
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("MenuUsuarios", "Home");
            }
            var cargas = (from o in db.OfertasCarga
                          where (o.Estado == "En progreso" && DbFunctions.TruncateTime(o.FechaOferta) >= DbFunctions.TruncateTime(fechaActual))
                          select o).OrderByDescending(o => o.FechaCreacion).Take(4).ToList();
            var transporte = (from o in db.OfertasTransporte
                              where (o.Estado == "En progreso" && DbFunctions.TruncateTime(o.FechaOferta) >= DbFunctions.TruncateTime(fechaActual))
                              select o).OrderByDescending(o => o.FechaCreacion).Take(4).ToList();

            MenuUsuariosViewModel vista = new MenuUsuariosViewModel() { ListadoOfertasTransporte = transporte, ListadoOfertasCarga = cargas };
            return View(vista);
        }

        [AllowAnonymous]
        public FileResult DescargaManualUsuario()
        {
            var sDocument = Server.MapPath("~/Files/manualusuarioflip.pdf");
            return File(sDocument, "application/pdf", sDocument);
        }

        [Authorize(Roles = "Administrador")]
        public FileResult DescargaManualUsuarioAdmin()
        {
            var sDocument = Server.MapPath("~/Files/manualusuarioflipadmin.pdf");
            return File(sDocument, "application/pdf", sDocument);
        }

        [AllowAnonymous]
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        [AllowAnonymous]
        public ActionResult Error()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [Authorize]
        public ActionResult ListarOfertasCarga()
        {
            var cargas = (from o in db.OfertasCarga
                          where (o.Estado == "En progreso" && DbFunctions.TruncateTime(o.FechaOferta) >= DbFunctions.TruncateTime(fechaActual))
                          select o).OrderByDescending(o => o.FechaCreacion).ToList(); ;
            return View(cargas);
        }

        [Authorize]
        public ActionResult ListarOfertasTransporte()
        {
            var transporte = (from o in db.OfertasTransporte
                              where (o.Estado == "En progreso" && DbFunctions.TruncateTime(o.FechaOferta) >= DbFunctions.TruncateTime(fechaActual))
                              select o).OrderByDescending(o => o.FechaCreacion).ToList(); ;
            return View(transporte);
        }

        [Authorize]
        public ActionResult MenuUsuarios()
        {
            var cargas = (from o in db.OfertasCarga
                          where (o.Estado == "En progreso" )
                          select o).OrderByDescending(o => o.FechaCreacion).Take(4).ToList();
            var transporte = (from o in db.OfertasTransporte
                              where (o.Estado == "En progreso" && DbFunctions.TruncateTime(o.FechaOferta) >= DbFunctions.TruncateTime(fechaActual))
                              select o).OrderByDescending(o => o.FechaCreacion).Take(4).ToList(); ;
            MenuUsuariosViewModel vista = new MenuUsuariosViewModel() { ListadoOfertasTransporte = transporte, ListadoOfertasCarga = cargas };
            return View(vista);
        }
        
        [Authorize(Roles = "Administrador")]
        public ActionResult MenuAdmins()
        {
            var cargas = (from o in db.OfertasCarga
                          where (o.Estado == "En progreso" && DbFunctions.TruncateTime(o.FechaOferta) >= DbFunctions.TruncateTime(fechaActual))
                          select o).OrderByDescending(o => o.FechaCreacion).Take(4).ToList(); ;
            var transporte = (from o in db.OfertasTransporte
                              where (o.Estado == "En progreso" && DbFunctions.TruncateTime(o.FechaOferta) >= DbFunctions.TruncateTime(fechaActual))
                              select o).OrderByDescending(o => o.FechaCreacion).Take(4).ToList(); ;
            MenuUsuariosViewModel vista = new MenuUsuariosViewModel() { ListadoOfertasTransporte = transporte, ListadoOfertasCarga = cargas };
            return View(vista);
        }

        [Authorize]
        public ActionResult HistorialOfertante()
        {
            if (User.Identity.IsAuthenticated)
            {
                string UserId = User.Identity.GetUserId().ToString();
                var cargas = (from o in db.OfertasCarga
                              where (o.OfertanteId == UserId)
                              select o).OrderByDescending(o => o.FechaCreacion).ToList(); ;
                var transporte = (from o in db.OfertasTransporte
                                  where (o.OfertanteId == UserId)
                                  select o).OrderByDescending(o => o.FechaCreacion).ToList(); ;
                MenuUsuariosViewModel vista = new MenuUsuariosViewModel() { ListadoOfertasTransporte = transporte, ListadoOfertasCarga = cargas };
                return View(vista);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [Authorize(Roles = "Administrador")]
        public ActionResult HistorialAdministrador()
        {
            var cargas = (from o in db.OfertasCarga
                              select o).OrderByDescending(o => o.FechaCreacion).ToList(); ;
                var transporte = (from o in db.OfertasTransporte
                                  select o).OrderByDescending(o => o.FechaCreacion).ToList(); ;
                MenuUsuariosViewModel vista = new MenuUsuariosViewModel() { ListadoOfertasTransporte = transporte, ListadoOfertasCarga = cargas };
                return View(vista);
        }

        [Authorize]
        public ActionResult OfertasActivas()
        {
           
            if (User.Identity.IsAuthenticated)
            {
                string UserId = User.Identity.GetUserId().ToString();
                var cargas = (from o in db.OfertasCarga
                              where (o.OfertanteId == UserId && o.Estado == "En progreso" &&  DbFunctions.TruncateTime(o.FechaOferta) >= DbFunctions.TruncateTime(fechaActual))
                              select o).OrderByDescending(o => o.FechaCreacion).ToList(); 
                var transporte = (from o in db.OfertasTransporte
                                  where (o.OfertanteId == UserId && o.Estado == "En progreso" && DbFunctions.TruncateTime(o.FechaOferta) >= DbFunctions.TruncateTime(fechaActual))
                                  select o).OrderByDescending(o => o.FechaCreacion).ToList(); ;
                MenuUsuariosViewModel vista = new MenuUsuariosViewModel() { ListadoOfertasTransporte = transporte, ListadoOfertasCarga = cargas };
                return View(vista);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [Authorize]
        public ActionResult BusquedaRapidaOferta(int? idOferta)
        {

            if (!idOferta.HasValue)
            {

                TempData["errorBusqueda"] = "Debe ingresar un código de oferta";
                return RedirectToAction("MenuUsuarios", "Home");

            }
            Oferta oferta = db.Ofertas.Find(idOferta);

            var usuarioId = User.Identity.GetUserId();
            var usuario = db.Users.Find(usuarioId);
            if (oferta == null || oferta.Estado != "En progreso" || oferta.FechaOferta.Date < fechaActual.Date)
            {

                if (usuario.RolString == "Cliente")
                {
                    TempData["errorBusqueda"] = "El id ingresado no corresponde a ninguna oferta o ya no se encuentra activa.";
                    return RedirectToAction("MenuUsuarios", "Home");
                }
                if (usuario.RolString == "Administrador")
                {
                    TempData["errorBusqueda"] = "El id ingresado no corresponde a ninguna oferta o ya no se encuentra activa. Puedes buscar la misma en el Historial de Ofertas.";
                    return RedirectToAction("MenuAdmins", "Home");
                }
            }
           
            // si la oferta es de carga redirecciona al action que muestra la oferta de carga
            if (oferta is OfertaCarga)
                {
                    return RedirectToAction("BusquedaRapidaOfertaCarga", "Home", new { OfertaId = oferta.OfertaId });
                }//si la oferta es de transporte muestra la vista con la oferta de transporte
                if (oferta is OfertaTransporte)
                {
                    return View(oferta);
                }
            
            return View(oferta);
        }

        [Authorize]
        public ActionResult BusquedaRapidaOfertaCarga(int? OfertaId)
        {
            OfertaCarga ofertaCarga = db.OfertasCarga.Find(OfertaId);

            if (ofertaCarga != null)
            {

                return View(ofertaCarga);

            }
            return RedirectToAction("MenuUsuarios", "Home");
        }

        public ActionResult BusquedaRapidaHistorialAdmin(int idOferta)
        {
            Oferta oferta = db.Ofertas.Find(idOferta);

            if (oferta == null)
            {
                TempData["errorBusqueda"] = "El id ingresado no corresponde a ninguna oferta";
                return RedirectToAction("HistorialAdministrador", "Home");
            }
            else
            {
                if (oferta is OfertaCarga)
                {
                    return RedirectToAction("BusquedaRapidaOfertaCargaHistorialAdmin", "Home", new { OfertaId = oferta.OfertaId });
                }//si la oferta es de transporte muestra la vista con la oferta de transporte
                if (oferta is OfertaTransporte)
                {
                    return View(oferta);
                }

            }
            return View();
        }

        public ActionResult BusquedaRapidaOfertaCargaHistorialAdmin(int OfertaId)
        {
            OfertaCarga ofertaCarga = db.OfertasCarga.Find(OfertaId);

            if (ofertaCarga != null)
            {

                return View(ofertaCarga);

            }
            return RedirectToAction("HistorialAdministrador", "Home");
        }
        
        [Authorize]
        public ActionResult BusquedaOfertaConFiltros(string TipoOferta, string PaisPartida, string CiudadPartida, string PaisDestino, string CiudadDestino, DateTime? FechaDesde, DateTime? FechaHasta, string TipoCamion, string TipoCaja)
        {

            if (TipoOferta == "todasLasOfertas" && TipoCamion == "" && TipoCaja == "")
            {
                var carg = (from o in db.OfertasCarga
                            where (o.Estado == "En progreso") &&
                            DbFunctions.TruncateTime(o.FechaOferta) >= DbFunctions.TruncateTime(fechaActual) &&
                            ((CiudadPartida == "") || (o.CiudadPartida.ToUpper() == CiudadPartida.ToUpper())) &&
                            ((FechaDesde == null) || (o.FechaOferta >= FechaDesde)) &&
                            ((FechaHasta == null) || (o.FechaOferta <= FechaHasta)) &&
                            ((PaisPartida == "") || (o.PaisPartida.ToUpper() == PaisPartida.ToUpper())) &&
                            ((CiudadDestino == "") || (o.CiudadDestino.ToUpper() == CiudadDestino.ToUpper())) &&
                            ((PaisDestino == "") || (o.PaisDestino.ToUpper() == PaisDestino.ToUpper()))
                            select o).OrderByDescending(o => o.FechaCreacion).ToList(); ;

                var transp = (from o in db.OfertasTransporte
                              where (o.Estado == "En progreso") &&
                               DbFunctions.TruncateTime(o.FechaOferta) >= DbFunctions.TruncateTime(fechaActual) &&
                              ((CiudadPartida == "") || (o.CiudadPartida.ToUpper() == CiudadPartida.ToUpper())) &&
                              ((FechaDesde == null) || (o.FechaOferta >= FechaDesde)) &&
                              ((FechaHasta == null) || (o.FechaOferta <= FechaHasta)) &&
                              ((PaisPartida == "") || (o.PaisPartida.ToUpper() == PaisPartida.ToUpper())) &&
                              ((CiudadDestino == "") || (o.CiudadDestino.ToUpper() == CiudadDestino.ToUpper())) &&
                              ((PaisDestino == "") || (o.PaisDestino.ToUpper() == PaisDestino.ToUpper()))
                              select o).OrderByDescending(o => o.FechaCreacion).ToList(); ;

                if (transp.Count() == 0 && carg.Count == 0)
                {
                    TempData["errorFiltros"] = "No existe oferta que coincida con los parametros ingresados";
                    return RedirectToAction("MenuUsuarios", "Home");
                }
                if (transp.Count() == 0)
                {
                    MenuUsuariosViewModel vistaTransp = new MenuUsuariosViewModel() { ListadoOfertasTransporte = null, ListadoOfertasCarga = carg };
                    return View(vistaTransp);
                }
                if (carg.Count() == 0)
                {
                    MenuUsuariosViewModel vistaCarg = new MenuUsuariosViewModel() { ListadoOfertasTransporte = transp, ListadoOfertasCarga = null };
                    return View(vistaCarg);
                }

                MenuUsuariosViewModel vista = new MenuUsuariosViewModel() { ListadoOfertasTransporte = transp, ListadoOfertasCarga = carg };
                return View(vista);
            }

            if (TipoOferta == "ofertaCarga")
            {
                var carg = (from o in db.OfertasCarga
                            where (o.Estado == "En progreso") &&
                            DbFunctions.TruncateTime(o.FechaOferta) >= DbFunctions.TruncateTime(fechaActual) &&
                            ((CiudadPartida == "") || (o.CiudadPartida.ToUpper() == CiudadPartida.ToUpper())) &&
                            ((FechaDesde == null) || (o.FechaOferta >= FechaDesde)) &&
                            ((FechaHasta == null) || (o.FechaOferta <= FechaHasta)) &&
                            ((PaisPartida == "") || (o.PaisPartida.ToUpper() == PaisPartida.ToUpper())) &&
                            ((CiudadDestino == "") || (o.CiudadDestino.ToUpper() == CiudadDestino.ToUpper())) &&
                            ((PaisDestino == "") || (o.PaisDestino.ToUpper() == PaisDestino.ToUpper()))
                            select o).OrderByDescending(o => o.FechaCreacion).ToList(); ;
                if (carg.Count() == 0)
                {
                    TempData["errorFiltros"] = "No existe oferta de carga que coincida con los parametros ingresados";
                    return RedirectToAction("MenuUsuarios", "Home");
                }
                MenuUsuariosViewModel vista = new MenuUsuariosViewModel() { ListadoOfertasTransporte = null, ListadoOfertasCarga = carg };
                return View(vista);

            }

            if (TipoOferta == "ofertaTransporte" || TipoCaja != "" || TipoCamion != "")
            {

                var transporte = (from o in db.OfertasTransporte
                                  where (o.Estado == "En progreso") &&
                                  DbFunctions.TruncateTime(o.FechaOferta) >= DbFunctions.TruncateTime(fechaActual) &&
                                  ((CiudadPartida == "") || (o.CiudadPartida.ToUpper() == CiudadPartida.ToUpper())) &&
                                  ((FechaDesde == null) || (o.FechaOferta >= FechaDesde)) &&
                                  ((FechaHasta == null) || (o.FechaOferta <= FechaHasta)) &&
                                  ((PaisPartida == "") || (o.PaisPartida.ToUpper() == PaisPartida.ToUpper())) &&
                                  ((CiudadDestino == "") || (o.CiudadDestino.ToUpper() == CiudadDestino.ToUpper())) &&
                                  ((PaisDestino == "") || (o.PaisDestino.ToUpper() == PaisDestino.ToUpper())) &&
                                  ((TipoCamion == "") || (o.TipoCamion.ToUpper() == TipoCamion.ToUpper())) &&
                                  ((TipoCaja == "") || (o.TipoCaja.ToUpper() == TipoCaja.ToUpper()))
                                  select o).OrderByDescending(o => o.FechaCreacion).ToList(); ;
                if (transporte.Count() == 0)
                {
                    TempData["errorFiltros"] = "No existe oferta de transporte que coincida con los parametros ingresados";
                    return RedirectToAction("MenuUsuarios", "Home");
                }
                MenuUsuariosViewModel vista = new MenuUsuariosViewModel() { ListadoOfertasTransporte = transporte, ListadoOfertasCarga = null };
                return View(vista);
            }

            return View();
        }


        // GET
        [Authorize]
        public ActionResult CreateOfertaCarga()
        {
            return View();
        }

        // POST:
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult CreateOfertaCarga([Bind(Include = "OfertaId,Estado,Detalles,PaisPartida,CiudadPartida,DireccionPartida,PaisDestino,CiudadDestino,DireccionDestino,FechaOferta,FechaCreacion,DescripcionMercaderia,RequiereExclusividad,Imagen1")] OfertaCarga ofertaCarga)
        {
            HttpPostedFileBase FileBase = Request.Files[0];
            if (FileBase.ContentLength == 0 && FileBase.FileName == "")
            {
                TempData["errorImagen"] = "El campo de imagen es obligatorio.";
                return View(ofertaCarga);
            }
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

            ofertaCarga.FechaCreacion = DateTime.Now;
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
        [Authorize]
        public ActionResult EditOfertaCarga(int idOferta)
        {
            OfertaCarga ofertaCarga = db.OfertasCarga.Find(idOferta);
            string Userid = User.Identity.GetUserId();
            if(Userid != ofertaCarga.OfertanteId)
            {
                TempData["mensajeError"] = "La oferta de código: " + idOferta + " no te pertenece.";
                return RedirectToAction("Error", "Home");
            }
            TempData["FechaOriginalOferta"] = ofertaCarga.FechaOferta;
            return View(ofertaCarga);
        }

        // POST:
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
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

            //con HiddenFor la fecha oferta no se esta sobrescribiendo
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
        [Authorize]
        public ActionResult CreateOfertaTransporte()
        {
            return View();
        }

        // POST
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
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

            ofertaTransporte.FechaCreacion = DateTime.Now;
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
        [Authorize]
        public ActionResult EditOfertaTransporte(int idOferta)
        {
            OfertaTransporte ofertaTransporte = db.OfertasTransporte.Find(idOferta);
            string Userid = User.Identity.GetUserId();
            if (Userid != ofertaTransporte.OfertanteId)
            {
                TempData["mensajeError"] = "La oferta de código: " + idOferta + " no te pertenece.";
                return RedirectToAction("Error", "Home");
            }
            TempData["FechaOriginalOferta"] = ofertaTransporte.FechaOferta;
            return View(ofertaTransporte);
        }

        // POST
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
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

        [Authorize]
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
                return RedirectToAction("DetallesOfertaCargaPropia", new { id = ofertaCarga.OfertaId });

            }
            return View(ofertaCarga);
        }

        [Authorize]
        public ActionResult DetallesOfertaCargaPropia(int? id)
        {
            OfertaCarga ofertaCarga = db.OfertasCarga.Include("ListaContactos").FirstOrDefault(o => o.OfertaId == id);

            if (ofertaCarga == null)
            {
                return HttpNotFound();
            }

            return View(ofertaCarga);
        }

        [Authorize(Roles = "Administrador")]
        public ActionResult DetallesOfertaCargaAdministrador(int? id)
        {
            OfertaCarga ofertaCarga = db.OfertasCarga.Include("ListaContactos").FirstOrDefault(o => o.OfertaId == id);

            if (ofertaCarga == null)
            {
                return HttpNotFound();
            }
            return View(ofertaCarga);
        }

        [Authorize]
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

        [Authorize]
        public ActionResult DetallesOfertaTransportePropia(int? id)
        {
            OfertaTransporte ofertaTransporte = db.OfertasTransporte.Include("ListaContactos").FirstOrDefault(o => o.OfertaId == id);

            if (ofertaTransporte == null)
            {
                return HttpNotFound();
            }
            return View(ofertaTransporte);
        }

        [Authorize(Roles = "Administrador")]
        public ActionResult DetallesOfertaTransporteAdministrador(int? id)
        {
            OfertaTransporte ofertaTransporte = db.OfertasTransporte.Include("ListaContactos").FirstOrDefault(o => o.OfertaId == id);

            if (ofertaTransporte == null)
            {
                return HttpNotFound();
            }
            return View(ofertaTransporte);
        }

        [Authorize(Roles = "Administrador")]
        public ActionResult VerOfertaAdministrador(int? idOferta)
        {
            Oferta oferta = db.Ofertas.Find(idOferta);
            if (oferta is OfertaCarga)
            {
                return RedirectToAction("DetallesOfertaCargaAdministrador", new { id = oferta.OfertaId });
            }
            if (oferta is OfertaTransporte)
            {
                return RedirectToAction("DetallesOfertaTransporteAdministrador", new { id = oferta.OfertaId });
            }
            return RedirectToAction("Menu", "Home");
        }

        [Authorize]
        public ActionResult DetallesOfertaGeneral(int idOferta)
        {
            if (db.OfertasCarga.Any(o => o.OfertaId == idOferta))
                return RedirectToAction("DetailsOfertaCargaUser", "Home", new { id = idOferta });
            else
                return RedirectToAction("DetailsOfertaTransporteUser", "Home", new { id = idOferta });
        }

        [Authorize]
        public ActionResult DenunciarOferta(int idOferta)
        {
            Oferta oferta = db.Ofertas.Find(idOferta);
            var userId = User.Identity.GetUserId();

            if (db.Reportes.Any(r => r.ofertaDenunciadaId == idOferta && r.DeuncianteId == userId))
            {
                if (oferta is OfertaCarga)
                {
                    TempData["mensaje"] = "Ya has denunciado esta oferta. La misma se encuentra en revisión.";
                    return RedirectToAction("DetailsOfertaCargaUser", new { id = idOferta });
                }
                if (oferta is OfertaTransporte)
                {

                    TempData["mensaje"] = "Ya has denunciado esta oferta. La misma se encuentra en revisión.";
                    return RedirectToAction("DetailsOfertaTransporteUser", new { id = idOferta });
                }

            }
            Session.Add("idOferta", idOferta);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult DenunciarOferta([Bind(Include = "Motivo,Detalle")] Reporte reporte)
        {
            if (Session["idOferta"] == null)
            {
                return RedirectToAction("Error", "Home");
            }
            int idOferta = (int)Session["idOferta"];
            if (reporte.Motivo == "seleccionarMotivo")
            {
                TempData["mensaje"] = "Es necesario seleccionar el motivo de la denuncia.";
                return RedirectToAction("DenunciarOferta", new { idOferta = idOferta });

            }
            if(reporte.Motivo == "otros" && reporte.Detalle ==null)
            {
                TempData["mensaje"] = "En caso de seleccionar 'Otro' es necesario determinar el motivo en los detalles.";
                return RedirectToAction("DenunciarOferta", new { idOferta = idOferta });
            }

            var userId = User.Identity.GetUserId();
            Oferta oferta = db.Ofertas.Find(idOferta);
            ApplicationUser denunciante = db.Users.Find(userId);
            ApplicationUser denunciado = db.Users.Find(oferta.OfertanteId);

            reporte.Fecha = DateTime.Now;
            reporte.CorreoDenunciado = denunciado.Email;
            reporte.CorreoDenunciante = denunciante.Email;
            reporte.DeuncianteId = userId;
            reporte.ofertaDenunciadaId = idOferta;
            reporte.DenunciadoId = oferta.OfertanteId;
            reporte.Estado = "Abierto";

            if (ModelState.IsValid)
            {
                db.Reportes.Add(reporte);
                Session.Remove("idOferta");
                db.SaveChanges();
                if (oferta is OfertaCarga)
                {
                    TempData["mensajeok"] = "Oferta denunciada, un administrador revisará la misma.";
                    return RedirectToAction("DetailsOfertaCargaUser", new { id = idOferta });
                }
                if (oferta is OfertaTransporte)
                {
                    TempData["mensajeok"] = "Oferta reportada, un administrador revisará su denuncia,";
                    return RedirectToAction("DetailsOfertaTransporteUser", new { id = idOferta });
                }
            }
            return RedirectToAction("Error", "Home");
        }

        [Authorize(Roles = "Administrador")]
        public ActionResult DarDeBajaOferta(int idOferta)
        {
            return View(idOferta);
        }

        [Authorize(Roles = "Administrador")]
        public ActionResult DarDeBajaOfertaConfirmado(int idOferta)
        {
            Oferta oferta = db.Ofertas.Find(idOferta);
            oferta.Estado = "Reportada";
            db.Entry(oferta).State = EntityState.Modified;
            db.SaveChanges();
            if (oferta is OfertaCarga)
            {
                return RedirectToAction("DetallesOfertaCargaAdministrador", new { id = idOferta });
            }
            if (oferta is OfertaTransporte)
            {
                return RedirectToAction("DetallesOfertaTransporteAdministrador", new { id = idOferta });
            }

            return RedirectToAction("ReportadosLista", "Home");
        }

        [Authorize(Roles = "Administrador")]
        public ActionResult ReActivarOferta(int idOferta)
        {
            return View(idOferta);
        }
        
        public ActionResult ReActivarOfertaConfirmado(int idOferta)
        {
            Oferta oferta = db.Ofertas.Find(idOferta);
            if (oferta.FechaOferta.Date >= fechaActual.Date)
            {
                oferta.Estado = "En progreso";
                db.Entry(oferta).State = EntityState.Modified;
                db.SaveChanges();
                if (oferta is OfertaCarga)
                {
                    TempData["mensajeOk"] = "Oferta activada";
                    return RedirectToAction("DetallesOfertaCargaAdministrador", new { id = idOferta });
                }
                if (oferta is OfertaTransporte)
                {
                    TempData["mensajeOk"] = "Oferta activada";
                    return RedirectToAction("DetallesOfertaTransporteAdministrador", new { id = idOferta });
                }
            }
            else
            {
                oferta.Estado = "Finalizada";
                db.Entry(oferta).State = EntityState.Modified;
                db.SaveChanges();
                if (oferta is OfertaCarga)
                {
                    TempData["mensajeOk"] = "La oferta ha caducado.";
                    return RedirectToAction("DetallesOfertaCargaAdministrador", new { id = idOferta });
                }
                if (oferta is OfertaTransporte)
                {
                    TempData["mensajeOk"] = "La oferta ha caducado.";
                    return RedirectToAction("DetallesOfertaTransporteAdministrador", new { id = idOferta });
                }
            }
            return RedirectToAction("MenuAdmins", "Home");
        }

        [Authorize(Roles = "Administrador")]
        public ActionResult ReportadosLista()
        {

            var reportesAbiertos = (from o in db.Reportes
                                    where (o.Estado == "Abierto")
                                    select o).ToList();
            var reportesCerrados = (from o in db.Reportes
                                    where (o.Estado == "Cerrado")
                                    select o).ToList();
            ReporteViewModel vista = new ReporteViewModel() { ListadoReportesAbiertos = reportesAbiertos, ListadoReportesCerrados = reportesCerrados };
            return View(vista);
        }

        [Authorize(Roles = "Administrador")]
        public ActionResult CerrarReporte(int? idReporte)
        {

            Session.Add("idReporte", idReporte);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public ActionResult CerrarReporte(string Resolucion)
        {
            if (Session["idReporte"] == null)
            {
                return RedirectToAction("Error", "Home");
            }
            int idReporte = (int)Session["idReporte"];
            if(Resolucion == "")
            {
                TempData["mensaje"] = "La resolución es obligatoria para cerrar el reporte.";
                return RedirectToAction("CerrarReporte", new { idReporte = idReporte });
            }
            var reporte = db.Reportes.Find(idReporte);
            if (reporte != null)
            {
                reporte.Resolucion = Resolucion;
                reporte.Estado = "Cerrado";
                db.Entry(reporte).State = EntityState.Modified;
                db.SaveChanges();
                Session.Remove("idReporte");
                return RedirectToAction("ReportadosLista", "Home");
            }
            return RedirectToAction("ReportadosLista", "Home");
        }

        [Authorize(Roles = "Administrador")]
        public ActionResult BuscarReporte(DateTime? Fecha)
        {
            if(Fecha == null)
            {
                TempData["errorFecha"] = "Debes seleccionar una fecha para buscar un reporte.";
                return RedirectToAction("ReportadosLista", "Home");
            }
            DateTime fecha = Fecha.Value;
            if(fecha > DateTime.Now.Date)
            {
                TempData["errorFecha"] = "La fecha ingresada no puede ser posterior al dia actual.";
                return RedirectToAction("ReportadosLista", "Home");
            }

            var reportesAbiertos = (from o in db.Reportes
                                    where (o.Estado == "Abierto")
                                    select o).ToList();
            var reportesCerrados = (from r in db.Reportes
                                    where (DbFunctions.TruncateTime(r.Fecha) == DbFunctions.TruncateTime(Fecha) && r.Estado == "Cerrado")
                                    select r).ToList();
            if(reportesCerrados.Count == 0)
            {
                TempData["errorFecha"] = "No se encontraron reportes en la fecha indicada.";
                return RedirectToAction("ReportadosLista", "Home");
            }
            ReporteViewModel vista = new ReporteViewModel() { ListadoReportesAbiertos = reportesAbiertos, ListadoReportesCerrados = reportesCerrados };
            return View(vista);
        }

        [Authorize(Roles = "Administrador")]
        public ActionResult BloquearCuentaUsuario(string id)
        {
            BloquearUsuarioViewModel bloquearViewModel = new BloquearUsuarioViewModel();
            bloquearViewModel.UsuarioId = id;
            Session.Add("idUsuario", id);
            return View(bloquearViewModel);

        }

        [Authorize(Roles = "Administrador")]
        public ActionResult BloquearUsuarioConfirmado(string Duracion)
        {
            if (Session["idUsuario"] == null)
            {
                return RedirectToAction("Error", "Home");
            }
            string usuarioId = (string)Session["idUsuario"];
            var usuario = db.Users.Find(usuarioId);
            if(Duracion == "")
            {
                TempData["mensaje"] = "Es necesario seleccionar una duración.";
                return RedirectToAction("BloquearCuentaUsuario", new { id = usuarioId });
            }
            if (Duracion == "sieteDias")
            {
                usuario.LockoutEndDateUtc = DateTime.Now.AddDays(2);
                db.SaveChanges();
            }
            if (Duracion == "treintaDias")
            {
                usuario.LockoutEndDateUtc = DateTime.Now.AddDays(30);
                db.SaveChanges();
            }
            if (Duracion == "unAño")
            {
                usuario.LockoutEndDateUtc = DateTime.Now.AddYears(1);
                db.SaveChanges();
            }
            if (Duracion == "permanente")
            {
                usuario.LockoutEndDateUtc = DateTime.MaxValue;
                db.SaveChanges();
            }
            Session.Remove("idUsuario");
            return RedirectToAction("DetailsUsers", new { id = usuarioId });
        }

        [Authorize(Roles = "Administrador")]
        public ActionResult DesbloquearUsuario(string id)
        {
            if (Session["idOferta"] == null)
            {
                return RedirectToAction("Error", "Home");
            }
            if (id != null)
            {
                BloquearUsuarioViewModel bloquearViewModel = new BloquearUsuarioViewModel();
                bloquearViewModel.UsuarioId = id;
                Session.Add("idUsuario", id);
                return View(bloquearViewModel);
            }
            return RedirectToAction("Menu", "Home");
        }

        [Authorize(Roles = "Administrador")]
        public ActionResult DesbloquearUsuarioConfirmado()
        {
            string usuarioId = (string)Session["idUsuario"];

            var usuario = db.Users.Find(usuarioId);
            usuario.LockoutEndDateUtc = DateTime.Now;
            db.SaveChanges();
            TempData["mensajeError"] = "Este contacto ya fue realizado anteriormente por lo que el ofertante no será notificado";

            return RedirectToAction("DetailsUsers", new { id = usuarioId });
        }

        [Authorize]
        public ActionResult FinalizarOferta(int idOferta)
        {
            return View(idOferta);
        }

        [Authorize]
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
                    return RedirectToAction("Menu", "Home");
                }
            }
            OfertaCAux.Estado = "Finalizada";
            db.Entry(OfertaCAux).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("DetallesOfertaCargaPropia", "Home", new { id = idOferta });
        }

        [Authorize]
        public ActionResult ContactosList(int idOferta)
        {
            OfertaCarga OfertaCAux = db.OfertasCarga.Include(o => o.ListaContactos).FirstOrDefault(o => o.OfertaId == idOferta);
            if (OfertaCAux == null)
            {
                OfertaTransporte OfertaTAux = db.OfertasTransporte.Include(o => o.ListaContactos).FirstOrDefault(o => o.OfertaId == idOferta);
                if (OfertaTAux != null)
                {
                    return View(OfertaTAux.ListaContactos);
                }
                else
                {
                    //entra si OfertaTAux == null y OfertaCAux == null
                    return RedirectToAction("Error", "Home");
                }
            }
            return View(OfertaCAux.ListaContactos);
        }

        [Authorize]
        public ActionResult ContactadosList()
        {
            var userId = User.Identity.GetUserId();
            //IEnumerable<Contacto> listaContactados = db.Contactos.Where(c => c.IdContactante == userId);
            var user = db.Users.Include(o => o.ListaContactados).FirstOrDefault(u => u.Id == userId);
            return View(user.ListaContactados);
        }

        [Authorize]
        public ActionResult CreateContacto(int idOferta)
        {
            //Con este if evitamos conflictos con volver acceder al POST ya habiendo creado el contacto
            //ej: por ruta / por botón atrás (history.back())
            var userId = User.Identity.GetUserId();
            if (db.Contactos.Any(c => c.IdOfertaContactada == idOferta && c.IdContactante == userId))
            {
                TempData["mensajeError"] = "Este contacto ya fue realizado anteriormente por lo que el ofertante no será notificado";
                return RedirectToAction("DatosOfertante", new { idOferta = idOferta });
            }
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
        [Authorize]
        public ActionResult CreateContacto([Bind(Include = "ContactoId,Calificacion,Comentario,FechaContacto")] Contacto contacto)
        {
            if(Session["idOferta"] == null)
            {
                return RedirectToAction("Error", "Home");
            }
            int idOfertaAux = (int)Session["idOferta"]; //id obtenido en DetallesOfertaTransporteCliente / DetallesOfertaCargaCliente
            var userId = User.Identity.GetUserId();
            //Verifico si contacto ya existe en toda la BD 
            if (db.Contactos.Any(c => c.IdOfertaContactada == idOfertaAux && c.IdContactante == userId))
            {
                TempData["mensajeError"] = "Este contacto ya fue realizado anteriormente por lo que el ofertante no será notificado";
                return RedirectToAction("DatosOfertante", new { idOferta = idOfertaAux });
            }

            //Verifico que último contacto de Usuario activo este calificado si usuario no es Premium
            var userAux = db.Users.Include(o => o.ListaContactados).FirstOrDefault(u => u.Id == userId); //Obtengo User con su lista de contactados
            int Last = userAux.ListaContactados.Count - 1;
            if (Last >= 0)
            {
                if (userAux.ListaContactados[Last].Calificacion == 0)
                {
                    if (userAux.Premium == false)
                    {
                        TempData["mensajeError"] = "Los usuarios gratuitos deben calificar y comentar el contacto previo para poder realizar uno nuevo, en caso de no recordar su contacto previo, puede presionar en Volver y en el menu de Ofertas puede consultar la lista de Contactos ordenada por fecha en la opción Contactados.";
                        return RedirectToAction("CalificarContacto", "Home", new { idContacto = userAux.ListaContactados[Last].ContactoId });
                    }
                }
            }
            contacto.FechaContacto = DateTime.Now;
            contacto.Estado = "En progreso";
            contacto.IdOfertaContactada = idOfertaAux;
            contacto.IdContactante = userId;
            //Busco la oferta
            OfertaCarga OfertaCAux = db.OfertasCarga.Include(o => o.ListaContactos).FirstOrDefault(o => o.OfertaId == idOfertaAux);
            if (OfertaCAux == null)
            {
                OfertaTransporte OfertaTAux = db.OfertasTransporte.Include(o => o.ListaContactos).FirstOrDefault(o => o.OfertaId == idOfertaAux);
                if (OfertaTAux != null)
                {
                    OfertaTAux.ListaContactos.Add(contacto); //Agrego contacto a lista de contactos de la oferta
                    userAux.ListaContactados.Add(contacto); //Agrego contacto a lista de contactados de User
                    db.Entry(OfertaTAux).State = EntityState.Modified;
                    db.Entry(userAux).State = EntityState.Modified;
                }
                else
                {
                    //entra si OfertaTAux == null y OfertaCAux == null | no debería suceder
                    return RedirectToAction("Error", "Home");
                }
            }
            else
            {
                OfertaCAux.ListaContactos.Add(contacto); //Agrego contacto a lista de contactos de la oferta
                userAux.ListaContactados.Add(contacto);//Agrego contacto a lista de contactados de User
                db.Entry(OfertaCAux).State = EntityState.Modified;
                db.Entry(userAux).State = EntityState.Modified;
            }
            if (ModelState.IsValid)
            {//Si todo esta en orden debería poder guardar contacto en bd y SaveChanges();
                db.Contactos.Add(contacto);
                Session.Remove("idOferta");
                db.SaveChanges();
                TempData["mensajeOk"] = "Contacto realizado, el propietario de la oferta también podrá visualizar sus datos de contacto.";
                return RedirectToAction("DatosOfertantePrimerContacto", new { idOferta = contacto.IdOfertaContactada });
            }
            return RedirectToAction("Error", "Home");
        }

        [Authorize]
        public ActionResult CalificarContacto(int idContacto)
        {
            Contacto con = db.Contactos.FirstOrDefault(c => c.ContactoId == idContacto);
            if (con.Comentario != null && con.Calificacion > 0)
            {
                TempData["mensajeError"] = "Este contacto ya fue calificado.";
                return RedirectToAction("ContactadosList", "Home");
            }
            return View(con);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult CalificarContacto([Bind(Include = "ContactoId, IdOfertaContactada, Calificacion,Comentario,FechaContacto")] Contacto contacto)
        {
            if (contacto.Calificacion == 0)
            {
                TempData["mensajeErrorCalificacion"] = "Debe seleccionar una calificación.";
                return View(contacto);
            }
            if (contacto.Comentario == null)
            {
                TempData["mensajeErrorComentario"] = "Debe escribir un comentario.";
                return View(contacto);
            }
            contacto.Estado = "Finalizado";
            contacto.IdContactante = User.Identity.GetUserId();
            db.Entry(contacto).State = EntityState.Modified;
            db.SaveChanges();
            TempData["mensajeOk"] = "Contacto calificado. Ya puede realizar un nuevo contacto en caso de que su cuenta sea gratuita.";
            if (Session["idOferta"] == null)
            {
                return RedirectToAction("ContactadosList", "Home");
            }
            else
            {
                return RedirectToAction("DetallesOfertaGeneral", "Home", new { idOferta = (int)Session["idOferta"] });
            }
        }

        //Nota: el nombre de la vista y de este método quedaron mal, puede ser usado con cualquier usuario (ej. ver DatosContactante(int idContactante))
        //se cargó una tarea de rename al backlog de tareas pendientes, NO intentar hacerlo rápido porque hay lugares que no cambia automático.
        [Authorize]
        public ActionResult DatosOfertante(int idOferta)
        {
            //No podemos pasar como parámetro de ruta el id de los usuarios debido a que es muy complejo
            //Esto nos lleva a tener que hacer una doble búsqueda cada vez que necesitamos obtener un usuario
            //que no sea el que tiene su sesión abierta
            Oferta oferta = db.Ofertas.Find(idOferta);
            var userAux = db.Users.Find(oferta.OfertanteId);
            return View(userAux);
        }

        [Authorize]
        public ActionResult DatosOfertantePrimerContacto(int idOferta)
        {
            //Duplicamos esta instancia para evitar los posibles problemas que se generan entre la primera vez que se crea el contacto
            //y las siguientes veces que el usuario por error o con otras intenciones intente acceder nuevamente ya habiendo contactado.
            Oferta oferta = db.Ofertas.Find(idOferta);
            var userAux = db.Users.Find(oferta.OfertanteId);
            return View(userAux);
        }

        [Authorize]
        public ActionResult DatosContactante(int idContacto)
        {
            //No podemos pasar como parámetro de ruta el id de los usuarios debido a que es muy complejo
            //Esto nos lleva a tener que hacer una doble búsqueda cada vez que necesitamos obtener un usuario
            //que no sea el que tiene su sesión abierta
            Contacto con = db.Contactos.Find(idContacto);
            var Contactante = db.Users.Find(con.IdContactante);
            return View("DatosOfertante", Contactante);
        }

        [Authorize]
        public ActionResult UsersList()
        {
            var userId = User.Identity.GetUserId();
            IEnumerable<ApplicationUser> usuarios = (from u in db.Users
                                                     where u.Id != userId
                                                     select u).Take(5);
            return View(usuarios);
        }

        [Authorize(Roles = "Administrador")]
        public ActionResult BuscarUsuario(string email)
        {
            if(email == "")
            {
                TempData["mensaje"] = "Debe ingresar un email.";
               return RedirectToAction("UsersList", "Home");
            }
            else 
            {
                IEnumerable<ApplicationUser> usuario = (from u in db.Users
                                                        where u.Email == email
                                                        select u);
                if (usuario.Count() == 0)
                {
                    TempData["mensaje"] = "No existe usuario registrado con el email ingresado.";
                   return RedirectToAction("UsersList", "Home");
                }
                else
                {
                    return View(usuario);
                }
            }
            
        }
        
        [Authorize]
        public ActionResult DetailsUsers(string id)
        {
            var userId = User.Identity.GetUserId();
            TempData["usuarioId"] = userId;
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

        [Authorize(Roles = "Administrador")]
        public ActionResult AssignRoleAdministrador()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
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

        [Authorize(Roles = "Administrador")]
        public ActionResult AssignRoleCliente()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public ActionResult AssignRoleCliente(string id)
        {
            var user = db.Users.Find(id);

            if (user == null)
            {
                return HttpNotFound();

            }
            UserManager.AddToRole(user.Id, "Cliente");
            UserManager.RemoveFromRole(user.Id, "Administrador");
            user.RolString = "Cliente";
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