using Entidades.Enumerados;


namespace Entidades.MetodosDeExtension
{
    public static class IngredientesExtension
    {

        public static double CalcularCostoIngredientes(this List<EIngrediente> ingredientes, int costoInicial)
        {
            double precioFinal = 0.0;

            foreach (var ingrediente in ingredientes)
            {
                int precioIngrediente = (int)ingrediente;
                int incremento = costoInicial * precioIngrediente / 100;
                precioFinal += incremento;
            }

            return precioFinal;
        }
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
