using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlipWeb.Domain
{
    [Table("Contactos")]
    public class Contacto
    {
        public int ContactoId { get; set; }
        //Estados: En Progreso, Cerrado, Reporte
        public string Estado { get; set; }
        [Display(Name = "Oferta nro.")]
        public int IdOfertaContactada { get; set; }
        public string IdContactante { get; set; }
        //[Range(1, 5)] se hace not null y no pasa el ModelState.IsValid al crearse
        [Display(Name = "Calificación")]
        public int Calificacion { get; set; }
        public string Comentario { get; set; }
        [Display(Name = "Fecha de contacto")]
        public DateTime FechaContacto { get; set; }
    }
}
