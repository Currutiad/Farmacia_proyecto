using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using System;
using Farmacia.enums;
using Avalonia.Media;
using Farmacia.Almacen.Productos;
using Farmacia.Almacen;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using Farmacia.Usuarios;
using Farmacia.Ventas;

namespace Farmacia;

public partial class Main : Window
{
    public List<Producto> _Productos { get; set; }

    public Main()
    {
        InitializeComponent();

        // Inicializa la lista con algunos datos de ejemplo
        _Productos = App.sucursal_Concepcion.Inventario.Productos_;
        // Enlaza la lista al ListBox
        ListProductos.Items.Clear();
        ListProductos.ItemsSource = _Productos;
        App.carrito_compras.Clear();
    }


    public bool _req = false;
    private void RadioButton_Checked(object? sender, RoutedEventArgs e)
    {
        if (sender is RadioButton radioButton)
        {
            // Si el contenido es "Sí", mostramos los controles adicionales
            if (radioButton.Content?.ToString() == "Sí")
            {
                ControlesReceta.IsVisible = true;
                _req = true;
            }
            else
            {
                // Si el contenido es "No", ocultamos los controles adicionales
                ControlesReceta.IsVisible = false;
            }
        }
    }

    private piel_t tipoPielSeleccionado; // Variable para almacenar el tipo de piel seleccionado

    private void TipoPiel_Checked(object? sender, RoutedEventArgs e)
    {
        if (sender is RadioButton radioButton)
        {
            // Convertir el contenido del RadioButton al enum TipoPiel
            tipoPielSeleccionado = Enum.Parse<piel_t>(radioButton.Content.ToString());
        }
    }

    public int TransformarEntradaINT(string dato)
    {
        if (int.TryParse(dato, out int precio))
        {
            return precio;
        }
        else
        {
            return 0;
        }
    }
    public double TransformarEntradaDOUBLE(string dato)
    {
        if (double.TryParse(dato, out double precio))
        {
            return precio;
        }
        else
        {
            return 0;
        }
    }


    public void OnClickAgregarMedicamento(object sender, RoutedEventArgs args)
    {
        var medicamento = new Medicamento
        {
            Id = TransformarEntradaINT(MED_ID.Text),
            Nombre = MED_NAME.Text,
            Precio = TransformarEntradaDOUBLE(MED_PRICE.Text),
            Peso = TransformarEntradaDOUBLE(MED_PESO.Text),
            Laboratorio = MED_LAB.Text,
            Bioequivalente = MED_BIO.Text,
            Req_Receta = _req
        };
        if (_req)
        {
            var aux = TransformarEntradaINT(RECETA_ID.Text);
            var receta = new Receta();
            medicamento.AgregarReceta(receta, aux, RECETA_MED.Text, RECETA_FECHA.SelectedDate.Value.DateTime);
        }

        App.sucursal_Concepcion.Inventario.AgregarProducto(medicamento);

        ResetMedicamentos();
    }

    public void ResetMedicamentos()
    {
        MED_ID.Clear();
        MED_NAME.Clear();
        MED_LAB.Clear();
        MED_BIO.Clear();
        MED_PESO.Clear();
        MED_PRICE.Clear();
        RECETA_FECHA.Clear();
        RECETA_ID.Clear();
        RECETA_MED.Clear();
    }

    public void OnClickAgregarDermocosmetico(object sender, RoutedEventArgs args)
    {
        var dermocosmetico = new Dermocosmetico
        {
            Id = TransformarEntradaINT(DERM_ID.Text),
            Nombre = DERM_NAME.Text,
            Precio = TransformarEntradaDOUBLE(DERM_PRICE.Text),
            Peso = TransformarEntradaDOUBLE(DERM_PESO.Text),
            Tipo_piel = tipoPielSeleccionado
        };

        App.sucursal_Concepcion.Inventario.AgregarProducto(dermocosmetico);
        ResetDermocosmeticos();
    }

    public void ResetDermocosmeticos()
    {
        DERM_ID.Clear();
        DERM_NAME.Clear();
        DERM_PRICE.Clear();
        DERM_PESO.Clear();
    }

    private MetodoPago_t metodo; // Variable para almacenar el tipo de piel seleccionado

    private void TipoDePago_Checked(object? sender, RoutedEventArgs e)
    {
        if (sender is RadioButton radioButton)
        {
            // Convertir el contenido del RadioButton al enum TipoPiel
            metodo = Enum.Parse<MetodoPago_t>(radioButton.Content.ToString());
        }
    }

    public void OnClickFinalizarAtencion(object sender, RoutedEventArgs args)
    {
        var cliente = new Cliente
        {
            Rut = TransformarEntradaINT(CLIENTE_RUT.Text),
            Nombre = CLIENTE_NOMBRE.Text,
            Fecha_nacimiento = Fecha_nacimiento_cliente.SelectedDate.Value.DateTime
        };

        App.sucursal_Concepcion.IngresarRegistro(cliente);

        var venta = new Venta(metodo, App.carrito_compras);

        var boleta = new Boleta(venta, cliente);

        BoletaFinal.Text = CompraRealizada(boleta);
    }

    public string CompraRealizada(Boleta boleta)
    {
        return $"Nombre Cliente: {boleta.Cliente_.Nombre}\n" +
               $"Metodo de Pago: {boleta.Venta.Metodo}\n" +
               $"Precio sin Descuento: {boleta.Venta.PrecioSinDescuento()}\n" +
               boleta.MostrarDescuento() +
               $"Precio Final: {boleta.MostrarTotal()}\n" +
               $"Sucursal: {App.sucursal_Concepcion.Nombre}\n" +
               $"Fecha de Compra: {boleta.Venta.Fecha}";
    }



    public void OnClickCerrarSesion(object sender, RoutedEventArgs args)
    {
        var win = new MainWindow();
        win.Show();
        Close();
    }

    public void OnClickAgregarProductoCarrito(object sender, RoutedEventArgs args)
    {
        var prod = new Producto();
        prod.Id = TransformarEntradaINT(PROD_ID.Text);
        prod.Nombre = PROD_NAME.Text;

        foreach (var productoAUX in App.sucursal_Concepcion.Inventario.Productos_)
        {
            if (productoAUX.Id == prod.Id)
            {
                prod.Precio = productoAUX.Precio;
                App.carrito_compras.Add(prod);
                break;
            }
        }
        CarritoCliente.Text = MostrarInformacion(App.carrito_compras);
        CarritoFinal.Text = MostrarInformacion(App.carrito_compras);


        ResetBuscar();
    }

    public string MostrarInformacion(List<Producto> lista)
    {
        double precioACUM = 0;
        foreach (var producto in lista)
        {
            precioACUM += producto.Precio;
        }

        return $"Productos: {lista.Count}" + $"  Precio a Pagar: {precioACUM}";
    }

    public void ResetBuscar()
    {
        PROD_ID.Clear();
        PROD_NAME.Clear();
    }




}