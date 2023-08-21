using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace FlexoCotizaciones.Models;

public class Totales
{
  
    public double Subtotal { get; set; }
    public double Iva { get; set; }
    public double  Total { get; set; }
}
