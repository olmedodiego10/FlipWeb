using FlipWeb.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FlipWeb.Models
{
    public class ReporteViewModel
    {
        public List<Reporte> ListadoReportesAbiertos { get; set; }
        public List<Reporte> ListadoReportesCerrados { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime Fecha { get; set; }
    }
}