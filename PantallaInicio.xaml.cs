using System.Text;
using System.Xml.Serialization;
using FlexoCotizaciones.Models;
using static System.Net.Mime.MediaTypeNames;

namespace FlexoCotizaciones;


public partial class PantallaInicio : ContentPage
{

    private const string url = "http://dataservice.flexografica.cl/validausuario.php";
    private HttpClient _Client = new HttpClient();
    public static string UsuarioGlobal = "";
    public PantallaInicio()
    {
        InitializeComponent();
       



    }

    private async void OnButton_Clicked(object sender, EventArgs e)
    {



        string resultado = "";

        if (!string.IsNullOrEmpty(rutusuario.Text))
        {
            var httpResponse = await _Client.GetAsync(url + "?accion=validar&rutusuario=" + rutusuario.Text);
            if (httpResponse.IsSuccessStatusCode)
            {
                var responseData = httpResponse.Content.ReadAsStringAsync();
                resultado = responseData.Result;
                resultado = "<?xml version='1.0'?>" + resultado;
            }
            Usuario MiUsuario= new Usuario();

            XmlSerializer Serializador = new XmlSerializer(MiUsuario.GetType());


            
            byte[] byteArray = Encoding.ASCII.GetBytes(resultado);
            MemoryStream stream = new MemoryStream(byteArray);


            var MisUsuariosDes = (Usuario)Serializador.Deserialize(stream);

            if (MisUsuariosDes.Id == 0)
            {
                await DisplayAlert("Error de Usuario", "Rut no tiene acceso a la aplicación", "Aceptar");
            }
            else
            {
                if (!string.IsNullOrEmpty(MisUsuariosDes.iniciales))
                {
                    await DisplayAlert("Ingreso", "Bienevenido " + MisUsuariosDes.iniciales, "Ingresar");
                    App.rutConectado = MisUsuariosDes.rut;
                    App.InicialesConectado = MisUsuariosDes.iniciales;
                    AppShell.Current.FlyoutHeader = new FlyoutHeaderBehavior();
                    App.rutConectado = MisUsuariosDes.rut;
                    App.InicialesConectado = MisUsuariosDes.iniciales;
                    await Shell.Current.GoToAsync($"//{nameof(ConfParametros)}");
                }
                else
                {
                    string result = await DisplayPromptAsync("Ingreso Sistema", "Coloque sus iniaciales");
                    var httpResponseAct = await _Client.GetAsync(url + "?accion=actualizar&rutusuario=" + rutusuario.Text + "&iniciales=" + result);
                    App.rutConectado = MisUsuariosDes.rut;
                    App.InicialesConectado = MisUsuariosDes.iniciales;
                    await Shell.Current.GoToAsync($"//{nameof(ConfParametros)}");
                }

            }


        }
        else
        {
            await DisplayAlert("Ingreso", "Debe ingresar su RUT", "Aceptar");
        }

        



        //vamos a pedir las iniciales





        

        
    }


}
