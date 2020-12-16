using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
