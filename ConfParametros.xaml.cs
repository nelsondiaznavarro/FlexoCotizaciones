using FlexoCotizaciones.Models;
using System.Text;
using System.Xml.Serialization;

namespace FlexoCotizaciones;

public partial class ConfParametros : ContentPage
{
    private const string url = "http://dataservice.flexografica.cl/validausuario.php";
    private const string url_parametros = "http://dataservice.flexografica.cl/parametros.php";
    private HttpClient _Client = new HttpClient();
    public ConfParametros()
	{
        
        InitializeComponent();
        TraeParametros();

        cmdGuardarParametros.Clicked += (sender, args) =>
        {
            ActualizaParametros();
        };

    }

    public async void TraeParametros()
    {
        string resultado = "";

        var httpResponse = await _Client.GetAsync(url_parametros + "?accion=listar");
        if (httpResponse.IsSuccessStatusCode)
        {
            var responseData = httpResponse.Content.ReadAsStringAsync();
            resultado = responseData.Result;
            resultado = "<?xml version='1.0'?>" + resultado;
        }
        CalculoParametros MisParametros = new CalculoParametros();

        XmlSerializer Serializador = new XmlSerializer(MisParametros.GetType());

        byte[] byteArray = Encoding.ASCII.GetBytes(resultado);
        MemoryStream stream = new MemoryStream(byteArray);

        var MisParametrosDes = (CalculoParametros)Serializador.Deserialize(stream);

        txtMetrosLineal.Text = MisParametrosDes.MetrosLineal.ToString();
        txtMetrosHora.Text = MisParametrosDes.MetrosHora.ToString();
        txtClisse.Text = MisParametrosDes.Clisse.ToString();
        txtHorasMaquina.Text = MisParametrosDes.HorasMaquina.ToString();
        txtCalce.Text = MisParametrosDes.Calce.ToString();
        txtBarniz.Text = MisParametrosDes.Barniz.ToString();
        txtLamina.Text = MisParametrosDes.lamina.ToString();
        txtColores.Text = MisParametrosDes.Colores.ToString();
        txtGanancia.Text =  MisParametrosDes.Ganancia.ToString();
        txtComisionDefecto.Text = MisParametrosDes.ComisionDefecto.ToString();
    }

    public async void ActualizaParametros()
    {
       
        string proceso = url_parametros + "?accion=actualizarparametros&MetrosLineal=" + txtMetrosLineal.Text + "&MetrosHora=" + txtMetrosHora.Text + "&Clisse=" + txtClisse.Text + "&HorasMaquina=" + txtHorasMaquina.Text + "&Calce=" + txtCalce.Text + "&Barniz=" + txtBarniz.Text + "&Lamina=" + txtLamina.Text + "&Colores=" + txtColores.Text + "&Ganancia=" + txtGanancia.Text;
        var httpResponseAct = await _Client.GetAsync(url_parametros + "?accion=actualizarparametros&MetrosLineal=" + txtMetrosLineal.Text + "&MetrosHora=" + txtMetrosHora.Text + "&Clisse=" + txtClisse.Text + "&HorasMaquina=" + txtHorasMaquina.Text + "&Calce=" + txtCalce.Text + "&Barniz=" + txtBarniz.Text + "&Lamina=" + txtLamina.Text + "&Colores=" + txtColores.Text + "&Ganancia=" + txtGanancia.Text + "&ComisionDefecto=" + txtComisionDefecto.Text);

        /*  App.ParametrosRepo.AddNewParametro(newParametro.Text, newValorPorcentaje.Text, newValorDesde.Text, newValorHasta.Text,pickerTipo.SelectedIndex != -1 ? pickerTipo.SelectedIndex.ToString()  : "0");*/
        await DisplayAlert("Parametros", "Parametros actualziados correctamente", "Aceptar");
        TraeParametros();
    }
}