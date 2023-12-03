﻿using Entidades.DataBase;
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

        private void IniciarIngreso()
        {
            cancellation = new CancellationTokenSource();
            Task.Run(() =>
            {
                while (!cancellation.Token.IsCancellationRequested)
                {
                    NotificarNuevoIngreso(); // Metodo que instanciará e invocará un nuevo menu
                    EsperarProximoIngreso();
                    cantPedidosFinalizados += 1;
                    DataBaseManager.GuardarTicket(this.nombre, this.menu);
                }
            });
        }

        private void NotificarNuevoIngreso()
        {
            if (this.OnIngreso is not null)
            {
                menu = new T();
                menu.IniciarPreparacion();
                this.OnIngreso.Invoke(menu);
            }
        }
        private void EsperarProximoIngreso()
        {
            int tiempoEspera = 0;

            if (this.OnIngreso is not null)
            {
                while (!cancellation.Token.IsCancellationRequested && menu.Estado == false)
                {
                    Thread.Sleep(1000);
                    tiempoEspera++;
                }
            }
            this.demoraPreparacionTotal += tiempoEspera;
        }
    }
}
