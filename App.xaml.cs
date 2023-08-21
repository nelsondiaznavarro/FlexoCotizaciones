namespace FlexoCotizaciones;

public partial class App : Application
{

    public static ParametrosRepository ParametrosRepo { get; set; }
    public static string rutConectado;
    public static string InicialesConectado;
    public  static  int idClienteSeleccionado = 0;
    public App()
	{
		InitializeComponent();
        MainPage = new AppShell();
        //MainPage = new Microsoft.Maui.Controls.NavigationPage(new PantallaInicio()); 
        //ParametrosRepo = parametrosRepository;
    }

    protected override void OnResume()
    {
        base.OnResume();
    }
}
