using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConCeldas
{
    public class Simulacion
    {
        private bool[,] mundo;
        private bool[,] siguienteGeneracion;
        private Task Proceso;

        public int Tamanio { get; private set; }
        public int Generacion { get; private set; }

        public Simulacion(int tamanio)
        {
            Tamanio = tamanio;
            mundo = new bool[tamanio, tamanio];
            siguienteGeneracion = new bool[tamanio, tamanio];
        }

        public Action<bool[,]> siguienteGeneracionFin;

        public bool this[int x, int y]
        {
            get { return this.mundo[x, y]; }
            set { this.mundo[x, y] = value; }
        }

        public bool AlternarCelda(int x, int y)
        {
            bool valorActual = mundo[x, y];
            return mundo[x, y] = !valorActual;
        }

        private static int VecinoVivo(bool[,] mundo, int tamanio, int x, int y, int movx, int movy)
        {
            int result = 0;

            int propMovX = x + movx;
            int propMovY = y + movy;
            bool fueralimite = propMovX < 0 || propMovX >= tamanio | propMovY < 0 || propMovY >= tamanio;
            if (!fueralimite)
            {
                result = mundo[x + movx, y + movy] ? 1 : 0;
            }
            return result;
        }

        private Task ProcesarGeneracion()
        {
            return Task.Factory.StartNew(() =>
            {
                Parallel.For(0, Tamanio, x =>
                {
                    Parallel.For(0, Tamanio, y =>
                    {
                        int numeroVecinos = VecinoVivo(mundo, Tamanio, x, y, -1, 0)
                            + VecinoVivo(mundo, Tamanio, x, y, -1, 1)
                            + VecinoVivo(mundo, Tamanio, x, y, 0, 1)
                            + VecinoVivo(mundo, Tamanio, x, y, 1, 1)
                            + VecinoVivo(mundo, Tamanio, x, y, 1, 0)
                            + VecinoVivo(mundo, Tamanio, x, y, 1, -1)
                            + VecinoVivo(mundo, Tamanio, x, y, 0, -1)
                            + VecinoVivo(mundo, Tamanio, x, y, -1, -1);

                        bool vivira = false;
                        bool estaVivo = mundo[x, y];

                        if (estaVivo && (numeroVecinos == 2 || numeroVecinos == 3))
                        {
                            vivira = true;
                        }
                        else if (!estaVivo && numeroVecinos == 3)
                        {
                            vivira = true;
                        }
                        siguienteGeneracion[x, y] = vivira;
                    });
                });
            });
        }


        public void Actualizar()
        {
            if (Proceso != null && Proceso.IsCompleted)
            {
                // cuando la generacion se completo
                var flip = siguienteGeneracion;
                siguienteGeneracion = mundo;
                mundo = flip;
                Generacion++;

                Proceso = ProcesarGeneracion();
                if (siguienteGeneracionFin != null)
                {
                    siguienteGeneracionFin(mundo);
                }                  
            }
        }

        public void IniciarGeneracion()
        {
            if (Proceso == null || (Proceso != null && Proceso.IsCompleted))
            {
                // Solo se inicia generacion si el proceso actual se completo o si no se ha inicializado
                Proceso = this.ProcesarGeneracion();
            }
        }
    }
}
