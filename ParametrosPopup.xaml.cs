using Mopups.Services;
using FlexoCotizaciones.Models;
using System.Collections.ObjectModel;

namespace FlexoCotizaciones;

public partial class ParametrosPopup 
{

    public ObservableCollection<TipoParametros> tipoParametros { get; set; }

    public ParametrosPopup()
	{
		InitializeComponent();
        cmdSalir.Clicked += CmdSalir_Clicked;
        cmdAgregarPar.Clicked += CmdAgregarPar_Clicked;

        Dictionary<string, int> tipoAint = new Dictionary<string, int>
        {
            { "Valor Fijo", 0 }, { "Porcentaje", 1 },
            { "Desde-Hasta", 2 }
        };

        foreach (string colorName in tipoAint.Keys)
        {
            pickerTipo.Items.Add(colorName);
        }
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

    private void CmdAgregarPar_Clicked(object sender, EventArgs e)
    {
        App.ParametrosRepo.AddNewParametro(newParametro.Text, newValorPorcentaje.Text, newValorDesde.Text, newValorHasta.Text,pickerTipo.SelectedIndex != -1 ? pickerTipo.SelectedIndex.ToString()  : "0");
        DisplayAlert("Parametros", "Parametros ingresado correctamente", "Aceptar");
        newParametro.Text = "";
        newValorPorcentaje.Text = "";
        newValorDesde.Text = "";
        newValorHasta.Text = "";
        newParametro.Focus();
    }

    private void CmdSalir_Clicked(object sender, EventArgs e)
    {
        MopupService.Instance.PopAsync();
    }


    public List<Parametros> TraeParametros()
    {
        List<Parametros> parametros = App.ParametrosRepo.GetAllPeople();
        return parametros;
    }


}

public class TipoParametros
{
    public string Nombre { get; set; }

}