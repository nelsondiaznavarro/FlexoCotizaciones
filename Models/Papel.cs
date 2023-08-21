using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlexoCotizaciones.Models;

public class Papel
{
    public int Id { get; set; }
    public int Codigo { get; set; }
    public string NombrePapel { get; set; }
    public double Valor { get; set; }

    public string IdNombre { get; set; }


}
