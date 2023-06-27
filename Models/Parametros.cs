using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace FlexoCotizaciones.Models;

[Table("parametros")]
public class Parametros
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public long Valordesde { get; set; }
    public long ValorHasta { get; set; }

    [MaxLength(200), Unique]
    public string Nombre { get; set; }
    public long Precio { get; set; }
    public int Tipo { get; set; }

}

