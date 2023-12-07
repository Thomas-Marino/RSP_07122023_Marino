using Entidades.DataBase;
using Entidades.Exceptions;
using Entidades.Files;
using Entidades.Interfaces;


namespace Entidades.Modelos
{
    public delegate void DelegadoDemoraAtencion(double demora);
    public delegate void DelegadoPedidoEnCurso(IComestible menu);

    public class Cocinero<T> where T : IComestible, new()
    {
        private CancellationTokenSource cancellation;

        private int cantPedidosFinalizados;
        private double demoraPreparacionTotal;

        private Mozo<T> mozo;
        private string nombre;
        private T pedidoEnPreparacion;
        private Queue<T> pedidos;
        private Task tarea;

        public event DelegadoDemoraAtencion OnDemora;
        public event DelegadoPedidoEnCurso OnPedido;

        public Cocinero(string nombre)
        {
            this.mozo = new Mozo<T>();
            this.nombre = nombre;
            this.pedidos = new Queue<T>();
            this.mozo.OnPedido += this.TomarNuevoPedido;
        }

        //No hacer nada
        public bool HabilitarCocina
        {
            get
            {
                return this.tarea is not null && (this.tarea.Status == TaskStatus.Running ||
                    this.tarea.Status == TaskStatus.WaitingToRun ||
                    this.tarea.Status == TaskStatus.WaitingForActivation);
            }
            set
            {
                if (value && !this.HabilitarCocina)
                {
                    this.cancellation = new CancellationTokenSource();
                    this.mozo.EmpezarATrabajar = true;
                    this.EmpezarACocinar();
                }
                else
                {
                    this.cancellation.Cancel();
                    this.mozo.EmpezarATrabajar = false;
                }
            }
        }

        //no hacer nada
        public double TiempoMedioDePreparacion { get => this.cantPedidosFinalizados == 0 ? 0 : this.demoraPreparacionTotal / this.cantPedidosFinalizados; }
        public string Nombre { get => nombre; }
        public int CantPedidosFinalizados { get => cantPedidosFinalizados; }

        public Queue<T> Pedidos
        {
            get => pedidos;
        }

        #region "Métodos"

        private void EmpezarACocinar()
        {
            this.tarea = Task.Run(() =>
            {
                while (!this.cancellation.IsCancellationRequested)
                {
                    if (this.pedidos.Count > 0)
                    {
                        this.pedidoEnPreparacion = this.pedidos.Dequeue();
                        this.OnPedido.Invoke(pedidoEnPreparacion);
                        EsperarProximoIngreso();
                        this.cantPedidosFinalizados += 1;
                        try
                        {
                            DataBaseManager.GuardarTicket(this.nombre, this.pedidoEnPreparacion);
                        }
                        catch (DataBaseManagerException DBexception)
                        {
                            throw new DataBaseManagerException(DBexception.Message);
                        }
                    }
                }
            }, this.cancellation.Token);

        }

        /// <summary>
        /// Método encargado de verificar si el evento OnDemora posee suscriptores.
        /// En caso exitoso se encargará de realizar el incremento del tiempo de espera en 1 por cada segundo
        /// que pase mientras que el hilo secundario no requiera cancelación y el estado del pedido
        /// sea false.
        /// Finalmente establecerá cual fue el tiempo de demora total en la preparación del pedido.
        /// </summary>
        private void EsperarProximoIngreso()
        { 
            int tiempoEspera = 0;

            if (this.OnDemora is not null)
            {
                while (this.mozo.EmpezarATrabajar == false && !this.cancellation.IsCancellationRequested)
                {
                    Thread.Sleep(1000);
                    tiempoEspera++;
                    this.OnDemora.Invoke(tiempoEspera);
                }
            }

            this.demoraPreparacionTotal += tiempoEspera;
        }

        private void TomarNuevoPedido(T menu)
        {
            if (this.OnPedido is not null)
            {
                this.pedidos.Enqueue(menu);
            }
        }
        #endregion
    }
}
