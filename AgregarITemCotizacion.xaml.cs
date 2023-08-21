using Mopups.Services;
using FlexoCotizaciones.Models;
using System.Collections.ObjectModel;
using static System.Net.Mime.MediaTypeNames;
using System.Xml.Serialization;
using System.Text;
namespace FlexoCotizaciones;

public partial class AgregarITemCotizacion : ContentPage
{
    private const string url = "http://dataservice.flexografica.cl/validausuario.php";
    private const string url_parametros = "http://dataservice.flexografica.cl/parametros.php";
    private HttpClient _Client = new HttpClient();


    //valores para  calculo de etiquetas 
    private double _precio_papel = 0;
    private double _IdPapel = 0;
    private CalculoParametros _ParametrosEtiquetas = null;
    public AgregarITemCotizacion()
	{
        InitializeComponent();
        ListarPapeles();
        TraeParametros();

        txtAltoEtiquetas.Completed += TxtAltoEtiquetas_Completed;


        cmbTroqueles.SelectedIndexChanged += CmbTroqueles_SelectedIndexChanged;

        cmbTipoItem.SelectedIndexChanged += CmbTipoItem_SelectedIndexChanged;
        cmbTiposPapel.SelectedIndexChanged += CmbTiposPapel_SelectedIndexChanged;

        cmdIngresarProducto.Clicked += CmdIngresarProducto_Clicked;
        //cmdCancelarProducto.Clicked += CmdCancelarProducto_Clicked;

        cmdIngresarServicio.Clicked += CmdIngresarServicio_Clicked;
        //cmdCancelarServicio.Clicked += CmdCancelarServicio_Clicked;

        cmdIngresarEtiqueta.Clicked += CmdIngresarEtiqueta_Clicked;
        //cmdCancelarEtiqueta.Clicked += CmdCancelarEtiqueta_Clicked;
    }

    private void CmbTroqueles_SelectedIndexChanged(object sender, EventArgs e)
    {
        var picker = (Picker)sender;
        int selectedIndex = picker.SelectedIndex;

        if (selectedIndex != -1)
        {
            try
            {
                string datoseleccionado = picker.Items[selectedIndex];
                txtLargoEtiquetas.Text = datoseleccionado.Split('-')[0].ToLower().Split('x')[1];
            }
            catch (Exception ex)
            {
                txtLargoEtiquetas.Text = "0";
            }

            txtcantidadEtiquetas.Focus();

        }
    }

    private void TxtAltoEtiquetas_Completed(object sender, EventArgs e)
    {
        TraeTroqueles(txtAltoEtiquetas.Text);
    }

    private async void CmdCancelarEtiqueta_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("Configuracion");
    }

    private void CmdIngresarEtiqueta_Clicked(object sender, EventArgs e)
    {
        throw new NotImplementedException();
    }

    private async void CmdCancelarServicio_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("Configuracion");
    }

    private void CmdIngresarServicio_Clicked(object sender, EventArgs e)
    {
        throw new NotImplementedException();
    }

    private async void CmdCancelarProducto_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("Configuracion");
    }

    private void CmdIngresarProducto_Clicked(object sender, EventArgs e)
    {
        throw new NotImplementedException();
    }

    private async void CmbTiposPapel_SelectedIndexChanged(object sender, EventArgs e)
    {

        Papel papel_seleccionado = (Papel)cmbTiposPapel.SelectedItem;
        _IdPapel = papel_seleccionado.Id;


        //traer datos del papel
        string resultado = "";

        var httpResponse = await _Client.GetAsync(url_parametros + "?accion=buscarpapelid&IdPapel=" + _IdPapel.ToString());

        if (httpResponse.IsSuccessStatusCode)
        {
            var responseData = httpResponse.Content.ReadAsStringAsync();
            resultado = responseData.Result;
            resultado = "<?xml version='1.0'?>" + resultado;
        }
        Papel MisUsuarios = new Papel();

        XmlSerializer Serializador = new XmlSerializer(MisUsuarios.GetType());

        byte[] byteArray = Encoding.ASCII.GetBytes(resultado);
        MemoryStream stream = new MemoryStream(byteArray);

        Papel MisPapeles = (Papel)Serializador.Deserialize(stream);
        _precio_papel = MisPapeles.Valor;
        
        //lblValorMt2.Text = MisPapeles.Valor.ToString();

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

        _ParametrosEtiquetas = MisParametrosDes;
        txtPorcComision.Text = _ParametrosEtiquetas.ComisionDefecto.ToString();

    }

    public async void TraeTroqueles(string anchoxalto)
    {
        string resultado = "";

        var httpResponse = await _Client.GetAsync(url_parametros + "?accion=buscartroquelmedida&Medida=" + anchoxalto);
        if (httpResponse.IsSuccessStatusCode)
        {
            var responseData = httpResponse.Content.ReadAsStringAsync();
            resultado = responseData.Result;
            resultado = "<?xml version='1.0'?>" + resultado;
        }
        Troqueles MisParametros = new Troqueles();

        XmlSerializer Serializador = new XmlSerializer(MisParametros.GetType());

        byte[] byteArray = Encoding.ASCII.GetBytes(resultado);
        MemoryStream stream = new MemoryStream(byteArray);

        var misTroqueles = (Troqueles)Serializador.Deserialize(stream);

        cmbTroqueles.ItemsSource = null;
        cmbTroqueles.ItemsSource = misTroqueles.ListaTroqueles;

    }


    private async void ListarPapeles()
    {
        //ListaUsuariosFront.ItemsSource = null;
        string resultado = "";

        var httpResponse = await _Client.GetAsync(url_parametros + "?accion=listarpapeles");
        if (httpResponse.IsSuccessStatusCode)
        {
            var responseData = httpResponse.Content.ReadAsStringAsync();
            resultado = responseData.Result;
            resultado = "<?xml version='1.0'?>" + resultado;
        }
        Papeles MisUsuarios = new Papeles();

        XmlSerializer Serializador = new XmlSerializer(MisUsuarios.GetType());

        byte[] byteArray = Encoding.ASCII.GetBytes(resultado);
        MemoryStream stream = new MemoryStream(byteArray);

        var MisPapeles = (Papeles)Serializador.Deserialize(stream);

        cmbTiposPapel.ItemsSource = MisPapeles.ListaPapeles;
    }


    private void CmbTipoItem_SelectedIndexChanged(object sender, EventArgs e)
    {


        var picker = (Picker)sender;
        int selectedIndex = picker.SelectedIndex;

        if (selectedIndex != -1)
        {
            DisplayAlert(picker.Items[selectedIndex], "", "Aceptar");
            if (picker.Items[selectedIndex] == "Producto")
            {
                gridProductos.IsVisible = true;
                gridServicios.IsVisible = false;
                gridEtiquetas.IsVisible = false;

            }

            if (picker.Items[selectedIndex] == "Servicio")
            {
                gridProductos.IsVisible = false;
                gridServicios.IsVisible = true;
                gridEtiquetas.IsVisible = false;

            }

            if (picker.Items[selectedIndex] == "Etiquetas")
            {
                gridProductos.IsVisible = false;
                gridServicios.IsVisible = false;
                gridEtiquetas.IsVisible = true;

            }

        }

    }
}