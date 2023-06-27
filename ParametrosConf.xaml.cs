using Mopups.Services;
using FlexoCotizaciones.Models;
namespace FlexoCotizaciones;

public partial class ParametrosConf : ContentPage
{
	public ParametrosConf()
	{
        InitializeComponent();
        cmdAgregar.Clicked += CmdAgregar_Clicked;
        //App.ParametrosRepo.deleteAll();
        //List<Parametros> lista = TraeParametros();
        //ListaParametros.ItemsSource = lista;
        cmdRefrescar.Clicked += CmdRefrescar_Clicked;
    }

    private void CmdRefrescar_Clicked(object sender, EventArgs e)
    {
        List<Parametros> lista = TraeParametros();
        ListaParametros.ItemsSource = lista;
    }

    private void CmdAgregar_Clicked(object sender, EventArgs e)
    {
        MopupService.Instance.PushAsync(new ParametrosPopup());
    }

    public List<Parametros> TraeParametros()
    {
        List<Parametros> parametros = App.ParametrosRepo.GetAllPeople();
        return parametros;
    }
}