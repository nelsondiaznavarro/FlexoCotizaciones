using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlexoCotizaciones.Models;

public class Troquel
{
    public int Id { get; set; }

    public int InternalCode { get; set; }
    public string size { get; set; }
    public int axis { get; set; }
    public int development { get; set; }
    public int cylinderZ { get; set; }

    public string dieType { get; set; }
    public string dieForm { get; set; }

    public string dato { get; set; }

}
