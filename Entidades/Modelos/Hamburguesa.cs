using Entidades.Enumerados;
using Entidades.Exceptions;
using Entidades.Files;
using Entidades.Interfaces;
using Entidades.MetodosDeExtension;
using System.Text;
using Entidades.DataBase;

namespace Entidades.Modelos
{
    public class Hamburguesa : IComestible
    {
        private double costo;
        private static int costoBase;
        private bool esDoble;
        private bool estado;
        private string imagen;
        List<EIngrediente> ingredientes;
        Random random;

        public bool Estado { get => this.estado; }
        public string Imagen { get { return this.imagen; } }
        public string Ticket => $"{this}\nTotal a pagar:{this.costo}";

        static Hamburguesa() => Hamburguesa.costoBase = 1500;
        public Hamburguesa() : this(false) { }
        public Hamburguesa(bool esDoble)
        {
            this.esDoble = esDoble;
            this.random = new Random();
        }

        #region "Métodos"
        /// <summary>
        /// Método encargado de agregar los ingredientes a la hamburguesa de manera aleatoria.
        /// </summary>
        private void AgregarIngredientes()
        {
            ingredientes = new List<EIngrediente>(random.IngredientesAleatorios());
        }

        /// <summary>
        /// Método que siempre y cuando el estado de la hamburguesa sea false, se encargará de: 
        /// generar un número aleatorio entre el 1 y 9 y asignar una imagen a la hamburguesa que 
        /// será displayeada en el formulario dependiendo del número aleatorio y luego llamara 
        /// al método AgregarIngredientes.
        /// </summary>
        public void IniciarPreparacion()
        {
            if (!this.estado)
            {
                int hamburguesaAleatoria = random.Next(1, 9);
                string tipoHamburguesa = $"Hamburguesa_{hamburguesaAleatoria}";
                this.imagen = DataBaseManager.GetImagenComida(tipoHamburguesa);
                AgregarIngredientes();
            }
        }

        /// <summary>
        /// Método encargado de asignarle el costo a la hamburguesa en relación al costo base
        /// y a los ingredientes de la misma. Luego cambiará el estado de la hamburguesa.
        /// </summary>
        /// <param name="cocinero">Cocinero encargado de preparar la hamburguesa.</param>
        public void FinalizarPreparacion(string cocinero)
        {
            this.costo = this.ingredientes.CalcularCostoIngredientes(costoBase);
            this.estado = true;
        }

        /// <summary>
        /// Método encargado de mostrar los datos de la hamburguesa.
        /// </summary>
        /// <returns>
        /// Datos formateados de la hamburguesa.
        /// </returns>
        private string MostrarDatos()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine($"Hamburguesa {(this.esDoble ? "Doble" : "Simple")}");
            stringBuilder.AppendLine("Ingredientes: ");
            this.ingredientes.ForEach(i => stringBuilder.AppendLine(i.ToString()));
            return stringBuilder.ToString();
        }

        public override string ToString() => this.MostrarDatos();

        #endregion
    }
}