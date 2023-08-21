using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace FlexoCotizaciones.Models;


public class Cliente
{
    public int Id { get; set; }
    public string RUT { get; set; }
    public string RazonSocial { get; set; }
    public string Direccion { get; set; }
    public string Comuna { get; set; }
    public string Ciudad { get; set; }
    public string Contacto { get; set; }
    public string Telefono { get; set; }
}
