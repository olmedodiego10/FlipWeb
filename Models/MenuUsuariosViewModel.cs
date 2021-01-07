using FlipWeb.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FlipWeb.Models
{
    public class MenuUsuariosViewModel
    {
        public List<OfertaCarga> ListadoOfertasCarga { get; set; }
        public List<OfertaTransporte> ListadoOfertasTransporte { get; set; }

        [Display(Name ="Tipo de oferta")]
        public string TipoOferta { get; set;}
        

        [Display(Name = "Fecha desde")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime FechaDesde { get; set; }

        [Display(Name = "Fecha hasta")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime FechaHasta { get; set; }

        [Display(Name = "País de partida")]
        public string PaisPartida { get; set; }

        [StringLength(50, ErrorMessage = "Limite de caracteres excedido")]
        [Display(Name = "Ciudad de Partida")]
        public string CiudadPartida { get; set; }

        [Display(Name = "País de destino")]
        public string PaisDestino { get; set; }

        [Required(ErrorMessage = "El campo Ciudad de destino es obligatorio.")]
        [StringLength(50, ErrorMessage = "Limite de caracteres excedido")]
        [Display(Name = "Ciudad de destino")]
        public string CiudadDestino { get; set; }

        [StringLength(30, ErrorMessage = "Limite de caracteres excedido")]
        [Display(Name = "Tipo de camión")]
        public string TipoCamion { get; set; }

        [StringLength(30, ErrorMessage = "Limite de caracteres excedido")]
        [Display(Name = "Tipo de caja")]
        public string TipoCaja { get; set; }
    }
}