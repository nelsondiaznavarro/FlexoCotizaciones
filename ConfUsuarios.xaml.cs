using Mopups.Services;
using FlexoCotizaciones.Models;
using System.Text;
using System.Xml.Serialization;

namespace FlexoCotizaciones;

public partial class ConfUsuarios : ContentPage
{
    private const string url = "http://dataservice.flexografica.cl/validausuario.php";
    private const string url_parametros = "http://dataservice.flexografica.cl/parametros.php";
    private HttpClient _Client = new HttpClient();

    Usuario UsuarioSeleccionado = null;
    public ConfUsuarios()
	{
		InitializeComponent();
        //ListarUsuarios();
        ListaUsuariosFront.ItemsSource = AppShell.UsuariosGlobal;
        cargar_usuarios();
        cmdAgregarUsuario.Clicked += cmdAgregarUsuario_Clicked;

        cmdEliminarUsuario.Clicked += CmdEliminarUsuario_Clicked;
    }

    private async void cargar_usuarios()
    {
        string resultado = "";
        AppShell.UsuariosGlobal.Clear();
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

        foreach (Usuario MiUsu in MisUsuariosDes.ListaUsuarios)
        {
            AppShell.UsuariosGlobal.Add(new Usuario {Id=MiUsu.Id,rut = MiUsu.rut,iniciales=MiUsu.iniciales,usado=MiUsu.usado });
        }
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
                //ListarUsuarios();
                cargar_usuarios();
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

    private void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        UsuarioSeleccionado = null;
        UsuarioSeleccionado = (Usuario)e.SelectedItem;

        //DisplayAlert("Item seleccionado", UsuarioSeleccionado.Id.ToString(), "Aceptar");

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
        //ListarUsuarios();
    }

}