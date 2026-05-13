using System.Text.RegularExpressions;

internal class Program
{
    static int ronda = 0;
    static int tirada, tiradaEnemigo;
    static Personaje pj = null;
    static Personaje enemigo = null;
    static Random Dados = new Random();

    static void Main(string[] args)
    {
        pj = CrearPersonaje();
        Menu();
    }

    private static Personaje CrearPersonaje()
    {
        while (true)
        {
            Console.WriteLine("\n--- Inserta nombre del personaje ---");
            string input = Console.ReadLine();

            if (Regex.IsMatch(input, @"^[a-zA-Z]+$"))
            {
                Console.WriteLine("\n--- ¡EMPIEZA EL JUEGO! ---");
                return new Personaje(input, 20, 1);
            }

            Console.WriteLine("El nombre solo puede contener letras.");
        }
    }

    private static int ComprobarOperacion(string input)
    {
        if (int.TryParse(input, out int resultado))
        {
            return resultado;
        }

        Console.WriteLine("No se ha reconocido la operación solicitada. Por favor, seleccione una válida.");
        return -1;
    }

    private static void Menu()
    {
        int operacion = -1;

        while (operacion != 0)
        {
            Console.WriteLine("\n--- SELECCIONA UNA OPERACIÓN ---");
            Console.WriteLine("1. Darse de piñas.");
            Console.WriteLine("2. Ver estadísticas.");
            Console.WriteLine("0. Salir del juego.");

            operacion = ComprobarOperacion(Console.ReadLine());

            switch (operacion)
            {
                case 1:
                    AvanzarRonda();
                    break;

                case 2:
                    Console.WriteLine("\n" + pj.VerStats());
                    Console.ReadLine();
                    break;

                case 0:
                    Environment.Exit(0);
                    break;
            }
        }
    }

    private static void AvanzarRonda()
    {
        // FIX 5: sustituida la recursión en el caso "chill" por un bucle do-while.
        // Antes, encadenar varios "chill" seguidos apilaba llamadas en el stack indefinidamente.
        int evento;
        do
        {
            Console.WriteLine("\n--- RONDA: " + ronda + " ---");
            ronda++;
            Console.ReadLine();

            evento = Dados.Next(0, 3);

            switch (evento)
            {
                case 0:
                    Console.WriteLine("¡Has sido atacado! Empieza la batalla... ¡Sobrevive!");
                    EmpezarPelea(false);
                    break;

                case 1:
                    Console.WriteLine("¡Has sido emboscado! Pierdes 1 punto de vida automáticamente... ¡Sobrevive!");
                    pj.Vida--;
                    EmpezarPelea(true);
                    break;

                case 2:
                    Console.WriteLine("Caminas con tranquilidad... no hay amenazas cerca. Descansas. +1 vida.");
                    pj.Vida++;
                    break;
            }
        } while (evento == 2); // si fue "chill" repetimos la ronda en lugar de apilar llamadas
    }

    private static void EmpezarPelea(bool emboscado)
    {
        enemigo = new Personaje("NPC", Dados.Next(2, 4), Dados.Next(1, 3));

        while (enemigo.Vida > 0)
        {
            Console.WriteLine("\n--- ESTADÍSTICAS DE TU OPONENTE ---");
            Console.WriteLine(enemigo.VerStats());

            // FIX 2: tiradas separadas para iniciativa en cada iteración del combate.
            // Antes, tirada y tiradaEnemigo se usaban tanto para la iniciativa como para
            // determinar críticos de ataque, fusionando dos fases que deben ser independientes.
            // Ahora: tirada/tiradaEnemigo = iniciativa. Dentro de cada turno se tira de nuevo para el ataque.
            tirada = TirarDado();
            tiradaEnemigo = TirarDado();

            // FIX 4: emboscado solo fuerza el primer turno, luego la iniciativa decide con normalidad.
            // Antes, emboscado era true durante toda la pelea, por lo que el jugador
            // siempre tenía el turno independientemente de las tiradas.
            bool tuTurno = emboscado || tirada >= tiradaEnemigo;
            emboscado = false; // se consume tras la primera iteración

            if (tuTurno)
            {
                TurnoTuyo();
            }
            else
            {
                TurnoOponente();
            }
        }

        Menu();
    }

    private static void TurnoTuyo()
    {
        Console.WriteLine("\n--- TU TURNO ---");
        Console.ReadLine();

        // FIX 2: tirada de ataque independiente de la de iniciativa.
        int tiradaAtaque = TirarDado();
        int tiradaDefensaEnemigo = TirarDado();

        Console.WriteLine($"Tiras: {tiradaAtaque} de ataque. El enemigo saca: {tiradaDefensaEnemigo} de defensa.");

        if (tiradaAtaque > tiradaDefensaEnemigo)
        {
            // FIX 2: el crítico de ataque se evalúa sobre la tirada de ataque, no sobre la de iniciativa.
            int danoReal = tiradaAtaque == 20 ? pj.Ataque + 1 : pj.Ataque;

            if (tiradaAtaque == 20)
                Console.WriteLine("¡CRÍTICO! (+1 de daño)");

            if (enemigo.Vida - danoReal <= 0)
            {
                Console.WriteLine("¡Has derrotado a tu enemigo!");
                enemigo.Vida = 0;
            }
            else
            {
                enemigo.Vida -= danoReal;
                Console.WriteLine($"¡Atravesaste la defensa enemiga! Daño infligido: {danoReal}. Vida del enemigo: {enemigo.Vida}");
            }
        }
        else if (tiradaDefensaEnemigo == 20)
        {
            // FIX 1 y 3: el contraataque enemigo ya no usa variables de campo ni recursión con tiradas viejas.
            // Se llama directamente a TurnoOponente() con nuevas tiradas, que se generan dentro del propio método.
            Console.WriteLine("¡DEFENSA CRÍTICA del enemigo! Te contraataca.");
            Console.ReadLine();
            TurnoOponente();
        }
        else
        {
            Console.WriteLine("El enemigo resistió tu acometida.");
        }

        Console.ReadLine();
    }

    private static void TurnoOponente()
    {
        Console.WriteLine("\n--- TURNO DEL OPONENTE ---");
        Console.ReadLine();

        // FIX 2: tirada de ataque independiente para el turno del oponente.
        int tiradaAtaqueEnemigo = TirarDado();
        int tiradaDefensaTuya = TirarDado();

        Console.WriteLine($"El enemigo tira: {tiradaAtaqueEnemigo} de ataque. Tu defensa: {tiradaDefensaTuya}.");

        if (tiradaAtaqueEnemigo > tiradaDefensaTuya)
        {
            // FIX 2: el crítico enemigo se evalúa sobre su tirada de ataque, no sobre la de iniciativa.
            int danoRecibido = tiradaAtaqueEnemigo == 20 ? enemigo.Ataque + 1 : enemigo.Ataque;

            if (tiradaAtaqueEnemigo == 20)
                Console.WriteLine("¡CRÍTICO ENEMIGO! (+1 de daño)");

            if (pj.Vida - danoRecibido <= 0)
            {
                Console.WriteLine("¡Has sido derrotado! ¡Fin de la partida!");
                Environment.Exit(0);
            }
            else
            {
                pj.Vida -= danoRecibido;
                Console.WriteLine($"¡El enemigo atravesó tus defensas! Daño recibido: {danoRecibido}. Tu vida: {pj.Vida}");
            }
        }
        else if (tiradaDefensaTuya == 20)
        {
            // FIX 1 y 3: el contraataque tuyo ya no usa variables de campo ni recursión con tiradas viejas.
            // Se llama directamente a TurnoTuyo() con nuevas tiradas generadas dentro del método.
            Console.WriteLine("¡DEFENSA CRÍTICA! Contraatacas al enemigo.");
            Console.ReadLine();
            TurnoTuyo();
        }
        else
        {
            Console.WriteLine("Tu defensa fue más fuerte que el ataque enemigo.");
        }

        Console.ReadLine();
    }

    private static int TirarDado()
    {
        return Dados.Next(1, 21);
    }
}