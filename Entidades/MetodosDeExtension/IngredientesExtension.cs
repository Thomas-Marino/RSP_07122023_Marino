using Entidades.Enumerados;


namespace Entidades.MetodosDeExtension
{
    public static class IngredientesExtension
    {
        /// <summary>
        /// Método de extensión de una lista de EIngredietne encargado de calcular el costo de la comida 
        /// partiendo de un costo base y dependiendo de cuantos ingredientes tenga agregados.
        /// </summary>
        /// <param name="ingredientes">Lista de ingredientes que seran agregados a la comida.</param>
        /// <param name="costoInicial">Costo base de la comida.</param>
        /// <returns>
        /// Precio incrementado dependiendo de cuantos ingredientes fueron agregados.
        /// </returns>
        public static double CalcularCostoIngredientes(this List<EIngrediente> ingredientes, int costoInicial)
        {
            double precioFinal = 0.0;

            foreach (var ingrediente in ingredientes)
            {
                int precioIngrediente = (int)ingrediente;
                int incremento = (costoInicial * precioIngrediente)/ 100;
                precioFinal += incremento;
            }

            return precioFinal + costoInicial;
        }
        /// <summary>
        /// Método de extensión para objetos Random encargado de tomar aleatoriamente entre
        /// 1 y la cantidad máxima de ingredientes de un enumerado de ingredientes.
        /// </summary>
        /// <param name="rand">Objeto random.</param>
        /// <returns>
        /// Una lista compuesta por ingredientes de un enumerado tomados aleatoriamente.
        /// </returns>
        public static List<EIngrediente> IngredientesAleatorios(this Random rand)
        {
            List<EIngrediente> ingredientes = new List<EIngrediente>()
            {
                EIngrediente.ADHERESO,
                EIngrediente.QUESO,
                EIngrediente.JAMON,
                EIngrediente.HUEVO,
                EIngrediente.PANCETA
            };
            int numeroAleatorio = rand.Next(1, (ingredientes.Count + 1));
            return ingredientes.Take(numeroAleatorio).ToList();
        }

    }
}
