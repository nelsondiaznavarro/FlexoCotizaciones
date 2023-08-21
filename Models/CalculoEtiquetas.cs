using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlexoCotizaciones.Models;

public class CalculoEtiquetas
{

    public int Id { get; set; }
    public int Cantidad { get; set; }
    public int Clisse { get; set; }
    public int laminas { get; set; }
    public int Colores { get; set; }
    public int CodMaterial { get; set; }
    public int Barniz { get; set; }
    public int Bandas { get; set; }
    public int ValorM2 { get; set; }
    public int AnchoPapel { get; set; }
    public int AltoEti { get; set; }
    public int AnchoEti { get; set; }
    public int EtixRollo { get; set; }
    public int GapAncho { get; set; }
    public double EtiquetaCosto { get; set; }
    public double XRollo { get; set; }
    public double CostoMasTroquel { get; set; }
    public double ValorVentaEtiqueta { get; set; }
    public double ValorVentaxRollo { get; set; }
    public double Troquel { get; set; }
    public int ComisionVendedorPorc { get; set; }
    public int ComisionVendedorMonto { get; set; }
    public int VentaGanancia { get; set; }
    public int VentaCliente { get; set; }
    public int GananciaNeta { get; set; }




}
