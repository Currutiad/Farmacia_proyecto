using Avalonia.Controls;
using Avalonia.Interactivity;
using Farmacia.Usuarios;
using System.Transactions;


namespace Farmacia
    {
        public partial class MainWindow : Window
        {
            
        
            public MainWindow()
            {
                InitializeComponent();
        }
            

            //Inicio de Sesion
            public void OnClickIngresar(object sender, RoutedEventArgs args)
            {
                
                var SucursalActual = App.sucursal_Concepcion;

                var empleado = BuscarEmpleado(txtNombre.Text, txtContrasenia.Text, SucursalActual);
                
                if(empleado != null )
                {
                  var registro = new Main();
                  registro.Show();
                  Close();
                }            
            }
            
            public Empleado BuscarEmpleado(string NombreUsuario, string Contrasenia, Sucursal sucursal)
            {
                foreach(var empleadoAUX in sucursal.Empleados)
                {
                    if(empleadoAUX.Nombre == NombreUsuario && empleadoAUX.Contrasenia == Contrasenia)
                    {
                        return empleadoAUX;
                    }
                }
            
                return null;
            }
        

        


            //Creacion de un Usuario en el JSON
            public void OnClickCrear(object sender, RoutedEventArgs args)
            {
                var win = new Window1();
                win.Show();
                Close();
            }
        }

    }
    