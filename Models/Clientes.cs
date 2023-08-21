using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace FlexoCotizaciones.Models;
public class Clientes
{
    [XmlElement("Cliente")]
    public List<Cliente> ListaClientes { get; set; }
}
