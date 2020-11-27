using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlipWeb.Domain
{
    [Table("OfertasCarga")]
    public class OfertaCarga : Oferta
    {
        [Required(ErrorMessage = "Campo requerido")]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Descripción de Mercadería")]
        public string DescripcionMercaderia { get; set; }
        [Display(Name = "¿Requiere exclusividad?")]
        public bool RequiereExclusividad { get; set; }
    }
}
