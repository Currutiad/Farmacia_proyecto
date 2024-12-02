using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Farmacia.enums;
using Farmacia.Usuarios;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Farmacia
{
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
        }

        public void OnClickRegistrarse(object sender, RoutedEventArgs args)
        {
            try
            {
                var empleado = new Empleado();

                if (VerificarDatos())
                {
                    // Asignación de propiedades del empleado
                    empleado.Nombre = txtUsername.Text;
                    empleado.Contrasenia = txtPassword.Text;

                    // Verifica si la fecha seleccionada es válida
                    if (Fecha_Nacimiento.SelectedDate.HasValue)
                    {
                        empleado.Fecha_nacimiento = Fecha_Nacimiento.SelectedDate.Value.DateTime;
                    }
                    else
                    {
                        throw new InvalidOperationException("Debe seleccionar una fecha de nacimiento válida.");
                    }

                    // Registro en la sucursal
                    App.sucursal_Concepcion.IngresarRegistro(empleado);

                    // Abre la ventana principal
                    var win = new MainWindow();
                    win.Show();

                    // Cierra la ventana actual
                    Close();
                }
            }
            catch
            {

            }
        }


        //Arreglar los condicionamientos

        public bool VerificarDatos()
        {
            bool PuedeRegistrarse = !string.IsNullOrEmpty(txtUsername.Text) && !string.IsNullOrEmpty(txtPassword.Text) && !string.IsNullOrEmpty(txtPasswordAUX.Text) &&
                               txtPassword.Text == txtPasswordAUX.Text;

            return PuedeRegistrarse;
        }
        public void OnclickVolver(object sender, RoutedEventArgs args)
        {
            var win = new MainWindow();
            win.Show();
            Close();
        }

        //Arreglar los condicionamientos
        bool PuedeRegistrarse;
        public void VerificarDatos2()
        {
            PuedeRegistrarse = !string.IsNullOrEmpty(txtUsername.Text) && !string.IsNullOrEmpty(txtPassword.Text) && !string.IsNullOrEmpty(txtPasswordAUX.Text) &&
                               txtPassword.Text == txtPasswordAUX.Text && Fecha_Nacimiento.SelectedDate.HasValue;
        }
    }



}

