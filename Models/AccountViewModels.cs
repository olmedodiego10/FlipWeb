using FlipWeb.Domain;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FlipWeb.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "Correo electrónico")]
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
        [Display(Name = "Código")]
        public string Code { get; set; }
        public string ReturnUrl { get; set; }

        [Display(Name = "Recordar sesión")]
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
        [Required(ErrorMessage = "El campo Correo electrónico es obligatorio")]
        [EmailAddress]
        [Display(Name = "Correo electrónico")]
        public string Email { get; set; }

        [Required(ErrorMessage = "El campo Contraseña es obligatorio")]
        [StringLength(100, ErrorMessage = "Su contraseña debe tener al menos {2} caracteres de largo.", MinimumLength = 6)]
        [RegularExpression(@"(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[a-zA-Z]).{6,}$", ErrorMessage = "Su contraseña debe tener al menos 6 caracteres incluido una mayúscula y un número.")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmar contraseña")]
        [Compare("Password", ErrorMessage = "La contraseña y la contraseña de confirmación no coinciden.")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "El campo Nombre es obligatorio")]
        [StringLength(30, ErrorMessage = "Límite de caracteres excedido")]
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

        public List<OfertaTransporte> ListaOfertasTransporteCreadas { get; set; }
        public List<OfertaCarga> ListaOfertasCargaCreadas { get; set; }
        public List<Contacto> ListaContactados { get; set; }
        public bool Premium { get; set; }
    }

    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Correo electrónico")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Su contraseña debe tener al menos {2} caracteres de largo.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmar contraseña")]
        [Compare("Password", ErrorMessage = "La contraseña y la contraseña de confirmación no coinciden.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Correo electrónico")]
        public string Email { get; set; }
    }
}
