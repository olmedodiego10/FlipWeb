using FlipWeb.Domain;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FlipWeb.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }
    }

    public class SendCodeViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
        public string ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
    }

    public class VerifyCodeViewModel
    {
        [Required]
        public string Provider { get; set; }

        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }
        public string ReturnUrl { get; set; }

        [Display(Name = "Remember this browser?")]
        public bool RememberBrowser { get; set; }

        public bool RememberMe { get; set; }
    }

    public class ForgotViewModel
    {
        [Required]
        [Display(Name = "Correo electrónico")]
        public string Email { get; set; }
    }

    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Correo electrónico")]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }

        [Display(Name = "Recordar sesión")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Correo electrónico")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Su contraseña debe tener al menos {2} caracteres de largo.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmar contraseña")]
        [Compare("Password", ErrorMessage = "Las contraseñas no coinciden.")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Campo Nombre requerido")]
        [StringLength(30, ErrorMessage = "Limite de caracteres excedido")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "Campo Apellido requerido")]
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
    }

    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}
