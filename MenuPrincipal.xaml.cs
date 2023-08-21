using Mopups.Services;
using FlexoCotizaciones.Models;
using System.Text;
using System.Xml.Serialization;


using Microsoft.Maui.LifecycleEvents;

namespace FlexoCotizaciones;

public partial class MenuPrincipal : TabbedPage
{
    private const string url = "http://dataservice.flexografica.cl/validausuario.php";
    private const string url_parametros = "http://dataservice.flexografica.cl/parametros.php";
    private HttpClient _Client = new HttpClient();

    Usuario UsuarioSeleccionado = null;

    public MenuPrincipal()
	{
		InitializeComponent();

        TraeParametros();
        ListarUsuarios();

        cmdAgregarUsuario.Clicked += cmdAgregarUsuario_Clicked;

        cmdEliminarUsuario.Clicked += CmdEliminarUsuario_Clicked;

        cmdGuardarParametros.Clicked += (sender, args) =>
        {
            ActualizaParametros();
        };

            cmdListarUsuarios.Clicked += async (sender, args) =>
        {
            string resultado = "";

            var httpResponse = await _Client.GetAsync(url + "?accion=listar");
            if (httpResponse.IsSuccessStatusCode)
            {
                var responseData = httpResponse.Content.ReadAsStringAsync();
                resultado = responseData.Result;
                resultado = "<?xml version='1.0'?>" + resultado;
            }
            Usuarios MisUsuarios = new Usuarios();

            XmlSerializer Serializador = new XmlSerializer(MisUsuarios.GetType());

            byte[] byteArray = Encoding.ASCII.GetBytes(resultado);
            MemoryStream stream = new MemoryStream(byteArray);

            var MisUsuariosDes = (Usuarios)Serializador.Deserialize(stream);

            ListaUsuariosFront.ItemsSource = MisUsuariosDes.ListaUsuarios;
            };

        


    }

    private async void CmdEliminarUsuario_Clicked(object sender, EventArgs e)
    {
        if (UsuarioSeleccionado != null)
        {
            bool OpcionSleeccionada = await DisplayAlert("Usuarios", "Desea eliminar el usuario (" + UsuarioSeleccionado.rut.ToString() + ")", "Si", "No");
            if (OpcionSleeccionada)
            {
                int idusuario = UsuarioSeleccionado.Id;
                string resultado = "";

                var httpResponse = await _Client.GetAsync(url_parametros + "?accion=eliminarusuario&IdUsuario=" + idusuario.ToString());
                if (httpResponse.IsSuccessStatusCode)
                {
                    var responseData = httpResponse.Content.ReadAsStringAsync();
                    resultado = responseData.Result;
                    resultado = "<?xml version='1.0'?>" + resultado;
                }
                UsuarioSeleccionado = null;
                ListarUsuarios();
            }
            else
            { 
            
            }
           
           
        }
        else
        {
           await DisplayAlert("Usuarios", "Debe seleccionar un usuario para eliminar", "Aceptar");
        }
    }

    private async void ListarUsuarios()
    {
        ListaUsuariosFront.ItemsSource = null;
        string resultado = "";

        var httpResponse = await _Client.GetAsync(url + "?accion=listar");
        if (httpResponse.IsSuccessStatusCode)
        {
            var responseData = httpResponse.Content.ReadAsStringAsync();
            resultado = responseData.Result;
            resultado = "<?xml version='1.0'?>" + resultado;
        }
        Usuarios MisUsuarios = new Usuarios();

        XmlSerializer Serializador = new XmlSerializer(MisUsuarios.GetType());

        byte[] byteArray = Encoding.ASCII.GetBytes(resultado);
        MemoryStream stream = new MemoryStream(byteArray);

        var MisUsuariosDes = (Usuarios)Serializador.Deserialize(stream);

        ListaUsuariosFront.ItemsSource = MisUsuariosDes.ListaUsuarios;
    }

    private void cmdAgregarUsuario_Clicked(object sender, EventArgs e)
    {
        MopupService.Instance.PushAsync(new UsuariosPopup());
        ListarUsuarios();
    }

    private void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        UsuarioSeleccionado = null;
        UsuarioSeleccionado = (Usuario)e.SelectedItem;

        //DisplayAlert("Item seleccionado", UsuarioSeleccionado.Id.ToString(), "Aceptar");
        
    }
    

    public async void ActualizaParametros()
    {
        //if (string.IsNullOrEmpty(txtRutUsuario.Text))
        //{
        //    await DisplayAlert("Usuarios", "Debe ingresar RUT Usuario", "Aceptar");
        //    txtRutUsuario.Focus();
        //    return;
        //}
        string proceso = url_parametros + "?accion=actualizarparametros&MetrosLineal="+ txtMetrosLineal.Text + "&MetrosHora="+ txtMetrosHora.Text + "&Clisse="+ txtClisse.Text + "&HorasMaquina="+ txtHorasMaquina.Text + "&Calce="+ txtCalce.Text + "&Barniz="+ txtBarniz.Text + "&Lamina="+ txtLamina.Text + "&Colores="+ txtColores.Text;
        var httpResponseAct = await _Client.GetAsync(url_parametros + "?accion=actualizarparametros&MetrosLineal="+ txtMetrosLineal.Text + "&MetrosHora="+ txtMetrosHora.Text + "&Clisse="+ txtClisse.Text + "&HorasMaquina="+ txtHorasMaquina.Text + "&Calce="+ txtCalce.Text + "&Barniz="+ txtBarniz.Text + "&Lamina="+ txtLamina.Text + "&Colores="+ txtColores.Text );

        /*  App.ParametrosRepo.AddNewParametro(newParametro.Text, newValorPorcentaje.Text, newValorDesde.Text, newValorHasta.Text,pickerTipo.SelectedIndex != -1 ? pickerTipo.SelectedIndex.ToString()  : "0");*/
        await DisplayAlert("Parametros", "Parametros actualziados correctamente", "Aceptar");
        TraeParametros();
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

        txtMetrosLineal.Text =  MisParametrosDes.MetrosLineal.ToString();
        txtMetrosHora.Text = MisParametrosDes.MetrosHora.ToString();
        txtClisse.Text = MisParametrosDes.Clisse.ToString();
        txtHorasMaquina.Text = MisParametrosDes.HorasMaquina.ToString();
        txtCalce.Text = MisParametrosDes.Calce.ToString();
        txtBarniz.Text = MisParametrosDes.Barniz.ToString();
        txtLamina.Text = MisParametrosDes.lamina.ToString();
        txtColores.Text = MisParametrosDes.Colores.ToString();
    }
}