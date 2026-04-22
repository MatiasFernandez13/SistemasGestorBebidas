using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE
{
    public class Venta
    {
        public int UsuarioId { get; set; }
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public List<VentaDetalle> Detalles { get; set; } = new List<VentaDetalle>();
        public decimal Total => Detalles.Sum(d => d.Subtotal);
    }

}
