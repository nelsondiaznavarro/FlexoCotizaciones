using FlexoCotizaciones.Models;
using System.Text;
using System.Xml.Serialization;

namespace FlexoCotizaciones;

public partial class Cotizaciones : ContentPage
{
    private const string url = "http://dataservice.flexografica.cl/validausuario.php";
    private const string url_parametros = "http://dataservice.flexografica.cl/parametros.php";
    private HttpClient _Client = new HttpClient();
    private CalculoParametros CalculoParametros = new CalculoParametros();

    
    
    public Cotizaciones()
	{
		InitializeComponent();
        limpiar_cotizacion();
        CargarParametros();
        AppShell.ItemCotizacionGlobal.Clear();

        cmdAgregarPar.Clicked += CmdAgregarPar_Clicked;
        cmdGenerarPDF.Clicked += CmdGenerarPDF_Clicked;
        cmdNuevaCotizacion.Clicked += CmdNuevaCotizacion_Clicked;
        cmdCancelarCotizacion.Clicked += CmdCancelarCotizacion_Clicked;
        cmdBuscarCliente.Clicked += CmdBuscarCliente_Clicked;
        txtRutUsuario.Completed += TxtRutUsuario_Completed;
        cmdDescripcionAdicional.Clicked += CmdDescripcionAdicional_Clicked;
        ListaItems.ItemsSource = AppShell.ItemCotizacionGlobal;
        listaTotales.ItemsSource = AppShell.TotalGlobal;
        BuscarClientes("");
        cmbRazonEmpresa.ItemsSource = AppShell.ClientesGlobal;
        cmbRazonEmpresa.SelectedIndexChanged += CmbRazonEmpresa_SelectedIndexChanged;
        //DisplayAlert("Volvio desde agregar items","222","Aceptar");
        //CargaItems();
    }

    private async void CmdDescripcionAdicional_Clicked(object sender, EventArgs e)
    {
        if (AppShell.ItemCotizacionGlobal.Count == 1)
        {
            string resultado = "";

            string result = await DisplayPromptAsync("Descripcion Adicional", "Ingrese descripcion Adicional ");

            if (!string.IsNullOrEmpty(result))
            {
                var httpResponse = await _Client.GetAsync(url_parametros + "?accion=actualizardescripcion&descripcionAdicional=" + result + "&Folio=" + lblFolio.Text + "$rutusuario=" + App.rutConectado);
                if (httpResponse.IsSuccessStatusCode)
                {
                    var responseData = httpResponse.Content.ReadAsStringAsync();
                    resultado = responseData.Result;
                    resultado = "<?xml version='1.0'?>" + resultado;
                }

                if (resultado.IndexOf("<resultado>1</resultado>") > -1)
                {
                    await DisplayAlert("Descripcion Adicional","Decripcion agregada correctamente","Aceptar");
                }

            }
           
        }

    }

    private void CmbRazonEmpresa_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (cmbRazonEmpresa.SelectedItem==null)
        {

            txtRutUsuario.Text = "";
            return;
        }

        Cliente item = (Cliente)cmbRazonEmpresa.SelectedItem;
        App.idClienteSeleccionado = item.Id;

        //buscar datos de cliente con el id

        txtRutUsuario.Text = item.RUT;

    }

    private async void CmdBuscarCliente_Clicked(object sender, EventArgs e)
    {
        string result = await DisplayPromptAsync("Buscar Clientes", "Ingrese R.U.T. o Nombre de Cliente");
        if (string.IsNullOrEmpty(result))
        {
            BuscarClientes("");
        }
        else
        {
            BuscarClientes(result);
        }


    }

    private async void BuscarClientes(string datosBusqueda)
    {
        string resultado = "";
        AppShell.ClientesGlobal.Clear();

        var httpResponse = await _Client.GetAsync(url_parametros + "?accion=buscarClienteRutNombre&rutcliente=" + datosBusqueda + "&RazonSocial=" + datosBusqueda);
        if (httpResponse.IsSuccessStatusCode)
        {
            var responseData = httpResponse.Content.ReadAsStringAsync();
            resultado = responseData.Result;
            resultado = "<?xml version='1.0'?>" + resultado;
        }
        Clientes MisClientes = new Clientes();

        XmlSerializer Serializador = new XmlSerializer(MisClientes.GetType());

        byte[] byteArray = Encoding.ASCII.GetBytes(resultado);
        MemoryStream stream = new MemoryStream(byteArray);

        var MisClientesDes = (Clientes)Serializador.Deserialize(stream);

        foreach (Cliente MiCliente in MisClientesDes.ListaClientes)
        {
            AppShell.ClientesGlobal.Add(new Cliente { Id = MiCliente.Id, RUT = MiCliente.RUT, RazonSocial = MiCliente.RazonSocial, Direccion = MiCliente.Direccion, Comuna = MiCliente.Comuna, Ciudad = MiCliente.Ciudad, Contacto = MiCliente.Contacto, Telefono = MiCliente.Telefono });
        }

    }


    public  void calcular_totales()
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

        //lblSubTotal.Text = subtotal.ToString("###,####,###,###");
        //lblIVA.Text = iva.ToString("###,####,###,###");
        //lblTotal.Text = total.ToString("###,####,###,###");
    }

    private async  void TxtRutUsuario_Completed(object sender, EventArgs e)
    {
        //buscar cliente
        string resultado = "";
        var httpResponse = await _Client.GetAsync(url_parametros + "?accion=buscarcliente&rutcliente=" + txtRutUsuario.Text);
        if (httpResponse.IsSuccessStatusCode)
        {
            var responseData = httpResponse.Content.ReadAsStringAsync();
            resultado = responseData.Result;
            resultado = "<?xml version='1.0'?>" + resultado;
        }
        Cliente MisCliente = new Cliente();

        XmlSerializer Serializador = new XmlSerializer(MisCliente.GetType());

        byte[] byteArray = Encoding.ASCII.GetBytes(resultado);
        MemoryStream stream = new MemoryStream(byteArray);

        Cliente MisClienteDes = (Cliente)Serializador.Deserialize(stream);

        if (string.IsNullOrEmpty(MisClienteDes.RazonSocial.ToString()))
        {
              var respuesta =  await DisplayAlert("Clientes", "No existe cliente,desea crear", "Aceptar","Cancelar");
           
        }
        else
        {
            //idClienteSeleccionado = MisClienteDes.Id;
            App.idClienteSeleccionado = MisClienteDes.Id;
            //txtRazonEmpresa.Text = MisClienteDes.RazonSocial.ToString();
           
        }
        
    }

    private void CmdCancelarCotizacion_Clicked(object sender, EventArgs e)
    {
        limpiar_cotizacion();
    }

    private void limpiar_cotizacion()
    {
        AppShell.ItemCotizacionGlobal.Clear();
        AppShell.CotizacionGlobal.Clear();
        AppShell.ClientesGlobal.Clear();
        AppShell.TotalGlobal.Clear();
        AppShell.TotalGlobal.Add(new Total { Nombre = "SubTotal", Valor = 0 });
        AppShell.TotalGlobal.Add(new Total { Nombre = "% Tasa IVA", Valor = 19 });
        AppShell.TotalGlobal.Add(new Total { Nombre = "IVA", Valor = 0 });
        AppShell.TotalGlobal.Add(new Total { Nombre = "Total", Valor = 0 });
        lblFolio.Text = "0";
        txtRutUsuario.Text = "";
        BuscarClientes("");
        //txtRazonEmpresa.Text = "";
        //lblSubTotal.Text = "0";
        //lblIVA.Text = "0";
        //lblTotal.Text = "0";


    }
    private async void CmdNuevaCotizacion_Clicked(object sender, EventArgs e)
    {
        //buscar un folio folios
        limpiar_cotizacion();

        string resultado = "";

        var httpResponse = await _Client.GetAsync(url_parametros + "?accion=obtenerfolio&rutusuario=" + App.rutConectado);
        if (httpResponse.IsSuccessStatusCode)
        {
            var responseData = httpResponse.Content.ReadAsStringAsync();
            resultado = responseData.Result;
            resultado = "<?xml version='1.0'?>" + resultado;
        }
        Folios MisFolios = new Folios();

        XmlSerializer Serializador = new XmlSerializer(MisFolios.GetType());

        byte[] byteArray = Encoding.ASCII.GetBytes(resultado);
        MemoryStream stream = new MemoryStream(byteArray);

       var  MisFoliosDes = (Folios)Serializador.Deserialize(stream);
        lblFolio.Text = MisFoliosDes.Folio.ToString();
    }

    private async void CmdGenerarPDF_Clicked(object sender, EventArgs e)
    {
        try
        {
            //Guardar los datos en la base
            //?accion=ingresacotizacion&Folio=0006&Fecha=2023-08-11&idCliente=1&FechaVencimiento=2023-08-30&Usuario=9121&Estado=1&CondicionPago=30%20dias

            if (string.IsNullOrEmpty(txtRutUsuario.Text) ||  AppShell.ItemCotizacionGlobal.Count < 1 || lblFolio.Text =="0")
            {
                await DisplayAlert("Cotizaciones", "No se puede emitir cotizacion sin Cliente, Items o Folio 0", "Aceptar");
                return;
            }

            string resultado = "";
            
            var subtotal = AppShell.TotalGlobal.FirstOrDefault();
            var Iva = AppShell.TotalGlobal.ElementAt(2) ;
            var Total = AppShell.TotalGlobal.ElementAt(3);
            string consulta = url_parametros + "?accion=ingresacotizacion&Folio=" + int.Parse(lblFolio.Text).ToString("####") + "&Fecha=" + txtFecha.Date.ToString("yyyy-MM-dd") + "&idCliente=" + App.idClienteSeleccionado.ToString() + "&FechaVencimiento=" + txtFecha.Date.ToString("yyyy-MM-dd") + "&Usuario=" + App.rutConectado + "&Estado=0&CondicionPago=" + cboCondicion.SelectedItem + "&Neto=" + subtotal.Valor.ToString() + "&Iva=" + Iva.Valor.ToString() + "&Total=" + Total.Valor.ToString() + "&PorcComision=" + CalculoParametros.ComisionDefecto + "&ValorComision=" + (double.Parse(subtotal.Valor.ToString()) * (CalculoParametros.ComisionDefecto / 100));
            var httpResponse = await _Client.GetAsync(url_parametros + "?accion=ingresacotizacion&Folio="+int.Parse(lblFolio.Text).ToString("####")+"&Fecha=" + txtFecha.Date.ToString("yyyy-MM-dd") + "&idCliente="+ App.idClienteSeleccionado.ToString()+ "&FechaVencimiento=" + txtFecha.Date.ToString("yyyy-MM-dd") + "&Usuario="+App.rutConectado + "&Estado=0&CondicionPago=" + cboCondicion.SelectedItem + "&Neto=" + subtotal.Valor.ToString() + "&Iva=" + Iva.Valor.ToString() + "&Total=" + Total.Valor.ToString() + "&PorcComision=" + CalculoParametros.ComisionDefecto + "&ValorComision=" + (double.Parse(subtotal.Valor.ToString()) * (CalculoParametros.ComisionDefecto/100)));
            if (httpResponse.IsSuccessStatusCode)
            {
                var responseData = httpResponse.Content.ReadAsStringAsync();
                resultado = responseData.Result;
                resultado = "<?xml version='1.0'?>" + resultado;
            }


            //guardar detalle
            foreach (ItemCotizacion miitem in AppShell.ItemCotizacionGlobal)
            {
                //Descripcion,UM,Cantidad,ValorUnitario,ValorTotal,Id_Cotizaciones
                consulta = url_parametros + "?accion=ingresadetallecotizacion&Descripcion=" + miitem.Descripcion + "&UM=" + miitem.UM + "&Cantidad=" + miitem.Cantidad + "&ValorUnitario=" + miitem.Valor + "&ValorTotal=" + miitem.Total + "&Id_Cotizaciones=" + lblFolio.Text;
                httpResponse = await _Client.GetAsync(consulta);
            }

            //Despues de guardar encabezado y detalle actualziar el folio

            httpResponse = await _Client.GetAsync(url_parametros + "?accion=actualizarfolio&rutusuario=" + App.rutConectado);
            if (httpResponse.IsSuccessStatusCode)
            {
                var responseData = httpResponse.Content.ReadAsStringAsync();
                resultado = responseData.Result;
                resultado = "<?xml version='1.0'?>" + resultado;
            }

          



            Uri uri = new Uri("http://dataservice.flexografica.cl/reporte.php?rutusuario=" + App.rutConectado + "&folio=" + lblFolio.Text);
            await Browser.Default.OpenAsync(uri, BrowserLaunchMode.SystemPreferred);

            //limpiar cotizacion

            limpiar_cotizacion();

            AppShell.ItemCotizacionGlobal.Clear();
        }
        catch (Exception ex)
        {
            // An unexpected error occurred. No browser may be installed on the device.
        }
    }

    private  void CargaItems()
    {
        //cmdAgregarPar.Clicked += CmdAgregarPar_Clicked;
        //List<ItemCotizacion> itemCotizacions = new List<ItemCotizacion>();

        //itemCotizacions.Add(new ItemCotizacion() { Id = 0, Descripcion = "Etiquetas medida 180x200mm pp transparente , impresas a 6 colores", Cantidad = 3000, UM = "UNI", Valor = 199, Total = 597000 });
        //itemCotizacions.Add(new ItemCotizacion() { Id = 0, Descripcion = "Item 1", Cantidad = 1, UM = "UNI", Valor = 100, Total = 100 });
        //itemCotizacions.Add(new ItemCotizacion() { Id = 0, Descripcion = "Item 1", Cantidad = 1, UM = "UNI", Valor = 100, Total = 100 });
        //itemCotizacions.Add(new ItemCotizacion() { Id = 0, Descripcion = "Item 1", Cantidad = 1, UM = "UNI", Valor = 100, Total = 100 });
        //itemCotizacions.Add(new ItemCotizacion() { Id = 0, Descripcion = "Item 1", Cantidad = 1, UM = "UNI", Valor = 100, Total = 100 });
        //itemCotizacions.Add(new ItemCotizacion() { Id = 0, Descripcion = "Item 1", Cantidad = 1, UM = "UNI", Valor = 100, Total = 100 });
        //itemCotizacions.Add(new ItemCotizacion() { Id = 0, Descripcion = "Item 1", Cantidad = 1, UM = "UNI", Valor = 100, Total = 100 });
        //itemCotizacions.Add(new ItemCotizacion() { Id = 0, Descripcion = "Item 1", Cantidad = 1, UM = "UNI", Valor = 100, Total = 100 });
        //itemCotizacions.Add(new ItemCotizacion() { Id = 0, Descripcion = "Item 1", Cantidad = 1, UM = "UNI", Valor = 100, Total = 100 });
        //itemCotizacions.Add(new ItemCotizacion() { Id = 0, Descripcion = "Item 1", Cantidad = 1, UM = "UNI", Valor = 100, Total = 100 });
        //itemCotizacions.Add(new ItemCotizacion() { Id = 0, Descripcion = "Item 1", Cantidad = 1, UM = "UNI", Valor = 100, Total = 100 });
        //itemCotizacions.Add(new ItemCotizacion() { Id = 0, Descripcion = "Item 1", Cantidad = 1, UM = "UNI", Valor = 100, Total = 100 });
        //itemCotizacions.Add(new ItemCotizacion() { Id = 0, Descripcion = "Item 1", Cantidad = 1, UM = "UNI", Valor = 100, Total = 100 });
        //itemCotizacions.Add(new ItemCotizacion() { Id = 0, Descripcion = "Item 1", Cantidad = 1, UM = "UNI", Valor = 100, Total = 100 });
        //itemCotizacions.Add(new ItemCotizacion() { Id = 0, Descripcion = "Item 1", Cantidad = 1, UM = "UNI", Valor = 100, Total = 100 });
        //itemCotizacions.Add(new ItemCotizacion() { Id = 0, Descripcion = "Item 1", Cantidad = 1, UM = "UNI", Valor = 100, Total = 100 });
        //itemCotizacions.Add(new ItemCotizacion() { Id = 0, Descripcion = "Item 1", Cantidad = 1, UM = "UNI", Valor = 100, Total = 100 });
        //itemCotizacions.Add(new ItemCotizacion() { Id = 0, Descripcion = "Item 1", Cantidad = 1, UM = "UNI", Valor = 100, Total = 100 });
        //itemCotizacions.Add(new ItemCotizacion() { Id = 0, Descripcion = "Item 1", Cantidad = 1, UM = "UNI", Valor = 100, Total = 100 });
        //itemCotizacions.Add(new ItemCotizacion() { Id = 0, Descripcion = "Item 1", Cantidad = 1, UM = "UNI", Valor = 100, Total = 100 });
        //itemCotizacions.Add(new ItemCotizacion() { Id = 0, Descripcion = "Item 1", Cantidad = 1, UM = "UNI", Valor = 100, Total = 100 });
        //itemCotizacions.Add(new ItemCotizacion() { Id = 0, Descripcion = "Item 1", Cantidad = 1, UM = "UNI", Valor = 100, Total = 100 });
        //itemCotizacions.Add(new ItemCotizacion() { Id = 0, Descripcion = "Item 1", Cantidad = 1, UM = "UNI", Valor = 100, Total = 100 });
        //itemCotizacions.Add(new ItemCotizacion() { Id = 0, Descripcion = "Item 1", Cantidad = 1, UM = "UNI", Valor = 100, Total = 100 });
        //ListaItems.ItemsSource = itemCotizacions;

    }

   

    private void CmdAgregarPar_Clicked(object sender, EventArgs e)
    {
        var resultado =  Mopups.Services.MopupService.Instance.PushAsync(new AgregarItemPopup());
        calcular_totales();
    }

    private async void CargarParametros()
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

        CalculoParametros = (CalculoParametros)Serializador.Deserialize(stream);

        //ListaUsuariosFront.ItemsSource = MisUsuariosDes.ListaUsuarios;
    }
}