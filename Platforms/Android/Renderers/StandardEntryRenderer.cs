using FlexoCotizaciones.Renderers;
using Microsoft.Maui.Controls.Compatibility;
#if IOS || MACCATALYST
using PlatformView = Microsoft.Maui.Platform.MauiTextField;
#elif ANDROID
using PlatformView = AndroidX.AppCompat.Widget.AppCompatEditText;
#elif WINDOWS
using PlatformView = Microsoft.UI.Xaml.Controls.TextBox;
#elif (NETSTANDARD || !PLATFORM) || (NET6_0_OR_GREATER && !IOS && !ANDROID)
using PlatformView = System.Object;
#endif
using Android.Content;
using Android.Views;
//using Microsoft.Maui.Controls.Handlers.Compatibility;
using Microsoft.Maui.Handlers;
using FlexoCotizaciones.Controls;
using System.ComponentModel;
using Microsoft.Maui.Controls.Compatibility.Platform.Android;
using Android.Graphics.Drawables;
using Microsoft.Maui.Platform;
using Microsoft.Maui.Controls.Platform;

namespace FlexoCotizaciones.Renderers;

public class StandardEntryRenderer : VisualElementRenderer<StandardEntry>
{
    public StandardEntryRenderer(Context context) : base(context)
    {
    }

    public StandardEntry ElementV2 => Element as StandardEntry;
    //protected override FormsEditText CreateNativeControl()
    //{
    //    var control = base.CreateNativeControl();
    //    UpdateBackground(control);
    //    return control;
    //}

    protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == StandardEntry.CornerRadiusProperty.PropertyName)
        {
            UpdateBackground();
        }
        else if (e.PropertyName == StandardEntry.BorderThicknessProperty.PropertyName)
        {
            UpdateBackground();
        }
        else if (e.PropertyName == StandardEntry.BorderColorProperty.PropertyName)
        {
            UpdateBackground();
        }

        base.OnElementPropertyChanged(sender, e);
    }

    protected override void UpdateBackgroundColor()
    {
        UpdateBackground();
    }
    protected void UpdateBackground(FormsEditText control)
    {
        if (control == null) return;

        var gd = new GradientDrawable();
        gd.SetColor(Element.BackgroundColor.ToAndroid());
        gd.SetCornerRadius(Context.ToPixels(ElementV2.CornerRadius));
        gd.SetStroke((int)Context.ToPixels(ElementV2.BorderThickness), ElementV2.BorderColor.ToAndroid());
        control.SetBackground(gd);

        var padTop = (int)Context.ToPixels(ElementV2.Padding.Top);
        var padBottom = (int)Context.ToPixels(ElementV2.Padding.Bottom);
        var padLeft = (int)Context.ToPixels(ElementV2.Padding.Left);
        var padRight = (int)Context.ToPixels(ElementV2.Padding.Right);

        control.SetPadding(padLeft, padTop, padRight, padBottom);
    }
    //protected void UpdateBackground()
    //{
    //    UpdateBackground(Control);
    //}
}

