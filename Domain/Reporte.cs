using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FlipWeb.Domain
{
    [Table("Reportes")]
    public class Reporte
    {
        public int ReporteId { get; set; }

        public int ofertaDenunciadaId { get; set; }

        public string Motivo { get; set; }

        public string Detalle { get; set; }

        public string DeuncianteId { get; set; }

        public string CorreoDenunciante { get; set; }

        public string CorreoDenunciado { get; set; }

        public string DenunciadoId { get; set; }

        public string Estado { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime Fecha { get; set; }

       // [Required(ErrorMessage = "Debe ingresar la resolución del reporte antes de cerrarlo")]
        public string Resolucion { get; set; }

        public bool ReporteAbierto()
        {
            if (Estado == "Abierto")
                return true;
            else
                return false;
        }

        public bool ReporteCerrado()
        {
            if (Estado == "Cerrado")
                return true;
            else
                return false;
        }
    }
    
}