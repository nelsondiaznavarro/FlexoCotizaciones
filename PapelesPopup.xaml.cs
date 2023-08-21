using Mopups.Services;
using FlexoCotizaciones.Models;
using System.Collections.ObjectModel;
using static System.Net.Mime.MediaTypeNames;
using System.Text;
using System.Xml.Serialization;

namespace FlexoCotizaciones;

public partial class PapelesPopup 
{
    private const string url = "http://dataservice.flexografica.cl/validausuario.php";
    private const string url_parametros = "http://dataservice.flexografica.cl/parametros.php";
    private HttpClient _Client = new HttpClient();
    private bool Editando = false;
    private int IdEditando = 0;
    public PapelesPopup(int IdPapel)
	{
		InitializeComponent();
        cmdCancelarPapel.Clicked += CmdCancelarPapel_Clicked;
        cmdIngresarPapel.Clicked += CmdIngresarPapel_Clicked;

        if (IdPapel > 0)
        {

            Editando = true;
            IdEditando = IdPapel;
            CargaPapel(IdPapel);
            cmdIngresarPapel.Text = "Modificar";
        }
    }


    private async void CargaPapel(int IdPapel)
    {
        string resultado = "";
        //AppShell.UsuariosGlobal.Clear();
        var httpResponse = await _Client.GetAsync(url_parametros + "?accion=buscarpapelid&IdPapel=" + IdPapel);
        if (httpResponse.IsSuccessStatusCode)
        {
            var responseData = httpResponse.Content.ReadAsStringAsync();
            resultado = responseData.Result;
            resultado = "<?xml version='1.0'?>" + resultado;
        }
        Papel MisUsuario = new Papel();

        XmlSerializer Serializador = new XmlSerializer(MisUsuario.GetType());

        byte[] byteArray = Encoding.ASCII.GetBytes(resultado);
        MemoryStream stream = new MemoryStream(byteArray);

        Papel MisUsuarioDes = (Papel)Serializador.Deserialize(stream);

        txtNombrePapel.Text = MisUsuarioDes.NombrePapel;
        txtValor.Text = MisUsuarioDes.Valor.ToString();
       



    }

    private async void CargaPapeles()
    {

        string resultado = "";
        AppShell.PapelesGlobal.Clear();

        var httpResponse = await _Client.GetAsync(url_parametros + "?accion=listarpapeles");
        if (httpResponse.IsSuccessStatusCode)
        {
            var responseData = httpResponse.Content.ReadAsStringAsync();
            resultado = responseData.Result;
            resultado = "<?xml version='1.0'?>" + resultado;
        }
        Papeles MisPapeles = new Papeles();

        XmlSerializer Serializador = new XmlSerializer(MisPapeles.GetType());

        byte[] byteArray = Encoding.ASCII.GetBytes(resultado);
        MemoryStream stream = new MemoryStream(byteArray);

        Papeles MisPepelesDes = (Papeles)Serializador.Deserialize(stream);

        foreach (Papel MiPapel in MisPepelesDes.ListaPapeles)
        {
            AppShell.PapelesGlobal.Add(new Papel { Id = MiPapel.Id, NombrePapel = MiPapel.NombrePapel, Valor = MiPapel.Valor, IdNombre = "" });
        }

    }


    private async void CmdIngresarPapel_Clicked(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(txtNombrePapel.Text) || string.IsNullOrEmpty(txtValor.Text))
        {
            await DisplayAlert("Papeles", "Debe ingresar todos los datos para agregar papel", "Aceptar");
            return;

        }

        string resultado = "";
        if (!Editando)
        {
            //AppShell.UsuariosGlobal.Clear();
            var httpResponse = await _Client.GetAsync(url_parametros + "?accion=crearpapel&NombrePapel=" + txtNombrePapel.Text + "&Valor=" + txtValor.Text );
            if (httpResponse.IsSuccessStatusCode)
            {
                var responseData = httpResponse.Content.ReadAsStringAsync();
                resultado = responseData.Result;
                resultado = "<?xml version='1.0'?>" + resultado;
            }

            if (resultado.IndexOf("<resultado>1</resultado>") > -1)   
            {
                await DisplayAlert("Papeles", "Papel ingresado correctamente", "Aceptar");
               
                CargaPapeles();
                MopupService.Instance.PopAsync();

            }
            else
            {
                await DisplayAlert("Clientes", "Error al ingresar cliente", "Aceptar");
                //LimpiarCampos();
                MopupService.Instance.PopAsync();
            }
        }
        else
        {
            //AppShell.UsuariosGlobal.Clear();
            var httpResponse = await _Client.GetAsync(url_parametros + "?accion=modificarpapel&IdPapel=" + IdEditando + "&NombrePapel=" + txtNombrePapel.Text + "&Valor=" + txtValor.Text);
            if (httpResponse.IsSuccessStatusCode)
            {
                var responseData = httpResponse.Content.ReadAsStringAsync();
                resultado = responseData.Result;
                resultado = "<?xml version='1.0'?>" + resultado;
            }

            if (resultado.IndexOf("<resultado>1</resultado>") > -1)
            {
                await DisplayAlert("Papeles", "Papel Modificado correctamente", "Aceptar");
               
                CargaPapeles();
                MopupService.Instance.PopAsync();

            }
            else
            {
                await DisplayAlert("Papeles", "Error al ingresar Papel", "Aceptar");
                
                MopupService.Instance.PopAsync();
            }
        }
    }

    private void CmdCancelarPapel_Clicked(object sender, EventArgs e)
    {
        MopupService.Instance.PopAsync();
    }
}