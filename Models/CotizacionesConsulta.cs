using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace FlexoCotizaciones.Models;

public class CotizacionesConsulta
{
    [XmlElement("CotizacionConsulta")]
    public List<CotizacionConsulta> ListaCotizaciones { get; set; }
}
