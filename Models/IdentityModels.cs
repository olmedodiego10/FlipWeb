using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using FlipWeb.Domain;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace FlipWeb.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {

        [Required(ErrorMessage = "Campo Nombre requerido")]
        [StringLength(30, ErrorMessage = "Limite de caracteres excedido")]
        public string Nombre { get; set; }
        [Required(ErrorMessage = "Campo apellido requerido")]
        [StringLength(30, ErrorMessage = "Limite de caracteres excedido")]
        public string Apellido { get; set; }
        [Required(ErrorMessage = "Campo Cédula requerido")]
        [StringLength(8, MinimumLength = 8, ErrorMessage = "Cédula completa sin puntos ni guiones")]
        [Display(Name = "Cédula")]
        public string Cedula { get; set; }
        [Required(ErrorMessage = "Campo Celular requerido")]
        [StringLength(20, ErrorMessage = "Limite de caracteres excedido")]
        public string Celular { get; set; }
        [Required(ErrorMessage = "Campo Teléfono requerido")]
        [StringLength(20, ErrorMessage = "Limite de caracteres excedido")]
        [Display(Name = "Teléfono")]
        public string Telefono { get; set; }
        public List<OfertaTransporte> ListaOfertasTransporteCreadas { get; set; }
        public List<OfertaCarga> ListaOfertasCargaCreadas { get; set; }
        public List<Contacto> ListaContactados { get; set; }
        public bool Premium { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Oferta> Ofertas { get; set; }
        public DbSet<OfertaCarga> OfertasCarga { get; set; }
        public DbSet<OfertaTransporte> OfertasTransporte { get; set; }
        public DbSet<Contacto> Contactos { get; set; }
        //public DbSet<Image> Images { get; set; }

        public ApplicationDbContext()
            : base("FlipConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}