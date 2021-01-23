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

        [Required(ErrorMessage = "El campo Nombre es obligatorio")]
        [StringLength(30, ErrorMessage = "Limite de caracteres excedido")]
        public string Nombre { get; set; }
        [Required(ErrorMessage = "El campo Apellido es obligatorio")]
        [StringLength(30, ErrorMessage = "Límite de caracteres excedido")]
        public string Apellido { get; set; }
        [Required(ErrorMessage = "El campo Cédula es obligatorio")]
        [StringLength(8, MinimumLength = 8, ErrorMessage = "Cédula completa sin puntos ni guiones")]
        [Display(Name = "Cédula")]
        public string Cedula { get; set; }
        [Required(ErrorMessage = "El campo Celular es obligatorio")]
        [StringLength(20, ErrorMessage = "Límite de caracteres excedido")]
        public string Celular { get; set; }
        [Required(ErrorMessage = "El campo Teléfono es obligatorio")]
        [StringLength(20, ErrorMessage = "Límite de caracteres excedido")]
        [Display(Name = "Teléfono")]
        public string Telefono { get; set; }

        public string RolString { get; set; }
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
        public DbSet<Reporte> Reportes { get; set; }

        public class FliplDBInitializer : CreateDatabaseIfNotExists<ApplicationDbContext>
        {
            protected override void Seed(ApplicationDbContext context)
            {
                var roleStore = new RoleStore<IdentityRole>(context);
                var roleManager = new RoleManager<IdentityRole>(roleStore);
                var userStore = new UserStore<ApplicationUser>(context);
                var userManager = new UserManager<ApplicationUser>(userStore);
                roleManager.Create(new IdentityRole("Administrador"));
                roleManager.Create(new IdentityRole("Cliente"));

                //Admin Diego
                ApplicationUser Admin1 = new ApplicationUser { Email = "olmedodiego10@gmail.com", Nombre = "Diego", Apellido = "Olmedo", Cedula = "45296002", Celular = "099593771", RolString = "Administrador", Telefono = "22279539", Premium = true, UserName = "olmedodiego10@gmail.com" };
                userManager.Create(Admin1, "Diego10111993");
                userManager.AddToRole(Admin1.Id, "Administrador");

                //Admin Adrián
                ApplicationUser Admin2 = new ApplicationUser { Email = "adrianrodriguez0510@gmail.com", Nombre = "Adrián", Apellido = "Rodríguez", Cedula = "11111111", Celular = "099111111", RolString = "Administrador", Telefono = "11111111", Premium = true, UserName = "adrianrodriguez0510@gmail.com" };
                userManager.Create(Admin2, "Adrian-1899");
                userManager.AddToRole(Admin2.Id, "Administrador");

                //Cliente Diego
                ApplicationUser Cliente1 = new ApplicationUser { Email = "olmedodiegocuentas@gmail.com", Nombre = "Diego", Apellido = "Olmedo", Cedula = "45296002", Celular = "099593771", RolString = "Cliente", Telefono = "22279539", Premium = true, UserName = "olmedodiegocuentas@gmail.com" };
                userManager.Create(Cliente1, "Diego10111993");
                userManager.AddToRole(Cliente1.Id, "Cliente");

                //Cliente Adrián
                ApplicationUser Cliente2 = new ApplicationUser { Email = "yiyo0510@hotmail.com", Nombre = "Adrián", Apellido = "Rodríguez", Cedula = "11111111", Celular = "099111111", RolString = "Cliente", Telefono = "099111111", Premium = true, UserName = "yiyo0510@hotmail.com" };
                userManager.Create(Cliente2, "Adrian-1899");
                userManager.AddToRole(Cliente2.Id, "Cliente");

                base.Seed(context);
            }
        }

        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
            Database.SetInitializer(new FliplDBInitializer());
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}