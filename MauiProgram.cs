using Microsoft.Extensions.Logging;
using Microsoft.Maui.LifecycleEvents;
using Mopups.Hosting;
using FlexoCotizaciones.Controls;
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
            })

    ;
        Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping("BoderlessEntry", (handler, view) =>
        {
#if ANDROID
            handler.PlatformView.SetBackgroundColor(Android.Graphics.Color.Transparent);
#elif IOS
            handler.PlatformView.BorderStyle = UIKit.UITextBorderStyle.None;
#elif WINDOWS
            handler.PlatformView.BorderThickness = new Microsoft.UI.Xaml.Thickness(0);
#endif
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
