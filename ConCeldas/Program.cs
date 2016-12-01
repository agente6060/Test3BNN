using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConCeldas
{
    class Program
    {
        static void Main(string[] args)
        {
            int tamanoMundo = 50;

            Simulacion sim = new Simulacion(tamanoMundo);

            Random rnd = new Random();

            // intercambiaremos estados ya que inicializan en false(muerto)
            int intercambios = rnd.Next(1, tamanoMundo);

            for (int i = 0; i < intercambios; i++)
            {
                sim.AlternarCelda(rnd.Next(1, tamanoMundo), rnd.Next(1, tamanoMundo));
            }
            sim.IniciarGeneracion();
            Thread.Sleep(100);
            ImprimirResultados(sim);

            Console.ReadKey();
        }

        private static void ImprimirResultados(Simulacion sim)
        {
            var line = new String('-', sim.Tamanio);
            Console.WriteLine(line);

            for (int y = 0; y < sim.Tamanio; y++)
            {
                for (int x = 0; x < sim.Tamanio; x++)
                {
                    Console.Write(sim[x, y] ? "1" : "0");
                }
                Console.WriteLine();
            }
        }
    }
}
