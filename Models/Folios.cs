using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace FlexoCotizaciones.Models;

public class Folios
{
    [XmlElement("Folios")]
    public int Id { get; set; }
    public int Folio { get; set; }
}
