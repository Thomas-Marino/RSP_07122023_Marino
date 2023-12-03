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

        public bool Estado { get; }
        public string Imagen { get; }
        public string Ticket => $"{this}\nTotal a pagar:{this.costo}";

        static Hamburguesa() => Hamburguesa.costoBase = 1500;
        public Hamburguesa() : this(false) { }
        public Hamburguesa(bool esDoble)
        {
            this.esDoble = esDoble;
            this.random = new Random();
        }

        private void AgregarIngredientes()
        {
            ingredientes = new List<EIngrediente>(random.IngredientesAleatorios());
        }

        public void FinalizarPreparacion(string cocinero)
        {
            this.costo = this.ingredientes.CalcularCostoIngredientes(costoBase);
            this.estado = false;
        }

        public void IniciarPreparacion()
        {
            if (!this.estado)
            {
                int hamburguesaAleatoria = random.Next(1, 9);
                string tipoHamburguesa = $"Hamburguesa_{hamburguesaAleatoria}";
                this.imagen = DataBaseManager.GetImagenComida(tipoHamburguesa);
                this.estado = true;
                AgregarIngredientes();
            }
        }

        private string MostrarDatos()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine($"Hamburguesa {(this.esDoble ? "Doble" : "Simple")}");
            stringBuilder.AppendLine("Ingredientes: ");
            this.ingredientes.ForEach(i => stringBuilder.AppendLine(i.ToString()));
            return stringBuilder.ToString();
        }

        public override string ToString() => this.MostrarDatos();
    }
}