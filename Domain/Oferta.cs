using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlipWeb.Models;

namespace FlipWeb.Domain
{
    [Table("Ofertas")]
    public class Oferta
    {
        public int OfertaId { get; set; }
        public string OfertanteId { get; set; }
        public List<Contacto> ListaContactos { get; set; }
        //Estados: En Progreso, Finalizada, Reportada
        public string Estado { get; set; }
        [Required(ErrorMessage = "El campo Detalles es obligatorio.")]
        [StringLength(200, ErrorMessage = "Limite de caracteres excedido")]
        [DataType(DataType.MultilineText)]
        public string Detalles { get; set; }
        [Required(ErrorMessage = "El campo País de partida es obligatorio.")]
        [Display(Name = "País de partida")]
        public string PaisPartida { get; set; }
        [Required(ErrorMessage = "El campo Ciudad de Partida es obligatorio.")]
        [StringLength(50, ErrorMessage = "Limite de caracteres excedido")]
        [Display(Name = "Ciudad de Partida")]
        public string CiudadPartida { get; set; }
        [Required(ErrorMessage = "El campo Dirección de partida es obligatorio.")]
        [StringLength(50, ErrorMessage = "Limite de caracteres excedido")]
        [Display(Name = "Dirección de partida")]
        public string DireccionPartida { get; set; }
        [Required(ErrorMessage = "El campo País de destino es obligatorio.")]
        [Display(Name = "País de destino")]
        public string PaisDestino { get; set; }
        [Required(ErrorMessage = "El campo Ciudad de destino es obligatorio.")]
        [StringLength(50, ErrorMessage = "Limite de caracteres excedido")]
        [Display(Name = "Ciudad de destino")]
        public string CiudadDestino { get; set; }
        [Required(ErrorMessage = "El campo Dirección de destino es obligatorio.")]
        [StringLength(50, ErrorMessage = "Limite de caracteres excedido")]
        [Display(Name = "Dirección de destino")]
        public string DireccionDestino { get; set; }
        [Display(Name = "Fecha de oferta")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime FechaOferta { get; set; }
        public DateTime FechaCreacion { get; set; }
        [Display(Name = "Imagen ilustrativa")]
        public byte[] Imagen1 { get; set; }

        public bool OfertaFinalizada()
        {
            if (Estado == "Finalizada")
                return true;
            else
                return false;
        }
        public bool ofertaReportada()
        {
            if (Estado == "Reportada")
                return true;
            else
                return false;
        }

        public double ReputacionOfertante()
        {
            List<Oferta> Ofertas = new ApplicationDbContext().Ofertas.Include("ListaContactos").Where(o => o.OfertanteId == OfertanteId).ToList();
            List<Contacto> Contactos = Ofertas.SelectMany(O => O.ListaContactos).ToList();

            if (Contactos.Count == 0)
            {
                return 0.00;
            }
            else
            {
                return Math.Round(Contactos.Average(c => c.Calificacion), 2);
            }
        }

        public List<String> Ultimos5ComentariosDeContactosOfertante()
        {
            //Obtengo todas las ofertas del ofertante porque las últimas 5 pueden no tener contactos calificados
            List<Oferta> Ofertas = new ApplicationDbContext().Ofertas.Include("ListaContactos").Where(o => o.OfertanteId == OfertanteId).ToList();
            List<Contacto> Contactos = Ofertas.SelectMany(O => O.ListaContactos).ToList();
            int Last = Contactos.Count() - 1;
            int i = 1;
            List<String>  UltimosComentarios = new List<string>();
            while (i < 5 && Last >= 0)
            {
                if (Contactos[Last].Comentario != null)
                {
                    UltimosComentarios.Add(Contactos[Last].Comentario);
                    i++;
                    Last--;
                }
                else
                {
                    i++;
                    Last--;
                }
            }
            return UltimosComentarios;
        }

    }
}
