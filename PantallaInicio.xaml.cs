namespace FlexoCotizaciones;

public partial class PantallaInicio : ContentPage
{
    public PantallaInicio()
    {
        InitializeComponent();
       



    }

    private async void OnButton_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new ParametrosConf());
    }


}
