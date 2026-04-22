using SERVICIOS;
using System;

namespace GeneradorHash
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Ingrese la contraseña para hashear:");
            string password = Console.ReadLine();

            string salt;
            string hash = SeguridadService.GenerarHashConSalt(password, out salt);

            Console.WriteLine("\n--- Resultado ---");
            Console.WriteLine($"Contraseña: {password}");
            Console.WriteLine($"Hash: {hash}");
            Console.WriteLine($"Salt: {salt}");
            Console.WriteLine("\nPresione una tecla para salir...");
            Console.ReadKey();
        }
    }
}

