using Farmacia.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using Farmacia.Almacen.Productos;
using Newtonsoft.Json;
using Farmacia.Usuarios;
using System.IO;
using System.Collections.ObjectModel;

/*
 

Diferencias con el UML:

 
 *Inventario:
 - Agregue un Diccionario que sirve para calcular el stock de los productos, funciona con un par clave-valor, la clave corresponde al ID del producto
   y el valor corresponde al stock total de cada ID.

 - Cambie los parametros de entrada de la interfaz Iinventario.

 - Agregue algunas funciones para el control del archivo JSON(CargarDatos() y GuardarenJson()) como tambien para el diccionario. 


 *Producto:
 - Agregue el atributo "peso" ya que es necesario para diferenciar algunos productos.


 *Medicamento:
  - Agregue el atributo Receta, el cual indica si el medicamento requiere de receta.
    SI Receta es true, se crea una clase Receta para el medicamento y tambien es necesaria la funcion AgregarReceta.

    Esto cambia un poco el UML ya que la composicion iría desde Receta a Medicamento.
 
 
 *Receta:
  - Solo puse los atributos "ID", "Nombre medico" y "Fecha".
  
 */

namespace Farmacia
{
    namespace Almacen
    {

        public interface IInventario
        {
            public void AgregarProducto(Productos.Producto producto);
            public void EliminarProducto(Productos.Producto producto);
            public void ActualizarStock(Productos.Producto producto, int cantidad);
            public void ConsultarStock(Productos.Producto producto);
        }

        public class Inventario : IInventario
        {

            public void AgregarProducto(Productos.Producto producto) { }           

            private List<Producto> productos_ = new List<Producto>();
            private Dictionary<int, int> stock;
            private List<Medicamento> medicamentos = new List<Medicamento>();
            private List<Dermocosmetico> dermocosmeticos = new List<Dermocosmetico>();

            public List<Productos.Producto> Productos_ { get => this.productos_; set => productos_ = value; }
            public Dictionary<int, int> Stock { get => this.stock; set => this.stock = value; }
            public List<Medicamento> Medicamentos { get => this.medicamentos; set => this.medicamentos = value; }   
            public List<Dermocosmetico> Dermocosmeticos { get => this.dermocosmeticos; set => this.dermocosmeticos = value; }

            public Inventario()
            {
                //Mediante esta funcion se llena la lista de productos con los datos del archivo JSON
           
                this.medicamentos = CargarDatos_Medicamentos();
                this.dermocosmeticos = CargarDatos_Dermocosmeticos();
                this.productos_.AddRange(medicamentos);
                this.productos_.AddRange(dermocosmeticos);

                this.stock = new Dictionary<int, int>();
                InicializarStock();

            }

            #region Medicamentos
            public void AgregarProducto(Medicamento medicamento)
            {
                medicamentos.Add(medicamento);   //Se agrega a la lista del inventario
                productos_.Add(medicamento);     //Necesario ya que el stock trabaja en esta lista

                if (stock.ContainsKey(medicamento.Id)) //Si el id se encuentra dentro del diccionario, el stock de la id aumenta en 1.
                {
                    stock[medicamento.Id] += 1;
                }
                else
                {
                    stock[medicamento.Id] = 1;
                }

                ActualizarDatos(medicamentos);
            }

            //Funcion que elimina el producto ingresado, cumple la funcion de detectar que clase hija es y opera de acuerdo a ella.
            public void EliminarProducto(Producto producto)
            {
                if(producto is Medicamento medicamento)
                {
                    medicamentos.Remove(medicamento);
                    Productos_.Remove(medicamento);
                    stock[medicamento.Id] -= 1;

                    if (stock[medicamento.Id] <= 0)
                    {
                        stock.Remove(medicamento.Id);
                    }
                    ActualizarDatos(medicamentos);

                }else if (producto is Dermocosmetico dermocosmetico)
                {
                    dermocosmeticos.Remove(dermocosmetico);
                    Productos_.Remove(dermocosmetico);
                    stock[dermocosmetico.Id] -= 1;

                    if (stock[dermocosmetico.Id] <= 0)
                    {
                        stock.Remove(dermocosmetico.Id);
                    }
                    ActualizarDatos(dermocosmeticos);
                }
            }

            public void EliminarProducto(int id)
            {
                var medicamento = medicamentos.FirstOrDefault(m => m.Id == id);
                if (medicamento != null)
                {
                    medicamentos.Remove(medicamento);
                    productos_.Remove(medicamento);
                    stock[medicamento.Id] -= 1;

                    if (stock[medicamento.Id] <= 0)
                    {
                        stock.Remove(medicamento.Id);
                    }
                    ActualizarDatos(medicamentos);
                }

                var dermocosmetico = dermocosmeticos.FirstOrDefault(d => d.Id == id);               
                if (dermocosmetico != null)
                {
                    dermocosmeticos.Remove(dermocosmetico);
                    productos_.Remove(dermocosmetico);
                    stock[dermocosmetico.Id] -= 1;

                    if (stock[dermocosmetico.Id] <= 0)
                    {
                        stock.Remove(dermocosmetico.Id);
                    }
                    ActualizarDatos(dermocosmeticos);
                }
            }

            #endregion



            #region Dermocosmeticos

            public void AgregarProducto(Dermocosmetico dermocosmetico)
            {
                dermocosmeticos.Add(dermocosmetico);   //Se agrega a la lista del inventario
                productos_.Add(dermocosmetico);     //Necesario ya que el stock trabaja en esta lista

                if (stock.ContainsKey(dermocosmetico.Id)) //Si el id se encuentra dentro del diccionario, el stock de la id aumenta en 1.
                {
                    stock[dermocosmetico.Id] += 1;
                }
                else
                {
                    stock[dermocosmetico.Id] = 1;
                }

                ActualizarDatos(Dermocosmeticos);
            }

            #endregion



            #region PROCESO PARA STOCK

            //Actualiza el stock recibiendo un producto e ingresando la cantidad 
            public void ActualizarStock(Productos.Producto producto, int cantidad)
            {
                if (stock.ContainsKey(producto.Id))  //Si el id se encuentra en el diccionario devuelve la cantidad de stock de dicho id
                {
                    stock[producto.Id] += cantidad;
                }
                else
                {
                    stock[producto.Id] = cantidad;
                }

                if (producto is Medicamento medicamento)
                {
                    for (int i = 0; i < cantidad; i++)
                    {
                        medicamentos.Add(medicamento);
                        ActualizarDatos(medicamentos);
                    }
                }
                else if (producto is Dermocosmetico dermocosmetico)
                {
                    for (int i = 0; i < cantidad; i++)
                    {
                        dermocosmeticos.Add(dermocosmetico);
                        ActualizarDatos(medicamentos);
                    }
                }

                for (int i = 0; i < cantidad; i++)
                {
                    productos_.Add(producto);
                }

            }



            //Consultar el Stock en base al producto
            public void ConsultarStock(Productos.Producto producto)
            {
                if (stock.ContainsKey(producto.Id))
                {
                    Console.WriteLine($"Stock del producto {producto.Nombre} (ID {producto.Id}): {stock[producto.Id]}");
                }
                else
                {
                    Console.WriteLine("El producto no se encuentra en el inventario.");
                }
            }


            //Consultar Stock mediante la ID
            public void ConsultarStock(int id)
            {
                if (stock.ContainsKey(id))
                {
                    Console.WriteLine($"Stock del producto (ID {id}): {stock[id]}");
                }
                else
                {
                    Console.WriteLine("El producto no se encuentra en el inventario.");
                }
            }


            //Función que inicia el stock, lo inicia vacío y luego recorre la lista del Inventario.
            public void InicializarStock()
            {
                // Iniciar en 0 el stock
                stock.Clear();

                // Recorrer todos los productos y contar las cantidades por ID
                foreach (var producto in productos_)
                {
                    if (stock.ContainsKey(producto.Id))
                    {
                        stock[producto.Id] += 1;
                    }
                    else
                    {
                        stock[producto.Id] = 1;
                    }
                }
            }

            #endregion



            #region proceso de Json

            //PROCESO JSON MEDICAMENTOS
            public List<Medicamento> CargarDatos_Medicamentos()
            {
                string datos = string.Empty, rutajson = "Medicamentos.json";

                using (var lector = new StreamReader(rutajson))
                {
                    datos = lector.ReadToEnd();
                }

                var medicamentosaux = JsonConvert.DeserializeObject<List<Medicamento>>(datos);

                return medicamentosaux ?? new List<Medicamento>();
            }

            public void ActualizarDatos(List<Medicamento> list_medicamentos)
            {
                string medicamentosaux = JsonConvert.SerializeObject(list_medicamentos.ToArray(), Formatting.Indented);
                File.WriteAllText("Medicamentos.json", medicamentosaux);
            }





            //Configuracion de JSON DERMOCOSMETICOS
            public List<Dermocosmetico> CargarDatos_Dermocosmeticos()
            {
                string datos = string.Empty, rutajson = "Dermocosmeticos.json";

                using (var lector = new StreamReader(rutajson))
                {
                    datos = lector.ReadToEnd();
                }

                var dermo = JsonConvert.DeserializeObject<List<Dermocosmetico>>(datos);

                return dermo ?? new List<Dermocosmetico>();
            }

            public void ActualizarDatos(List<Dermocosmetico> list_dermo)
            {
                string dermo = JsonConvert.SerializeObject(list_dermo.ToArray(), Formatting.Indented);
                File.WriteAllText("Dermocosmeticos.json", dermo);
            }
            #endregion


        }


        //Creacion de la clase Receta para Medicamento.
        public class Receta
        {
            private int id_receta;
            private string n_medico = string.Empty;
            private DateTime fecha;

            public int Id_receta { get => id_receta; set => id_receta = value; }
            public string N_medico { get => n_medico; set => n_medico = value; }
            public DateTime Fecha { get => fecha; set => fecha = value; }

            public Receta() { }

            public Receta(int id_receta, string n_medico, DateTime fecha)
            {
                this.id_receta = id_receta;
                this.n_medico = n_medico;
                this.fecha = fecha;
            }
        }



        //Creacion del espacio de nombre Productos, en donde se definen Medicamentos y Dermocosmeticos.
        namespace Productos
        {
            public class Producto
            {
                private int id;
                private string nombre = string.Empty;
                private double precio;
                private double peso;
                private string imagen;

                public int Id { get => id; set => id = value; }
                public string Nombre { get => nombre; set => nombre = value; }
                public double Precio { get => precio; set => precio = value; }
                public double Peso { get => peso; set => peso = value; }
                public string Imagen { get ; set ; }


                //Productos sin fechas
                public Producto(int id, string nombre, double precio, double peso)
                {
                    this.id = id;
                    this.nombre = nombre;
                    this.precio = precio;
                    this.peso = peso;
                    this.imagen = imagen;
                }
                public Producto() { }

                public override string ToString()
                {
                    return $"ID: <{Id}>  -  Nombre: <{Nombre}>  -  Precio: <{Precio}>";
                }

            }

            public class Medicamento : Producto
            {
                private string laboratorio = string.Empty;
                private string tipo = string.Empty;
                private string bioequivalente;             
                private bool req_receta;
                private Receta receta;

                public string Laboratorio { get => laboratorio; set => laboratorio = value; }
                public string Tipo { get => tipo; set => tipo = value; }
                public string Bioequivalente { get => bioequivalente; set => bioequivalente = value; }
                public bool Req_Receta { get => req_receta; set => req_receta = value; }

                public Medicamento(int id, string nombre, double precio, double peso, string laboratorio, string tipo, string bioequivalente, bool req_receta)
                    : base(id, nombre, precio, peso)
                {
                    this.laboratorio = laboratorio;
                    this.tipo = tipo;
                    this.bioequivalente = bioequivalente;
                    this.req_receta = req_receta;


                    //Con esto definimos una Composicion. Si el medicamento requiere una receta, se inicializa la clase definida anteriormente
                    if (req_receta)
                    {
                        this.receta = new Receta();

                    }
                    else
                    {
                        this.req_receta = false;
                    }
                }

                public Medicamento() : base() { }

                public Medicamento(int id, string nombre, double precio, double peso, string laboratorio, string bioequivalente, bool req_receta)
                    : base(id, nombre, precio, peso)
                {
                    this.laboratorio = laboratorio;
                    this.bioequivalente = bioequivalente;
                    this.req_receta = req_receta;              

                    //Con esto definimos una Composicion. Si el medicamento requiere una receta, se inicializa la clase definida anteriormente
                    if (req_receta)
                    {
                        this.receta = new Receta();

                    }
                    else
                    {
                        this.req_receta = false;
                    }
                }

                // Se define la funcion para agregar una receta a algun medicamento que la requiera. 
                public void AgregarReceta(Receta rece, int id, string medico, DateTime fecha)
                {
                    rece.Id_receta = id;
                    rece.N_medico = medico;
                    rece.Fecha = fecha;                
                }
            }


            public class Dermocosmetico : Producto
            {
                private piel_t tipo_piel;
                private string tipo = string.Empty;

                public piel_t Tipo_piel { get => tipo_piel; set => tipo_piel = value; }
                public string Tipo { get => tipo; set => tipo = value; }

                public Dermocosmetico(int id, string nombre, double precio, double peso, piel_t tipo_piel, string tipo)
                    : base(id, nombre, precio, peso)
                {
                    this.tipo_piel = tipo_piel;
                    this.tipo = tipo;
                }

                public Dermocosmetico(int id, string nombre, double precio, double peso, piel_t tipo)
                    : base(id, nombre, precio, peso)
                {
                    this.tipo_piel = tipo;
                }

                public Dermocosmetico() : base() { }
            }

        }

    }
}