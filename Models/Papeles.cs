using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace FlexoCotizaciones.Models;

public class Papeles
{
    [XmlElement("Papel")]
    public List<Papel> ListaPapeles { get; set; }
}
