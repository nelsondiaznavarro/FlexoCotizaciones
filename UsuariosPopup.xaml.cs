using Mopups.Services;
using FlexoCotizaciones.Models;
using System.Collections.ObjectModel;
using static System.Net.Mime.MediaTypeNames;
using System.Text;
using System.Xml.Serialization;

namespace FlexoCotizaciones;

public partial class UsuariosPopup 
{


    private const string url = "http://dataservice.flexografica.cl/validausuario.php";
    private HttpClient _Client = new HttpClient();

    public ObservableCollection<TipoParametros> tipoParametros { get; set; }

    public UsuariosPopup()
	{
		InitializeComponent();
        cmdSalir.Clicked += CmdSalir_Clicked;
        cmdAgregarPar.Clicked += CmdAgregarPar_Clicked;
    
    }


    private void OnPickerSelectedIndexChanged(object sender, EventArgs e)
    {
        var picker = (Picker)sender;
        int selectedIndex = picker.SelectedIndex;

        if (selectedIndex != -1)
        {
            //labelname.Text = picker.Items[selectedIndex];
        }
    }

    private async void CmdAgregarPar_Clicked(object sender, EventArgs e)
    {

        if (string.IsNullOrEmpty(txtRutUsuario.Text))
        {
            await DisplayAlert("Usuarios", "Debe ingresar RUT Usuario", "Aceptar");
            txtRutUsuario.Focus();
            return;
        }
        var httpResponseAct = await _Client.GetAsync(url + "?accion=crear&rutusuario=" + txtRutUsuario.Text + "&usado=0");

        
        await DisplayAlert("Usuarios", "Usuario ingresado correctamente", "Aceptar");
        txtRutUsuario.Text = "";
        txtIniciales.Text = "";
        cargar_usuarios();
        txtRutUsuario.Focus();
       
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
            AppShell.UsuariosGlobal.Add(new Usuario { Id = MiUsu.Id, rut = MiUsu.rut, iniciales = MiUsu.iniciales, usado = MiUsu.usado });
        }
    }

    private void CmdSalir_Clicked(object sender, EventArgs e)
    {
        //Navigation.PushAsync(new ConfUsuarios());
        MopupService.Instance.PopAsync();
    }


    public List<Parametros> TraeParametros()
    {
        List<Parametros> parametros = App.ParametrosRepo.GetAllPeople();
        return parametros;
    }


}

