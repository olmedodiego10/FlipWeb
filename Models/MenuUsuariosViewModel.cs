using FlipWeb.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FlipWeb.Models
{
    public class MenuUsuariosViewModel
    {
        public List<OfertaCarga> ListadoOfertasCarga { get; set; }
        public List<OfertaTransporte> ListadoOfertasTransporte { get; set; }
    }
}