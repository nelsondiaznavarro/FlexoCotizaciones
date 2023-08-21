using FlexoCotizaciones.Models;
using Mopups.Services;
using System.Text;
using System.Xml.Serialization;

namespace FlexoCotizaciones;

public partial class IngresaPapeles : ContentPage
{
    private const string url = "http://dataservice.flexografica.cl/validausuario.php";
    private const string url_parametros = "http://dataservice.flexografica.cl/parametros.php";
    private HttpClient _Client = new HttpClient();


    public IngresaPapeles()
	{
		InitializeComponent();
       
        ListaPapelesFront.ItemsSource = AppShell.PapelesGlobal;
        CargaPapeles();
        cmdAgregarPapel.Clicked += CmdAgregarPapel_Clicked;
        cmdModificarPapel.Clicked += CmdModificarPapel_Clicked;
        cmdEliminarPapel.Clicked += CmdEliminarPapel_Clicked;
    }

    private async void CmdEliminarPapel_Clicked(object sender, EventArgs e)
    {
        if (ListaPapelesFront.SelectedItem == null)
        {
            await DisplayAlert("Papeles", "Debe selecionar un papel para Eliminar", "Aceptar");
            return;
        }

        Papel miitem = (Papel)ListaPapelesFront.SelectedItem;

        bool Respuesta = await DisplayAlert("Papeles", "Realmente desea eliminar Papel ", "Aceptar", "Cancelar");

        if (!Respuesta)
        {
            return;
        }

        string resultado = "";
        AppShell.ClientesGlobal.Clear();


        var httpResponse = await _Client.GetAsync(url_parametros + "?accion=eliminarPapel&IdPapel=" + miitem.Id);
        if (httpResponse.IsSuccessStatusCode)
        {
            var responseData = httpResponse.Content.ReadAsStringAsync();
            resultado = responseData.Result;
            resultado = "<?xml version='1.0'?>" + resultado;
        }


        if (resultado.IndexOf("<resultado>1</resultado>") > -1)
        {
            await DisplayAlert("Papeles", "Papel eliminado correctamente", "Aceptar");

            CargaPapeles();


        }
        else
        {
            await DisplayAlert("Papel", "Error al eliminar Papel", "Aceptar");
            CargaPapeles();
        }
    }

    private async void CmdModificarPapel_Clicked(object sender, EventArgs e)
    {

        if (ListaPapelesFront.SelectedItem == null)
        {
            await DisplayAlert("Papeles", "Debe selecionar un papel para Modificar", "Aceptar");
            return;
        }


        Papel miitem = (Papel)ListaPapelesFront.SelectedItem;
        MopupService.Instance.PushAsync(new PapelesPopup(miitem.Id));
    }

    private void CmdAgregarPapel_Clicked(object sender, EventArgs e)
    {
        MopupService.Instance.PushAsync(new PapelesPopup(0));
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
            AppShell.PapelesGlobal.Add(new Papel { Id = MiPapel.Id, NombrePapel = MiPapel.NombrePapel, Valor = MiPapel.Valor, IdNombre= ""});
        }

    }

}