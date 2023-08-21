using FlexoCotizaciones.Models;
using System.Collections.ObjectModel;
namespace FlexoCotizaciones;

public partial class AppShell : Shell
{
    public static ObservableCollection<Cotizacion> CotizacionGlobal { get; set; }
    public static ObservableCollection<ItemCotizacion> ItemCotizacionGlobal { get; set; }
    public static ObservableCollection<Usuario> UsuariosGlobal { get; set; }
    public static ObservableCollection<Total> TotalGlobal { get; set; }
    public static ObservableCollection<Cliente> ClientesGlobal { get; set; }
    public static ObservableCollection<Papel> PapelesGlobal { get; set; }

    public AppShell()
	{
		InitializeComponent();

        CotizacionGlobal = new ObservableCollection<Cotizacion>();
        ItemCotizacionGlobal = new ObservableCollection<ItemCotizacion>();
        UsuariosGlobal = new ObservableCollection<Usuario>();
        TotalGlobal = new ObservableCollection<Total>();
        ClientesGlobal = new ObservableCollection<Cliente>();
        PapelesGlobal = new ObservableCollection<Papel>();

        BindingContext = this;

        Routing.RegisterRoute("AgregarITemCotizacion", typeof(AgregarITemCotizacion));
        Routing.RegisterRoute("Cotizaciones", typeof(Cotizaciones));
        cmdCerrarSesion.Clicked += CmdCerrarSesion_Clicked;
	}

    private async void CmdCerrarSesion_Clicked(object sender, EventArgs e)
    {
        App.rutConectado = "";
        App.InicialesConectado = "";
        await Shell.Current.GoToAsync($"//{nameof(PantallaInicio)}");


    }
}
