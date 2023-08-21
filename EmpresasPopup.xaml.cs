using Mopups.Services;
using FlexoCotizaciones.Models;
using System.Collections.ObjectModel;
using static System.Net.Mime.MediaTypeNames;
using System.Text;
using System.Xml.Serialization;

namespace FlexoCotizaciones;

public partial class EmpresasPopup
{
    private const string url = "http://dataservice.flexografica.cl/validausuario.php";
    private const string url_parametros = "http://dataservice.flexografica.cl/parametros.php";
    private HttpClient _Client = new HttpClient();
    private bool Editando = false;
    private int IdEditando = 0;
    public EmpresasPopup(int IdEmpresa=0)
	{
		InitializeComponent();

        cmdCancelarCliente.Clicked += CmdCancelarCliente_Clicked;
        cmdIngresarCliente.Clicked += CmdIngresarCliente_Clicked;
        if (IdEmpresa > 0)
        {

            Editando = true;
            IdEditando = IdEmpresa;
            CargaCliente(IdEmpresa);
            cmdIngresarCliente.Text = "Modificar";
        }
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
            AppShell.ClientesGlobal.Add(new Cliente { Id = MiCliente.Id, RUT = MiCliente.RUT, RazonSocial = MiCliente.RazonSocial, Direccion = MiCliente.Direccion, Comuna = MiCliente.Comuna, Ciudad = MiCliente.Ciudad, Contacto = MiCliente.Contacto, Telefono = MiCliente.Telefono });
        }

    }
    private async void CmdIngresarCliente_Clicked(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(txtRutCliente.Text) || string.IsNullOrEmpty(txtRazonSocial.Text) || string.IsNullOrEmpty(txtDireccion.Text)
            || string.IsNullOrEmpty(txtComuna.Text) || string.IsNullOrEmpty(txtCiudad.Text) || string.IsNullOrEmpty(txtContacto.Text) || string.IsNullOrEmpty(txtTelefono.Text))
        {
            await DisplayAlert("Clientes","Debe ingresar todos los datos para agregar cliente","Aceptar" );
            return;
        
        }

        string resultado = "";
        if (!Editando)
        {
            //AppShell.UsuariosGlobal.Clear();
            var httpResponse = await _Client.GetAsync(url_parametros + "?accion=crearcliente&rutcliente=" + txtRutCliente.Text + "&RazonSocial=" + txtRazonSocial.Text + "&Direccion=" + txtDireccion.Text
                                                                                                           + "&Comuna=" + txtComuna.Text + "&Ciudad=" + txtCiudad.Text + "&Contacto=" + txtContacto.Text + "&Telefono=" + txtTelefono.Text);
            if (httpResponse.IsSuccessStatusCode)
            {
                var responseData = httpResponse.Content.ReadAsStringAsync();
                resultado = responseData.Result;
                resultado = "<?xml version='1.0'?>" + resultado;
            }

            if (resultado.IndexOf("<resultado>1</resultado>") > -1)
            {
                await DisplayAlert("Clientes", "Cliente ingresado correctamente", "Aceptar");
                LimpiarCampos();
                CargaClientes();
                MopupService.Instance.PopAsync();

            }
            else
            {
                await DisplayAlert("Clientes", "Error al ingresar cliente", "Aceptar");
                LimpiarCampos();
                MopupService.Instance.PopAsync();
            }
        }
        else
        {
            //AppShell.UsuariosGlobal.Clear();
            var httpResponse = await _Client.GetAsync(url_parametros + "?accion=modificarcliente&idCliente="+ IdEditando + "&rutcliente=" + txtRutCliente.Text + "&RazonSocial=" + txtRazonSocial.Text + "&Direccion=" + txtDireccion.Text
                                                                                                           + "&Comuna=" + txtComuna.Text + "&Ciudad=" + txtCiudad.Text + "&Contacto=" + txtContacto.Text + "&Telefono=" + txtTelefono.Text);
            if (httpResponse.IsSuccessStatusCode)
            {
                var responseData = httpResponse.Content.ReadAsStringAsync();
                resultado = responseData.Result;
                resultado = "<?xml version='1.0'?>" + resultado;
            }

            if (resultado.IndexOf("<resultado>1</resultado>") > -1)
            {
                await DisplayAlert("Clientes", "Cliente ingresado correctamente", "Aceptar");
                LimpiarCampos();
                CargaClientes();
                MopupService.Instance.PopAsync();

            }
            else
            {
                await DisplayAlert("Clientes", "Error al ingresar cliente", "Aceptar");
                LimpiarCampos();
                MopupService.Instance.PopAsync();
            }
        }




        
      

    }

    private void LimpiarCampos()
    {
        txtRutCliente.Text = "";
        txtRazonSocial.Text = "";
        txtDireccion.Text = "";
        txtComuna.Text = "";
        txtCiudad.Text = "";
        txtContacto.Text = "";
        txtTelefono.Text = "";
    }

    private void CmdCancelarCliente_Clicked(object sender, EventArgs e)
    {
        MopupService.Instance.PopAsync();
    }

    private async void  CargaCliente(int IdEmpresa)
    {
        string resultado = "";
        AppShell.UsuariosGlobal.Clear();
        var httpResponse = await _Client.GetAsync(url_parametros + "?accion=buscarEmpresa&idCliente=" + IdEmpresa);
        if (httpResponse.IsSuccessStatusCode)
        {
            var responseData = httpResponse.Content.ReadAsStringAsync();
            resultado = responseData.Result;
            resultado = "<?xml version='1.0'?>" + resultado;
        }
        Cliente MisUsuario = new Cliente();

        XmlSerializer Serializador = new XmlSerializer(MisUsuario.GetType());

        byte[] byteArray = Encoding.ASCII.GetBytes(resultado);
        MemoryStream stream = new MemoryStream(byteArray);

        Cliente MisUsuarioDes = (Cliente)Serializador.Deserialize(stream);

        txtRutCliente.Text = MisUsuarioDes.RUT;
        txtRazonSocial.Text = MisUsuarioDes.RazonSocial;
        txtDireccion.Text = MisUsuarioDes.Direccion;
        txtComuna.Text = MisUsuarioDes.Comuna;
        txtCiudad.Text = MisUsuarioDes.Ciudad;
        txtContacto.Text = MisUsuarioDes.Contacto;
        txtTelefono.Text = MisUsuarioDes.Telefono;

        
       
    }

}