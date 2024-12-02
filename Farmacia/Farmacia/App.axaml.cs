using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Farmacia.Almacen;
using Farmacia.Almacen.Productos;
using Farmacia.enums;
using Farmacia.Usuarios;
using Farmacia.Ventas;
using System.Collections.Generic;


namespace Farmacia
{
    public partial class App : Application
    {

        public static Farmacia.Sucursal sucursal_Concepcion { get; private set; }
        public static List<Producto> carrito_compras { get; private set; }
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);

            sucursal_Concepcion = new Sucursal("Farmacias Faker", "Luis Durand");
            carrito_compras = new List<Producto>();

        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow();
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}