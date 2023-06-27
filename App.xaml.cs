namespace FlexoCotizaciones;

public partial class App : Application
{

    public static ParametrosRepository ParametrosRepo { get; set; }
    public App(ParametrosRepository parametrosRepository)
	{
		InitializeComponent();

        MainPage = new Microsoft.Maui.Controls.NavigationPage(new PantallaInicio()); 
        ParametrosRepo = parametrosRepository;
    }
}
