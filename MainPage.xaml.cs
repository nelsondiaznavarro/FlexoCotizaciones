using Mopups.Services;
using FlexoCotizaciones.Models;
namespace FlexoCotizaciones;

public partial class MainPage : ContentPage
{
	int count = 0;

	public MainPage()
	{
		InitializeComponent();
        cmdAgregar.Clicked += CmdAgregar_Clicked;
        cmdListar.Clicked += CmdListar_Clicked  ;
	}

    private void CmdListar_Clicked(object sender, EventArgs e)
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

