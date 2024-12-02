using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//Espacios de nombre
using Farmacia.Almacen;
using Farmacia.Almacen.Productos;
using Farmacia.enums;
using Farmacia.Usuarios;

/*

Diferencias UML:

 *Descuento:
  - Solo deje la clase con los atributos ID y %_descuento. Elimine la funcion ya que el descuento lo aplique directamente en el precio final de la venta

 *Venta:
  - Solo quite el atributo unidades
 
 *Boleta:
  -Deje el atributo Codigo, pero agregue como atributo que debe tener una clase Venta. Ya que sin Venta no hay boleta. 
  - No hice el espacio de nombre Usuarios por lo que aun falta para terminar la clase Boleta
 
 
 */


namespace Farmacia
{
    namespace Ventas
    {

        //Creacion de la clase Descuento, la cual ve el dia actual y le asigna un % dependiendo de que día es.
        public class Descuento
        {
            private int id_descuento;
            private double p_descuento;

            public int Id_descuento { get => id_descuento; set => id_descuento = value; }
            public double P_Descuento { get => p_descuento; set => p_descuento = value; }

            public Descuento(int id)
            {
                this.id_descuento = id;
                this.p_descuento = DiDescuento(DateTime.Now);
            }

            public Descuento()  //Esto sirve en cualquier caso y te crea un objeto Descuento con id=0 y el % correspondiente al dia de la semana
            {
                this.p_descuento = DiDescuento(DateTime.Now);
            }

            public double DiDescuento(DateTime dia)
            {
                DayOfWeek diaAux = dia.DayOfWeek;

                switch (diaAux)
                {
                    case DayOfWeek.Monday:
                        return 0.1;
                    case DayOfWeek.Tuesday:
                        return 0.05;
                    case DayOfWeek.Wednesday:
                        return 0.08;
                    case DayOfWeek.Thursday:
                        return 0.2;
                    case DayOfWeek.Friday:
                        return 0.15;
                    case DayOfWeek.Saturday:
                        return 0.05;
                    case DayOfWeek.Sunday:
                        return 0.12;
                    default:
                        return 0;
                }

            }
        }


        public class Venta
        {
            private int id_venta;
            private double precio;
            private MetodoPago_t metodo;
            private List<Producto> productos = new List<Producto>();
            private DateTime fecha;
            private Inventario inventario = new Inventario();

            public int Id_venta { get => id_venta; set => id_venta = value; }
            public MetodoPago_t Metodo { get => metodo; set => metodo = value; }
            public List<Producto> Productos { get => productos; set => productos = value; }
            public DateTime Fecha { get => fecha; set => fecha = value; }
            public double Precio { get => precio; set => precio = value; }
            public Inventario Inventario { get => inventario; set => inventario = value; }

            public Venta(int id, MetodoPago_t metodo, List<Producto> prod)
            {
                this.id_venta = id;
                this.metodo = metodo;
                this.productos = prod;
                this.fecha = DateTime.Now;
                this.precio = CalcularTotal();
            }

            public Venta(MetodoPago_t metodo, List<Producto> prod)
            {
                this.metodo = metodo;
                this.productos = prod;
                this.fecha = DateTime.Now;
                this.precio = CalcularTotal();
            }

            public Venta() { }

            public double PrecioSinDescuento()
            {
                double acum = 0;

                foreach (var prod in Productos)
                {
                    acum += prod.Precio;
                }

                return acum;
            }

            public void RealizarVenta()
            {
                foreach (var prod in productos)
                {
                    inventario.EliminarProducto(prod);
                }
            }

            public double CalcularTotal()
            {
                double contador = 0;

                foreach (var item in productos)
                {
                    contador += item.Precio;
                }

                Descuento descuento = new Descuento();

                return (contador - (contador * descuento.P_Descuento));
            }

            public void AgregarProducto(Producto producto)
            {
                Productos.Add(producto);
                Precio = CalcularTotal();
            }

            public void EliminarProducto(Producto prod)
            {
                Productos.RemoveAll(p => p == prod);
                Precio = CalcularTotal();
            }
        }



        public interface IBoleta
        {
            public void MostrarTotal();
            public void MostrarVenta();
            public void MostrarCliente();
            public void MostrarBoleta();
            public void MostrarDescuento();

        }
        public class Boleta
        {
            private int id_compra;
            Venta venta = new Venta();
            Cliente cliente_;


            public int Id_compra { get => id_compra; set => id_compra = value; }
            public Venta Venta { get => venta; set => venta = value; }
            public Cliente Cliente_ { get => cliente_; set => cliente_ = value; }

            public Boleta(int id, Venta venta, Cliente cliente)
            {
                this.Id_compra = id;
                this.venta = venta;
                this.cliente_ = cliente;

                Venta.RealizarVenta();
            }

            public Boleta(Venta venta, Cliente cliente)
            {
                this.venta = venta;
                this.cliente_ = cliente;

                Venta.RealizarVenta();
            }

            public double MostrarTotal()
            {
                return venta.CalcularTotal();
            }

            public void MostrarCliente()
            {
                Console.WriteLine($"Nombre de cliente: {Cliente_.Nombre}");
            }

            public void MostrarBoleta()
            {
                Console.WriteLine($"ID: {Id_compra}");

                Console.WriteLine("Productos:");
                foreach (var item in venta.Productos)
                {
                    Console.WriteLine($"{item.Nombre} --- ${item.Precio}");
                }
                Console.WriteLine("");
            }

            public string MostrarDescuento()
            {
                Descuento descuento = new Descuento();
                return $"Descuento: {descuento.P_Descuento * 100}%\n";
            }
        }


    }
}
