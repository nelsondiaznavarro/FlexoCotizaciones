using Mopups.Services;
using FlexoCotizaciones.Models;

using System.Collections.ObjectModel;

namespace FlexoCotizaciones;

public partial class ParametrosConf : ContentPage
{

    private const string url = "http://dataservice.flexografica.cl/databaseconnect.php";
    private HttpClient _Client = new HttpClient();

    public ParametrosConf()
	{
        InitializeComponent();
        cmdAgregar.Clicked += CmdAgregar_Clicked;
        //App.ParametrosRepo.deleteAll();
        //List<Parametros> lista = TraeParametros();
        //ListaParametros.ItemsSource = lista;
        cmdRefrescar.Clicked +=  (sender, args) =>
        {

            List<Parametros> lista = TraeParametros();
            ListaParametros.ItemsSource = lista;
            //var httpResponse = await _Client.GetAsync(url);
            //if (httpResponse.IsSuccessStatusCode)
            //{


            //    var responseData = httpResponse.Content.ReadAsStringAsync();
            //    string resultado = responseData.Result;

            //}

        };
    }

    private async Task CmdRefrescar_Clicked(object sender, EventArgs e)
    {


        var httpResponse = await _Client.GetAsync(url);
        if (httpResponse.IsSuccessStatusCode)
        {
           await  DisplayAlert("Lectura OK", "Datos", "Aceptar");

            //string responseData = httpResponse.Content.ReadAsStringAsync();

        }

        List<Parametros> lista = TraeParametros();
        ListaParametros.ItemsSource = lista;
    }


    private async Task Descarga_Parametros(object sender, EventArgs e)
    {
        var httpResponse = await _Client.GetAsync(url);

        if (httpResponse.IsSuccessStatusCode)
        {
            await DisplayAlert("Lectura OK", "Datos", "Aceptar");

            //string responseData = httpResponse.Content.ReadAsStringAsync();
            
        }
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