using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlexoCotizaciones.Models;

public  class Cotizacion
{
    public int Id { get; set; }
    public string  Folio { get; set; }
    public string Fecha { get; set; }
    public int idCLiente { get; set; }
    public string FechaVencimiento { get; set; }
    public string  Usuario { get; set; }
    public int Estado { get; set; }
    public string  CondicionPago { get; set; }
}
