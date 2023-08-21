using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlexoCotizaciones.Models;

public class ItemCotizacion
{
    public double Id { get; set; }
    public string Descripcion { get; set; }
    public double Cantidad { get; set; }
    public string UM { get; set; }
    public double Valor { get; set; }
    public double Total { get; set; }
   
}
