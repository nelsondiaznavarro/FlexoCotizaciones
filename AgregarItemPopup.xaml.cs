using Mopups.Services;
using FlexoCotizaciones.Models;
using System.Collections.ObjectModel;
using static System.Net.Mime.MediaTypeNames;
using System.Xml.Serialization;
using System.Text;

namespace FlexoCotizaciones;
public partial class AgregarItemPopup
{
    private const string url = "http://dataservice.flexografica.cl/validausuario.php";
    private const string url_parametros = "http://dataservice.flexografica.cl/parametros.php";
    private HttpClient _Client = new HttpClient();


    //valores para  calculo de etiquetas 
    private double _precio_papel = 0;
    private double _IdPapel = 0;
    private CalculoParametros _ParametrosEtiquetas = null;

    private int gap = 3;

    private string DescripcionAdicional = "";
    private bool TieneDescAdicional = false;

    public AgregarItemPopup()
    {
        InitializeComponent();
        TieneDescAdicional = false;
        DescripcionAdicional = "";
        //lblFolio.Text = AppShell.CotizacionGlobal.FirstOrDefault().Folio.ToString();

        //AppShell.ItemCotizacionGlobal.Add(new ItemCotizacion { Id = 0, Descripcion = "Prueba de item 2", UM = "UNI", Valor = 1000, Total = 10000, Cantidad = 10 });
        ListarPapeles();
        TraeParametros();

        txtAltoEtiquetas.Completed += TxtAltoEtiquetas_Completed;


        cmdCalcularEtiqueta.Clicked += CmdCalcularEtiqueta_Clicked;
        cmbTipoItem.SelectedIndexChanged += CmbTipoItem_SelectedIndexChanged;
        cmbTiposPapel.SelectedIndexChanged += CmbTiposPapel_SelectedIndexChanged;

        cmbTroqueles.SelectedIndexChanged += CmbTroqueles_SelectedIndexChanged;

        cmdIngresarProducto.Clicked += CmdIngresarProducto_Clicked;
        cmdCancelarProducto.Clicked += CmdCancelarProducto_Clicked;

        cmdIngresarServicio.Clicked += CmdIngresarServicio_Clicked;
        cmdCancelarServicio.Clicked += CmdCancelarServicio_Clicked;

        cmdIngresarEtiqueta.Clicked += CmdIngresarEtiqueta_Clicked;
        cmdCancelarEtiqueta.Clicked += CmdCancelarEtiqueta_Clicked;

        cmdDescAdicional.Clicked += CmdDescAdicional_Clicked;
    }

    private async  void CmdDescAdicional_Clicked(object sender, EventArgs e)
    {
        string result = await DisplayPromptAsync("Descripcion Adicional", "Ingrese descripcion Adicional ");

        if (!string.IsNullOrEmpty(result))
        {
            TieneDescAdicional = true;
            DescripcionAdicional = result;
        }
    }

    private void CmdCalcularEtiqueta_Clicked(object sender, EventArgs e)
    {
        //validaciones

        if (cmbTiposPapel.SelectedIndex == -1)
        {
            DisplayAlert("Error en calculo", "Debe seleccionar tipo de papel", "Aceptar");
            return;
        }

        double valorAlto=0;
        double valorAncho =0;
        if (string.IsNullOrEmpty(txtAltoEtiquetas.Text) || !double.TryParse(txtAltoEtiquetas.Text, out valorAlto) || string.IsNullOrEmpty(txtLargoEtiquetas.Text) || !double.TryParse(txtLargoEtiquetas.Text, out valorAncho))
        {
            DisplayAlert("Error en calculo","Debe ingresar alto y ancho y deben ser numerico","Aceptar");
            return;
        }

        double cantidadEtiquetas = 0;
        if (string.IsNullOrEmpty(txtcantidadEtiquetas.Text) || !double.TryParse(txtcantidadEtiquetas.Text, out cantidadEtiquetas))
        {
            DisplayAlert("Error en calculo", "Debe ingresar cantidad etiquetas  y debe ser numerico", "Aceptar");
            return;
        }

        double cantidadLaminas = 0;
        if (string.IsNullOrEmpty(txtLaminas.Text) || !double.TryParse(txtLaminas.Text, out cantidadLaminas))
        {
            DisplayAlert("Error en calculo", "Debe ingresar cantidad laminas  y debe ser numerico", "Aceptar");
            return;
        }

        double cantidadColores = 0;
        if (string.IsNullOrEmpty(txtColores.Text) || !double.TryParse(txtColores.Text, out cantidadColores))
        {
            DisplayAlert("Error en calculo", "Debe ingresar cantidad Colores y debe ser numerico", "Aceptar");
            return;
        }

        double cantidadClisse = 0;
        if (string.IsNullOrEmpty(txtClisse.Text) || !double.TryParse(txtClisse.Text, out cantidadClisse))
        {
            DisplayAlert("Error en calculo", "Debe ingresar cantidad Clisse", "Aceptar");
            return;
        }

        double cantidadBarniz = 0;
        if (string.IsNullOrEmpty(txtBarniz.Text) || !double.TryParse(txtBarniz.Text, out cantidadBarniz))
        {
            DisplayAlert("Error en calculo", "Debe ingresar barniz", "Aceptar");
            return;
        }

        double cantidadPorcComision = 0;
        if (string.IsNullOrEmpty(txtPorcComision.Text) || !double.TryParse(txtPorcComision.Text, out cantidadPorcComision))
        {
            DisplayAlert("Error en calculo", "Debe ingresar % Comision", "Aceptar");
            return;
        }

        double cantEtiquetasBanda = 0;
        if (string.IsNullOrEmpty(txtNumeroBandas.Text) || !double.TryParse(txtNumeroBandas.Text, out cantEtiquetasBanda))
        {
            DisplayAlert("Error en calculo", "Debe ingresar N° Etiquetas x Bandas", "Aceptar");
            return;
        }

        //realizar calculo
        double numeroBandas = cantEtiquetasBanda;
        double mermacada1000mts = 50;
        double metrosLineales = Math.Round(((valorAlto + 1)/1000)*(cantidadEtiquetas/ numeroBandas));
       
        double merma = (metrosLineales / 1000) * mermacada1000mts;
        double calce = _ParametrosEtiquetas.Calce;
        double total = Math.Round(metrosLineales + merma + calce);
        double anchopapel = (numeroBandas * (valorAncho + 3))/10 ;
        double metros2 = Math.Round((anchopapel / 100) * total);
        double tiempomaquina = total / _ParametrosEtiquetas.MetrosHora;


        double papel = (anchopapel/100)*total* _precio_papel;
        double barniz = 230;
        double tinta = 400;
        double lamina = _ParametrosEtiquetas.lamina * cantidadLaminas;
        double clisse = _ParametrosEtiquetas.Clisse * cantidadClisse;
        double maquina = (total / _ParametrosEtiquetas.MetrosHora) * _ParametrosEtiquetas.HorasMaquina;

        double totalplata = papel + barniz + tinta + lamina + clisse + maquina;

        double etiqueta_costo = totalplata / cantidadEtiquetas;
        double valor_etiqueta = etiqueta_costo + (etiqueta_costo * ((_ParametrosEtiquetas.Ganancia + double.Parse(txtPorcComision.Text)) / 100)) + 3;

        double total_plata_cliente = valor_etiqueta * cantidadEtiquetas;

        double comision_vendedor = total_plata_cliente * (double.Parse(txtPorcComision.Text) / 100);

        txtPrecioEtiqueta.Text = valor_etiqueta.ToString("###,###,###.###");
        txtTotalItemEtiqueta.Text = Math.Round( total_plata_cliente).ToString("###,###,###");
        txtMontoComision.Text = Math.Round( comision_vendedor).ToString("###,###,###");


    }

    private void CmbTroqueles_SelectedIndexChanged(object sender, EventArgs e)
    {
        var picker = (Picker)sender;
        int selectedIndex = picker.SelectedIndex;

        if (selectedIndex != -1)
        {
            try
            {
                string datoseleccionado = picker.Items[selectedIndex];
                txtLargoEtiquetas.Text = datoseleccionado.Split('-')[0].ToLower().Split('x')[1];
            }
            catch (Exception ex)
            {
                txtLargoEtiquetas.Text = "0";
            }

            txtcantidadEtiquetas.Focus();

        }
    }

    private void TxtAltoEtiquetas_Completed(object sender, EventArgs e)
    {
        TraeTroqueles(txtAltoEtiquetas.Text);
       
    }

    private async void CmdCancelarEtiqueta_Clicked(object sender, EventArgs e)
    {
        //Navigation.PushAsync(new Cotizaciones());
        await MopupService.Instance.PopAsync();
    }

    private void CmdIngresarEtiqueta_Clicked(object sender, EventArgs e)
    {
        //validar si esta hecho el calculo
        if (cmbTiposPapel.SelectedIndex == -1)
        {
            DisplayAlert("Error en calculo", "Debe seleccionar tipo de papel", "Aceptar");
            return;
        }

        double valorAlto = 0;
        double valorAncho = 0;
        if (string.IsNullOrEmpty(txtAltoEtiquetas.Text) || !double.TryParse(txtAltoEtiquetas.Text, out valorAlto) || string.IsNullOrEmpty(txtLargoEtiquetas.Text) || !double.TryParse(txtLargoEtiquetas.Text, out valorAncho))
        {
            DisplayAlert("Error en calculo", "Debe ingresar alto y ancho y deben ser numerico", "Aceptar");
            return;
        }

        double cantidadEtiquetas = 0;
        if (string.IsNullOrEmpty(txtcantidadEtiquetas.Text) || !double.TryParse(txtcantidadEtiquetas.Text, out cantidadEtiquetas))
        {
            DisplayAlert("Error en calculo", "Debe ingresar cantidad etiquetas  y debe ser numerico", "Aceptar");
            return;
        }

        double cantidadLaminas = 0;
        if (string.IsNullOrEmpty(txtLaminas.Text) || !double.TryParse(txtLaminas.Text, out cantidadLaminas))
        {
            DisplayAlert("Error en calculo", "Debe ingresar cantidad laminas  y debe ser numerico", "Aceptar");
            return;
        }

        double cantidadColores = 0;
        if (string.IsNullOrEmpty(txtColores.Text) || !double.TryParse(txtColores.Text, out cantidadColores))
        {
            DisplayAlert("Error en calculo", "Debe ingresar cantidad Colores y debe ser numerico", "Aceptar");
            return;
        }

        double cantidadClisse = 0;
        if (string.IsNullOrEmpty(txtClisse.Text) || !double.TryParse(txtClisse.Text, out cantidadClisse))
        {
            DisplayAlert("Error en calculo", "Debe ingresar cantidad Clisse", "Aceptar");
            return;
        }

        double cantidadBarniz = 0;
        if (string.IsNullOrEmpty(txtBarniz.Text) || !double.TryParse(txtBarniz.Text, out cantidadBarniz))
        {
            DisplayAlert("Error en calculo", "Debe ingresar barniz", "Aceptar");
            return;
        }

        double cantidadPorcComision = 0;
        if (string.IsNullOrEmpty(txtPorcComision.Text) || !double.TryParse(txtPorcComision.Text, out cantidadPorcComision))
        {
            DisplayAlert("Error en calculo", "Debe ingresar % Comision", "Aceptar");
            return;
        }

        if (!string.IsNullOrEmpty(txtPrecioEtiqueta.Text) && !string.IsNullOrEmpty(txtTotalItemEtiqueta.Text))
        {
            var PapelSeleccionado = (Papel)cmbTiposPapel.SelectedItem;
            string DescPaso = "Etiqueta " + txtAltoEtiquetas.Text + "x" + txtLargoEtiquetas.Text + " " + PapelSeleccionado.NombrePapel.ToString() + " " + txtColores.Text + " Color(es)";
            if (TieneDescAdicional)
            {
                DescPaso = DescPaso + " " + DescripcionAdicional; 
            }
            AppShell.ItemCotizacionGlobal.Add(new ItemCotizacion { Id = 0, Descripcion = DescPaso, UM = "UNI", Valor = double.Parse( txtPrecioEtiqueta.Text), Total = double.Parse(txtTotalItemEtiqueta.Text), Cantidad = int.Parse( txtcantidadEtiquetas.Text) });
            calcular_totales();
            MopupService.Instance.PopAsync();
        }
        else
        {
            DisplayAlert("Calculo Etiquetas", "Debe realizar el calculo para ingresar a presupuesto", "Aceptar");
        }


    }

    public void calcular_totales()
    {
        double subtotal = 0;
        double iva = 0;
        double total = 0;

        foreach (ItemCotizacion itemCotizacion in AppShell.ItemCotizacionGlobal)
        {
            //DisplayAlert("Total Item", itemCotizacion.Total.ToString(), "Aceptar");
            subtotal += itemCotizacion.Total;
        }

        iva = subtotal * 0.19;

        total = subtotal + iva;

        AppShell.TotalGlobal.Clear();
        AppShell.TotalGlobal.Add(new Total { Nombre = "SubTotal", Valor =subtotal });
        AppShell.TotalGlobal.Add(new Total { Nombre = "% Tasa IVA", Valor = 19 });
        AppShell.TotalGlobal.Add(new Total { Nombre = "IVA", Valor =iva });
        AppShell.TotalGlobal.Add(new Total { Nombre = "Total", Valor = total});
        //lblSubTotal.Text = subtotal.ToString("###,####,###,###");
        //lblIVA.Text = iva.ToString("###,####,###,###");
        //lblTotal.Text = total.ToString("###,####,###,###");
    }


    private async void CmdCancelarServicio_Clicked(object sender, EventArgs e)
    {
        //Navigation.PushAsync(new Cotizaciones());
        await MopupService.Instance.PopAsync();
    }

    private void CmdIngresarServicio_Clicked(object sender, EventArgs e)
    {
        throw new NotImplementedException();
    }

    private async void CmdCancelarProducto_Clicked(object sender, EventArgs e)
    {
        //Navigation.PushAsync(new Cotizaciones());
        await MopupService.Instance.PopAsync();
    }

    private void CmdIngresarProducto_Clicked(object sender, EventArgs e)
    {
        throw new NotImplementedException();
    }

    private async void CmbTiposPapel_SelectedIndexChanged(object sender, EventArgs e)
    {

        Papel papel_seleccionado = (Papel)cmbTiposPapel.SelectedItem;
        _IdPapel = papel_seleccionado.Id;


        //traer datos del papel
        string resultado = "";

        var httpResponse = await _Client.GetAsync(url_parametros + "?accion=buscarpapelid&IdPapel=" + _IdPapel.ToString());

        if (httpResponse.IsSuccessStatusCode)
        {
            var responseData = httpResponse.Content.ReadAsStringAsync();
            resultado = responseData.Result;
            resultado = "<?xml version='1.0'?>" + resultado;
        }
        Papel MisUsuarios = new Papel();

        XmlSerializer Serializador = new XmlSerializer(MisUsuarios.GetType());

        byte[] byteArray = Encoding.ASCII.GetBytes(resultado);
        MemoryStream stream = new MemoryStream(byteArray);

        Papel MisPapeles = (Papel)Serializador.Deserialize(stream);
        _precio_papel = MisPapeles.Valor;

        //lblValorMt2.Text = MisPapeles.Valor.ToString();

    }


    public async void TraeParametros()
    {
        string resultado = "";

        var httpResponse = await _Client.GetAsync(url_parametros + "?accion=listar");
        if (httpResponse.IsSuccessStatusCode)
        {
            var responseData = httpResponse.Content.ReadAsStringAsync();
            resultado = responseData.Result;
            resultado = "<?xml version='1.0'?>" + resultado;
        }
        CalculoParametros MisParametros = new CalculoParametros();

        XmlSerializer Serializador = new XmlSerializer(MisParametros.GetType());

        byte[] byteArray = Encoding.ASCII.GetBytes(resultado);
        MemoryStream stream = new MemoryStream(byteArray);

        var MisParametrosDes = (CalculoParametros)Serializador.Deserialize(stream);

        _ParametrosEtiquetas = MisParametrosDes;
        txtPorcComision.Text = _ParametrosEtiquetas.ComisionDefecto.ToString();

    }

    public async void TraeTroqueles(string anchoxalto)
    {
        string resultado = "";

        var httpResponse = await _Client.GetAsync(url_parametros + "?accion=buscartroquelmedida&Medida=" + anchoxalto);
        if (httpResponse.IsSuccessStatusCode)
        {
            var responseData = httpResponse.Content.ReadAsStringAsync();
            resultado = responseData.Result;
            resultado = "<?xml version='1.0'?>" + resultado;
        }
        Troqueles MisParametros = new Troqueles();

        XmlSerializer Serializador = new XmlSerializer(MisParametros.GetType());

        byte[] byteArray = Encoding.ASCII.GetBytes(resultado);
        MemoryStream stream = new MemoryStream(byteArray);

        var misTroqueles = (Troqueles)Serializador.Deserialize(stream);

        cmbTroqueles.ItemsSource = null;
        if (misTroqueles.ListaTroqueles.Count > 0)
        {
            cmbTroqueles.BackgroundColor = Colors.GreenYellow;

        }

        cmbTroqueles.ItemsSource = misTroqueles.ListaTroqueles;

    }


    private async void ListarPapeles()
    {
        //ListaUsuariosFront.ItemsSource = null;
        string resultado = "";

        var httpResponse = await _Client.GetAsync(url_parametros + "?accion=listarpapeles");
        if (httpResponse.IsSuccessStatusCode)
        {
            var responseData = httpResponse.Content.ReadAsStringAsync();
            resultado = responseData.Result;
            resultado = "<?xml version='1.0'?>" + resultado;
        }
        Papeles MisUsuarios = new Papeles();

        XmlSerializer Serializador = new XmlSerializer(MisUsuarios.GetType());

        byte[] byteArray = Encoding.ASCII.GetBytes(resultado);
        MemoryStream stream = new MemoryStream(byteArray);

        var MisPapeles = (Papeles)Serializador.Deserialize(stream);

        cmbTiposPapel.ItemsSource = MisPapeles.ListaPapeles;
    }


    private void CmbTipoItem_SelectedIndexChanged(object sender, EventArgs e)
    {


        var picker = (Picker)sender;
        int selectedIndex = picker.SelectedIndex;

        if (selectedIndex != -1)
        {
            //DisplayAlert(picker.Items[selectedIndex], "", "Aceptar");
            if (picker.Items[selectedIndex] == "Producto")
            {
                gridProductos.IsVisible = true;
                gridServicios.IsVisible = false;
                gridEtiquetas.IsVisible = false;

            }

            if (picker.Items[selectedIndex] == "Servicio")
            {
                gridProductos.IsVisible = false;
                gridServicios.IsVisible = true;
                gridEtiquetas.IsVisible = false;

            }

            if (picker.Items[selectedIndex] == "Etiquetas")
            {
                gridProductos.IsVisible = false;
                gridServicios.IsVisible = false;
                gridEtiquetas.IsVisible = true;

            }

        }

    }
}