using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlipWeb.Domain
{
    [Table("OfertasTransporte")]
    public class OfertaTransporte : Oferta
    {
        [Required(ErrorMessage = "El campo Medidas de caja de camión es obligatorio.")]
        [StringLength(30, ErrorMessage = "Limite de caracteres excedido")]
        [Display(Name = "Medidas de caja de camión")]
        public string MedidasCaja { get; set; }
        [Required(ErrorMessage = "El campo Tipo de caja es obligatorio.")]
        [StringLength(30, ErrorMessage = "Limite de caracteres excedido")]
        [Display(Name = "Tipo de caja")]
        public string TipoCaja { get; set; }
        [Required(ErrorMessage = "El campo Tipo de camión es obligatorio.")]
        [StringLength(30, ErrorMessage = "Limite de caracteres excedido")]
        [Display(Name = "Tipo de camión")]
        public string TipoCamion { get; set; }
        [Required(ErrorMessage = "El campo ITV es obligatorio.")]
        [StringLength(30, ErrorMessage = "Limite de caracteres excedido")]
        [Display(Name = "La inspección técnica del vehículo (ITV)")]
        public string ITV { get; set; }
        [Required(ErrorMessage = "El campo Habilitación bromatológica es obligatorio.")]
        [StringLength(30, ErrorMessage = "Limite de caracteres excedido")]
        [Display(Name = "Habilitación bromatológica")]
        public string HabilitacionBromatologica { get; set; }
        [Required(ErrorMessage = "Campo requerido")]
        public decimal Costo { get; set; }
    }
}
