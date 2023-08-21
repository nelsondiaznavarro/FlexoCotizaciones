using FlexoCotizaciones.Models;
using Mopups.Services;
using System.Text;
using System.Xml.Serialization;

namespace FlexoCotizaciones;

public partial class IngresaClientes : ContentPage
{
    private const string url = "http://dataservice.flexografica.cl/validausuario.php";
    private const string url_parametros = "http://dataservice.flexografica.cl/parametros.php";
    private HttpClient _Client = new HttpClient();

    public IngresaClientes()
    {
        InitializeComponent();
        ListaClientesFront.ItemsSource = AppShell.ClientesGlobal;
        CargaClientes();
        cmdAgregarCliente.Clicked += CmdIngresarCliente_Clicked;
        cmdModificarUsuarios.Clicked += CmdModificarUsuarios_Clicked;
        cmdEliminarCliente.Clicked += CmdEliminarCliente_Clicked;
        cmdBuscarClientes.Clicked += CmdBuscarClientes_Clicked;
	}

    private async void CmdBuscarClientes_Clicked(object sender, EventArgs e)
    {
        //if (string.IsNullOrEmpty(txtBusqueda.Text))
        //{
        //    await DisplayAlert("Clientes", "Debe ingresar valor para busqueda", "Aceptar");
        //    return;
        //}

        BuscarClientes(txtBusqueda.Text);
    }

    private async void BuscarClientes(string datosBusqueda)
    {
        string resultado = "";
        AppShell.ClientesGlobal.Clear();

        var httpResponse = await _Client.GetAsync(url_parametros + "?accion=buscarClienteRutNombre&rutcliente="+ datosBusqueda + "&RazonSocial=" + datosBusqueda);
        if (httpResponse.IsSuccessStatusCode)
        {
            var responseData = httpResponse.Content.ReadAsStringAsync();
            resultado = responseData.Result;
            resultado = "<?xml version='1.0'?>" + resultado;
        }
        Clientes MisClientes = new Clientes();

        XmlSerializer Serializador = new XmlSerializer(MisClientes.GetType());

        byte[] byteArray = Encoding.ASCII.GetBytes(resultado);
        MemoryStream stream = new MemoryStream(byteArray);

        var MisClientesDes = (Clientes)Serializador.Deserialize(stream);

        foreach (Cliente MiCliente in MisClientesDes.ListaClientes)
        {
            AppShell.ClientesGlobal.Add(new Cliente { Id = MiCliente.Id, RUT = MiCliente.RUT, RazonSocial = MiCliente.RazonSocial, Direccion = MiCliente.Direccion, Comuna = MiCliente.Comuna, Ciudad = MiCliente.Ciudad, Contacto = MiCliente.Contacto, Telefono = MiCliente.Telefono });
        }

    }

    private async  void CmdEliminarCliente_Clicked(object sender, EventArgs e)
    {
        if (ListaClientesFront.SelectedItem == null)
        {
              await DisplayAlert("Clientes", "Debe selecionar un cliente para Eliminar", "Aceptar");
            return;
        }

        Cliente miitem = (Cliente)ListaClientesFront.SelectedItem;

        bool Respuesta = await DisplayAlert("Clientes", "Realmente desea eliminar Cliente ", "Aceptar","Cancelar");

        if (!Respuesta)
        {
            return;
        }

        string resultado = "";
        AppShell.ClientesGlobal.Clear();


        var httpResponse = await _Client.GetAsync(url_parametros + "?accion=eliminarCliente&idCliente=" + miitem.Id);
        if (httpResponse.IsSuccessStatusCode)
        {
            var responseData = httpResponse.Content.ReadAsStringAsync();
            resultado = responseData.Result;
            resultado = "<?xml version='1.0'?>" + resultado;
        }


        if (resultado.IndexOf("<resultado>1</resultado>") > -1)
        {
            await DisplayAlert("Clientes", "Cliente eliminado correctamente", "Aceptar");

            CargaClientes();


        }
        else
        {
            await DisplayAlert("Clientes", "Error al eliminar cliente", "Aceptar");
            CargaClientes();
        }
    }

    private async  void CmdModificarUsuarios_Clicked(object sender, EventArgs e)
    {

        if (ListaClientesFront.SelectedItem==null)
        {
            await DisplayAlert("Clientes", "Debe selecionar un cliente para Modificar", "Aceptar");
            return;
        }


        Cliente miitem = (Cliente)ListaClientesFront.SelectedItem;
        MopupService.Instance.PushAsync(new EmpresasPopup(miitem.Id));

    }

    private async void cmdEditarCliente_Clicked(object sender, EventArgs e)
    {
        await DisplayAlert("Editar", "Editar", "Aceptar");
        var miitem = ListaClientesFront.SelectedItem;
    }
    
    private async void CargaClientes()
    {
        string resultado = "";
        AppShell.ClientesGlobal.Clear();
        
        var httpResponse = await _Client.GetAsync(url_parametros + "?accion=listarClientes");
        if (httpResponse.IsSuccessStatusCode)
        {
            var responseData = httpResponse.Content.ReadAsStringAsync();
            resultado = responseData.Result;
            resultado = "<?xml version='1.0'?>" + resultado;
        }
        Clientes MisClientes = new Clientes();

        XmlSerializer Serializador = new XmlSerializer(MisClientes.GetType());

        byte[] byteArray = Encoding.ASCII.GetBytes(resultado);
        MemoryStream stream = new MemoryStream(byteArray);

        var MisClientesDes = (Clientes)Serializador.Deserialize(stream);

        foreach (Cliente MiCliente in MisClientesDes.ListaClientes)
        {
            AppShell.ClientesGlobal.Add(new Cliente { Id = MiCliente.Id, RUT = MiCliente.RUT, RazonSocial= MiCliente.RazonSocial, Direccion = MiCliente.Direccion, Comuna = MiCliente.Comuna, Ciudad = MiCliente.Ciudad, Contacto = MiCliente.Contacto, Telefono = MiCliente.Telefono });
        }

    }

    private void LimpiarCampos()
    {
        //txtRutCliente.Text = "";
        //txtRazonSocial.Text = "";
        //txtDireccion.Text = "";
        //txtComuna.Text = "";
        //txtCiudad.Text = "";
        //txtContacto.Text = "";
        //txtTelefono.Text = "";
    }



    private async  void CmdIngresarCliente_Clicked(object sender, EventArgs e)
    {

        MopupService.Instance.PushAsync(new EmpresasPopup(0));
    }
}