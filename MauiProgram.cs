using Microsoft.Extensions.Logging;
using Mopups.Hosting;
namespace FlexoCotizaciones;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureMopups()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

#if DEBUG
		builder.Logging.AddDebug();
#endif
        string dbPathParametros = FileAccessHelper.GetLocalFilePath("parametrosdb.db3");
        builder.Services.AddSingleton<ParametrosRepository>
          (s => ActivatorUtilities.CreateInstance<ParametrosRepository>(s, dbPathParametros));

        return builder.Build();
	}
}
