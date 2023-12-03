namespace Entidades.Interfaces
{
    public interface IComestible
    {
        public bool Estado { get; }
        public string Imagen { get; }
        public string Ticket { get; }

        public void FinalizarPreparacion(string cocinero);
        public void IniciarPreparacion();
    }
}
