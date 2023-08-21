using FlexoCotizaciones.Models;
using System.Text;
using System.Xml.Serialization;

namespace FlexoCotizaciones;

public partial class HistorialCotizacionesV1 : ContentPage
{
    private const string url = "http://dataservice.flexografica.cl/validausuario.php";
    private const string url_parametros = "http://dataservice.flexografica.cl/parametros.php";
    private HttpClient _Client = new HttpClient();
    private CotizacionesConsulta MisCotizaciones = new CotizacionesConsulta();
    
    public HistorialCotizacionesV1()
	{
		InitializeComponent();
        // cmdBuscarCotizaciones.Clicked += CmdBuscarCotizaciones_Clicked;
        //ListaConsultaCotizaciones.ItemsSource = MisCotizaciones;
        cmdBuscarCotizaciones.Clicked += CmdBuscarCotizaciones_Clicked1;
        BuscarClientes("");
        cmbRazonEmpresa.ItemsSource = AppShell.ClientesGlobal;
        cmbRazonEmpresa.SelectedIndexChanged += CmbRazonEmpresa_SelectedIndexChanged;
        cmdLimpiarBusqueda.Clicked += CmdLimpiarBusqueda_Clicked;

    }

    private void CmdLimpiarBusqueda_Clicked(object sender, EventArgs e)
    {
        cmbRazonEmpresa.ItemsSource = null;
        cmbRazonEmpresa.ItemsSource = AppShell.ClientesGlobal;
        txtRutUsuario.Text = "";
        ListaConsultaCotizaciones.ItemsSource = null;
        txtBusquedaFolio.Text = "";
    }

    private void CmbRazonEmpresa_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (cmbRazonEmpresa.SelectedItem == null)
        {

            txtRutUsuario.Text = "";
            return;
        }

        Cliente item = (Cliente)cmbRazonEmpresa.SelectedItem;
        App.idClienteSeleccionado = item.Id;

        //buscar datos de cliente con el id

        txtRutUsuario.Text = item.RUT;

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


    private async void CmdPdfCotizaciones_Clicked(object sender, EventArgs e)
    {
        if (ListaConsultaCotizaciones.SelectedItem == null)
        {
            await DisplayAlert("Historial", "Debe seleccionar la fila para  re imprimir PDF", "Aceptar");
            return;
        }
        else

        {

            CotizacionConsulta MiCotizacion = (CotizacionConsulta)ListaConsultaCotizaciones.SelectedItem;
            Uri uri = new Uri("http://dataservice.flexografica.cl/reporte.php?rutusuario=" + App.rutConectado + "&folio=" + MiCotizacion.Folio);
            await Browser.Default.OpenAsync(uri, BrowserLaunchMode.SystemPreferred);
        }

       

    }

    private async void CmdCopiarCotizaciones_Clicked(object sender, EventArgs e)
    {
    }

    private async void CmdBuscarCotizaciones_Clicked1(object sender, EventArgs e)
    {
        if ((cmbRazonEmpresa.SelectedItem)==null && string.IsNullOrEmpty(txtRutUsuario.Text) && string.IsNullOrEmpty(txtBusquedaFolio.Text) && CheckIncluirFechas.IsChecked == false)
        {

            await DisplayAlert("Busqueda", "Debe ingresar R.U.T., Nombre o Fechas para generar la busqueda", "Aceptar");
            return;
        }
        else
        {

            string query = "select C.*, DC.Id as Id_Detalle ,DC.Descripcion,DC.UM,DC.Cantidad,DC.ValorUnitario,DC.ValorTotal, DC.id_Cotizaciones, CL.RUT, CL.RazonSocial  from Cotizaciones C,Detalle_Cotizaciones DC,Clientes CL  where ";
            query += " C.Folio = DC.id_Cotizaciones and C.idCliente = CL.Id ";
            int FolioBusqueda = 0;
            if (int.TryParse(txtBusquedaFolio.Text, out FolioBusqueda))
            {
                query += " and C.Folio =" + FolioBusqueda.ToString() + " ";
            }

            if ((cmbRazonEmpresa.SelectedItem) != null)
            {
                query += " and C.idCliente =" + App.idClienteSeleccionado + " ";
            }


            if (CheckIncluirFechas.IsChecked)
            {
                if (dpFechaDesde.Date > dpFechaHasta.Date)
                {
                    await DisplayAlert("Historial", "Fecha hasta debe ser mayor o igual que Fecha desde", "Aceptar");
                    return;
                }


                query += " and C.Fecha between '" + dpFechaDesde.Date.ToString("yyyy-MM-dd") + "' and '" + dpFechaHasta.Date.ToString("yyyy-MM-dd") + "'";
            }




            ListaConsultaCotizaciones.ItemsSource = null;
            string resultado = "";

            var httpResponse = await _Client.GetAsync(url_parametros + "?accion=consultacotizacionesquery&query=" + query);
            if (httpResponse.IsSuccessStatusCode)
            {
                var responseData = httpResponse.Content.ReadAsStringAsync();
                resultado = responseData.Result;
                resultado = "<?xml version='1.0'?>" + resultado;
            }
            CotizacionesConsulta MisCot = new CotizacionesConsulta();

            XmlSerializer Serializador = new XmlSerializer(MisCot.GetType());

            byte[] byteArray = Encoding.ASCII.GetBytes(resultado);
            MemoryStream stream = new MemoryStream(byteArray);

            CotizacionesConsulta MisPapeles = (CotizacionesConsulta)Serializador.Deserialize(stream);
            ListaConsultaCotizaciones.ItemsSource = MisPapeles.ListaCotizaciones;

        }
    }

    private async void CmdBuscarCotizaciones_Clicked(object sender, EventArgs e)
    {
        if (cmbRazonEmpresa.SelectedItem==null && string.IsNullOrEmpty(txtRutUsuario.Text) && string.IsNullOrEmpty(txtBusquedaFolio.Text) && CheckIncluirFechas.IsChecked == false)
        {

            await DisplayAlert("Busqueda", "Debe ingresar R.U.T., Nombre o Fechas para generar la busqueda", "Aceptar");
            return;
        }
        else
        {
            ListaConsultaCotizaciones.ItemsSource = null;
            string resultado = "";

            var httpResponse = await _Client.GetAsync(url_parametros + "?accion=consultacotizaciones&Folio=" + txtBusquedaFolio.Text);
            if (httpResponse.IsSuccessStatusCode)
            {
                var responseData = httpResponse.Content.ReadAsStringAsync();
                resultado = responseData.Result;
                resultado = "<?xml version='1.0'?>" + resultado;
            }
            CotizacionesConsulta MisCot = new CotizacionesConsulta();

            XmlSerializer Serializador = new XmlSerializer(MisCot.GetType());

            byte[] byteArray = Encoding.ASCII.GetBytes(resultado);
            MemoryStream stream = new MemoryStream(byteArray);

            CotizacionesConsulta MisPapeles = (CotizacionesConsulta)Serializador.Deserialize(stream);
            ListaConsultaCotizaciones.ItemsSource = MisPapeles.ListaCotizaciones;

        }


        
    }



}