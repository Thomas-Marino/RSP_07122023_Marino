using Entidades.DataBase;
using Entidades.Exceptions;
using Entidades.Files;
using Entidades.Interfaces;


namespace Entidades.Modelos
{
    public delegate void DelegadoDemoraAtencion(double demora);
    public delegate void DelegadoNuevoIngreso(IComestible menu);

    public class Cocinero<T> where T : IComestible, new()
    {
        private CancellationTokenSource cancellation;

        private int cantPedidosFinalizados;
        private double demoraPreparacionTotal;

        private T menu;
        private string nombre;
        private Task tarea;

        public event DelegadoDemoraAtencion OnDemora;
        public event DelegadoNuevoIngreso OnIngreso;

        public Cocinero(string nombre)
        {
            this.nombre = nombre;
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
                    this.IniciarIngreso();
                }
                else
                {
                    this.cancellation.Cancel();
                }
            }
        }

        //no hacer nada
        public double TiempoMedioDePreparacion { get => this.cantPedidosFinalizados == 0 ? 0 : this.demoraPreparacionTotal / this.cantPedidosFinalizados; }
        public string Nombre { get => nombre; }
        public int CantPedidosFinalizados { get => cantPedidosFinalizados; }

        #region "Métodos"
        /// <summary>
        /// Ejecutará en un hilo secundario la accion de: NotificarNuevoIngreso, EsperarProximoIngreso, incrementará
        /// la cantidad de pedidos finalizados en 1 luego de esperar al proximo ingreso y guardará el ticket en la BD.
        /// </summary>
        private void IniciarIngreso()
        {
            this.tarea = Task.Run(() =>
            {
                while (!this.cancellation.IsCancellationRequested)
                {
                    this.NotificarNuevoIngreso(); // Metodo que instanciará e invocará un nuevo menu
                    this.EsperarProximoIngreso();
                    this.cantPedidosFinalizados += 1;
                    DataBaseManager.GuardarTicket(this.nombre, this.menu);
                }
            }, this.cancellation.Token);
        }

        /// <summary>
        /// Método encargado de verificar si el evento OnIngreso posee suscriptores.
        /// En caso exitoso instanciará un nuevo menú, iniciará la preparación del menú y notificará el menú.
        /// </summary>
        private void NotificarNuevoIngreso()
        {
            if (this.OnIngreso is not null)
            {
                menu = new T();
                menu.IniciarPreparacion();
                this.OnIngreso.Invoke(menu);
            }
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
                while (this.menu.Estado == false && !this.cancellation.IsCancellationRequested)
                {
                    Thread.Sleep(1000);
                    tiempoEspera++;
                    this.OnDemora.Invoke(tiempoEspera);
                }
            }

            this.demoraPreparacionTotal += tiempoEspera;
        }
        #endregion
    }
}
