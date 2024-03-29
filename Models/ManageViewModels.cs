﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using FlipWeb.Domain;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;

namespace FlipWeb.Models
{
    public class IndexViewModel
    {
        public bool HasPassword { get; set; }
        public IList<UserLoginInfo> Logins { get; set; }
        public string PhoneNumber { get; set; }
        public bool TwoFactor { get; set; }
        public bool BrowserRemembered { get; set; }
        [Required(ErrorMessage = "El campo Correo electrónico es obligatorio")]
        [EmailAddress]
        [Display(Name = "Correo electrónico")]
        public string Email { get; set; }
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
        public List<Contacto> ListaContactos { get; set; }

        public double CalcularReputacion()
        {
            if(ListaContactos.Count == 0)
            {
                return 0.00;
            }
            double Rep = ListaContactos.Average(c => c.Calificacion);
            return Math.Round(Rep, 2);
        }

        public List<String> Ultimos5ComentariosDeContactosOfertante()
        {

            int Last = ListaContactos.Count() - 1;
            int i = 1;
            List<String> Ultimos5Comentarios = new List<string>();
            while (i < 5 && Last >= 0)
            {
                if (ListaContactos[Last].Comentario != null)
                {
                    Ultimos5Comentarios.Add(ListaContactos[Last].Comentario);
                    i++;
                    Last--;
                }
                else
                {
                    i++;
                    Last--;
                }
            }
            return Ultimos5Comentarios;
        }

    }

    public class ManageLoginsViewModel
    {
        public IList<UserLoginInfo> CurrentLogins { get; set; }
        public IList<AuthenticationDescription> OtherLogins { get; set; }
    }

    public class FactorViewModel
    {
        public string Purpose { get; set; }
    }

    public class SetPasswordViewModel
    {
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "La nueva contraseña y su confirmación no coinciden")]
        public string ConfirmPassword { get; set; }
    }

    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "Campo obligatorio")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña actual")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "Campo obligatorio")]
        [StringLength(100, ErrorMessage = "Su nueva contraseña debe tener al menos {2} caracteres de largo.", MinimumLength = 6)]
        [RegularExpression(@"(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[a-zA-Z]).{6,}$", ErrorMessage = "Su nueva contraseña debe tener al menos 6 caracteres incluido una mayúscula y un número.")]
        [DataType(DataType.Password)]
        [Display(Name = "Nueva contraseña")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmar nueva contraseña")]
        [Compare("NewPassword", ErrorMessage = "La nueva contraseña y la contraseña de confirmación no coinciden.")]
        public string ConfirmPassword { get; set; }
    }

    public class AddPhoneNumberViewModel
    {
        [Required]
        [Phone]
        [Display(Name = "Phone Number")]
        public string Number { get; set; }
    }

    public class VerifyPhoneNumberViewModel
    {
        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }

        [Required]
        [Phone]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }
    }

    public class ConfigureTwoFactorViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
    }
}