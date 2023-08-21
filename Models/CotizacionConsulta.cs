using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlexoCotizaciones.Models;

public class CotizacionConsulta
{
    public int Id { get; set; }
    public int Folio { get; set; }
    public string Fecha { get; set; }
    public int IdCliente { get; set; }
    public string FechaVencimiento { get; set; }
    public int Usuario { get; set; }
    public int Estado { get; set; }
    public string CondicionPago { get; set; }
    public double  Neto { get; set; }
    public double Iva { get; set; }
    public double Total { get; set; }
    public double PorcComision { get; set; }
    public double ValorComision { get; set; }
    public int Id_Detalle { get; set; }
    public string Descripcion { get; set; }
    public string UM { get; set; }
    public double Cantidad { get; set; }
    public double ValorUnitario { get; set; }
    public double ValorTotal { get; set; }
    public int id_Cotizaciones { get; set; }
    public string RUT { get; set; }
    public string RazonSocial { get; set; }




}
