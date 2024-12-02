using Farmacia.Almacen;
using Farmacia.Almacen.Productos;
using Farmacia.enums;
using Farmacia.Usuarios;
using Farmacia.Ventas;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
/*
 * POR HACER => PROCESO DE VENTA() Y TERMINAR VENTA()
 * 
 * 
 Diferencias UML:

 *Interfaces => Cambie la aplicacion de las interfaces a SUCURSAL debido a que es más practico tener las funciones ahí, 
                ya que la lista de empleados y de clientes las tiene la sucursal.
 
 
 *Cliente => solo le agregue que debe tener una contraseña y que le "tipoCliente" sea un atributo opcional en la creacion del cliente.
 
 */



namespace Farmacia
{

    public interface IRegistro
    {
        public void IngresarRegistro();
    }

    public interface II_Sesion
    {
        public bool IniciarSesion();

    }

    public class Sucursal : IRegistro, II_Sesion
    {
        private string nombre = string.Empty;
        private string direccion = string.Empty;
        private List<Empleado> empleados = new List<Empleado>();
        private List<Cliente> clientes = new List<Cliente>();
        private Inventario inventario = new Inventario();

        public string Nombre { get => nombre; set => nombre = value; }
        public string Direccion { get => direccion; set => direccion = value; }
        public List<Empleado> Empleados { get => empleados; set => empleados = value; }
        public List<Cliente> Clientes { get => clientes; set => clientes = value; }
        public Inventario Inventario { get => inventario; set => inventario = value; }

        public Sucursal()
        {
            this.empleados = CargarDatos_Empleado();
            this.clientes = CargarDatos_Clientes();
        }


        public Sucursal(string nombre, string direccion)
        {
            this.empleados = CargarDatos_Empleado();
            this.clientes = CargarDatos_Clientes();
            this.nombre = nombre;
            this.direccion = direccion;
        }

        public Producto BuscarProducto(Producto producto)
        {
            foreach (var prod in inventario.Productos_)
            {
                if (prod.Id == producto.Id)
                {
                    return prod;
                }
            }
            throw new Exception("Producto no encontrado");
        }






        #region proceso de Json

        //Empleados, Guardado de los datos dentro del json respectivo. Realizando sobrecarga a la funcion ActualizarDatos que accede al archivo dependiendo de si es cliente o empleado.
        public List<Empleado> CargarDatos_Empleado()
        {
            string datos = string.Empty, rutajson = "Registro_Empleados.json";

            using (var lector = new StreamReader(rutajson))
            {
                datos = lector.ReadToEnd();
            }

            var empleados = JsonConvert.DeserializeObject<List<Empleado>>(datos);

            return empleados ?? new List<Empleado>();
        }

        public void ActualizarDatos(List<Empleado> list_empleados)
        {
            string empleados = JsonConvert.SerializeObject(list_empleados.ToArray(), Formatting.Indented);
            File.WriteAllText("Registro_Empleados.json", empleados);
        }

        //Clientes, Estas funciones permiten el registro de los clientes y el guardado de sus de datos en el archivo json correspondiente.
        public List<Cliente> CargarDatos_Clientes()
        {
            string datos = string.Empty, rutajson = "Registro_Clientes.json";

            using (var lector = new StreamReader(rutajson))
            {
                datos = lector.ReadToEnd();
            }

            var clientes = JsonConvert.DeserializeObject<List<Cliente>>(datos);

            return clientes ?? new List<Cliente>();
        }

        public void ActualizarDatos(List<Cliente> list_clientes)
        {
            string clientes = JsonConvert.SerializeObject(list_clientes.ToArray(), Formatting.Indented);
            File.WriteAllText("Registro_Clientes.json", clientes);
        }
        #endregion





        #region Aplicacion del Registro e Inicio de Sesion, con sobrecarga de los metodos de la interfaz.

        //Cumplimos con las funciones a heredar, pero realizamos sobrecarga para diferenciar el proceso para un cliente y un empleado
        public void IngresarRegistro() { }
        public bool IniciarSesion() { return false; }


        //PROCESO EMPLEADO

        public void IngresarRegistro(Empleado empleado)
        {
            Empleados.Add(empleado);
            ActualizarDatos(Empleados);
        }

        public bool IniciarSesion(Empleado empleado)
        {
            foreach (var empleadoAUX in Empleados)
            {
                if (empleadoAUX.Id == empleado.Id)
                {
                    return true;
                }
            }

            return false;
        }

        public void Despedir(Empleado empleado)
        {
            Empleados.RemoveAll(p => p.Id == empleado.Id);
            ActualizarDatos(Empleados);
        }

        //PROCESO CLIENTE

        public void IngresarRegistro(Cliente cliente)
        {
            Clientes.Add(cliente);
            ActualizarDatos(Clientes);
        }

        public bool IniciarSesion(Cliente cliente)
        {
            foreach (var clienteAUX in Clientes)
            {
                if (clienteAUX.Id == cliente.Id)
                {
                    return true;
                }
            }

            return false;
        }

        public void EliminarCliente(Cliente cliente)
        {
            Clientes.RemoveAll(p => p.Id == cliente.Id);
            ActualizarDatos(Clientes);
        }

        #endregion


    }




    namespace Usuarios
    {

        public class Persona
        {
            private int rut;
            private int verificador;
            private string nombre = string.Empty;
            private string apellido_paterno = string.Empty;
            private DateTime fecha_nacimiento = new DateTime();

            public int Rut { get => rut; set => rut = value; }
            public int Verificador { get => verificador; set => verificador = value; }
            public string Nombre { get => nombre; set => nombre = value; }
            public string Apellido_paterno { get => apellido_paterno; set => apellido_paterno = value; }
            public DateTime Fecha_nacimiento { get => fecha_nacimiento; set => fecha_nacimiento = value; }

            public Persona(int rut, int verificador, string nombre, string apellido, DateTime fecha)
            {
                this.rut = rut;
                this.verificador = verificador;
                this.nombre = nombre;
                this.apellido_paterno = apellido;
                this.fecha_nacimiento = fecha;
            }

            public Persona(string nombre, DateTime fecha)
            {
                this.nombre = nombre;
                this.fecha_nacimiento = fecha;
            }

            public Persona() { }

        }

        public class Empleado : Persona
        {
            private int id;
            private turno_t turno;
            private string contrasenia = string.Empty;
            private double sueldo_bruto;
            private double descuentos_previsionales;

            public int Id { get => id; set => id = value; }
            public turno_t Turno { get => turno; set => turno = value; }
            public string Contrasenia { get => contrasenia; set => contrasenia = value; }
            public double Sueldo_bruto { get => sueldo_bruto; set => sueldo_bruto = value; }
            public double Descuentos_previsionales { get => descuentos_previsionales; set => descuentos_previsionales = value; }

            public double CalcularSueldo()
            {
                return Sueldo_bruto - Descuentos_previsionales;
            }

            public Empleado(int rut, int verificador, string nombre, string apellido, DateTime fecha, int id, turno_t turno, string contrasenia, double sueldo, double descuento)
                : base(rut, verificador, nombre, apellido, fecha)
            {
                this.id = id;
                this.turno = turno;
                this.contrasenia = contrasenia;
                this.sueldo_bruto = sueldo;
                this.descuentos_previsionales = descuento;

            }

            public Empleado(int id, turno_t turno, string contrasenia, double sueldo, double descuento) : base()
            {
                this.id = id;
                this.turno = turno;
                this.contrasenia = contrasenia;
                this.sueldo_bruto = sueldo;
                this.descuentos_previsionales = descuento;
            }

            //Constructor para crear un registro con los datos: id, Nombre, Contraseña, fecha y turno
            public Empleado(string nombre, DateTime fecha, int id, turno_t turno, string contrasenia)
                : base(nombre, fecha)
            {
                {
                    this.id = id;
                    this.turno = turno;
                    this.contrasenia = contrasenia;
                }
            }

            public Empleado() : base() { }

            public void ProcesoVenta(int id, MetodoPago_t metodoPago, List<Producto> productos)
            {

            }
            public void FinalizarVenta(Cliente cliente)
            {
                //var venta = new Venta(int id);
                //var boleta = new Boleta(venta, cliente);
            }

        }

        public class Cliente : Persona
        {
            private int id;
            private tipoCliente_t tipo_cliente;
            private string contrasenia = string.Empty;

            public int Id { get => id; set => id = value; }
            public tipoCliente_t Tipo_Cliente { get => tipo_cliente; set => tipo_cliente = value; }
            public string Contrasenia { get => contrasenia; set => contrasenia = value; }

            public Cliente(int rut, int verificador, string nombre, string apellido, DateTime fecha, int id, tipoCliente_t tipo, string contrasenia)
                : base(rut, verificador, nombre, apellido, fecha)
            {
                this.id = id;
                this.tipo_cliente = tipo;
                this.contrasenia = contrasenia;
            }


            //Constructor de Registro para clientes, en el cual solo pide el nombre, fecha y contraseña
            public Cliente(string nombre, DateTime fecha, int id, string contrasenia)
                : base(nombre, fecha)
            {
                this.id = id;
                this.contrasenia = contrasenia;
            }

            public Cliente() : base() { }
        }
    }
}








