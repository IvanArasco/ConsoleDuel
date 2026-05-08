using System.Text.RegularExpressions;

// x No le damos importancia a la defensa para reducir el daño de ataque
// x No funciona crítico del personaje SIN matar.
// x Cuanta vida te queda después de recibir daño no sale.
internal class Program
{
    static int ronda = 0;
    static int tirada, tiradaEnemigo;
    static bool contraataque = false;
    static bool contraataqueEnemigo = false;
    static Personaje pj = null;
    static Personaje enemigo = null;
    static Random Dados = new Random();
    static void Main(string[] args)
    {
        pj = CrearPersonaje();

        Menu();
    }
    public static Personaje CrearPersonaje()
    {
        string input;
        while (pj == null)
        {
            Console.WriteLine("\n--- Inserta nombre del personaje ---");
            input = Console.ReadLine();

            if (Regex.IsMatch(input, @"^[a-zA-Z]+$"))
            {
                Console.WriteLine("\n--- ¡EMPIEZA EL JUEGO!: --- ");
                return new Personaje(input, 20, 1, 1);
            }
        }
        return null;
    }
    public static int ComprobarOperacion(string input)
    {
        if (int.TryParse(input, out int resultado))
        {
            return resultado;
        }
        else
        {
            Console.WriteLine("No se ha reconocido la operación solicitada. Por favor, seleccione una válida.");
            return -1;
        }
    }
    public static void Menu()
    {
        int operacion = -1;

        while (operacion != 0)
        {
            Console.WriteLine("--- SELECCIONA UNA OPERACIÓN ---\n ");
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
                    System.Environment.Exit(0);
                    break;
            }
        }
    }
    public static void AvanzarRonda()
    {

        Console.WriteLine("\n--- RONDA: --- " + ronda);
        ronda++;
        Console.ReadLine(); // Darle al enter para iniciar la ronda.

        switch (Dados.Next(0, 3))
        {
            case 0: // te atacan
                Console.WriteLine("¡Has sido atacado! Empieza la batalla... ¡Sobrevive!");
                EmpezarPelea(false);
                break;
            case 1: // emboscada
                Console.WriteLine("¡Has sido emboscado! Pierdes 1p. de vida automáticamente... y empieza la batalla, ¡sobrevive!");
                pj.Vida--;
                EmpezarPelea(true);
                break;

            case 2: // chill
                Console.WriteLine("Caminas con tranquilidad... no parece haber amenazas cerca. Aprovechas para descansar. +1 vida.");
                pj.Vida++;
                AvanzarRonda();
                break;
        }
    }
    public static void EmpezarPelea(bool emboscado)
    {
        enemigo = new Personaje("NPC", Dados.Next(2, 4), Dados.Next(1, 3), Dados.Next(1, 3));

        while (enemigo.Vida > 0)
        {
            Console.WriteLine("\n--- ESTADÍSTICAS DE TU OPONENTE ---");
            Console.WriteLine(enemigo.VerStats());

            tirada = pj.TirarDado();
            tiradaEnemigo = enemigo.TirarDado();

            bool tuTurno = emboscado || tirada > tiradaEnemigo;
            // si te emboscan te toca a ti si o si. Y solo en caso de que tu dado sea mayor al del enemigo será tu turno (iniciativa)

            if (tuTurno && !contraataqueEnemigo) // que sea tu turno (de normal te podría tocar pero te podrían contraatacar)
            {
                TurnoTuyo(false);
            } else
            {
                TurnoOponente(false);
            }
        }

        Menu(); // regresamos al menu (solo va a suceder cuando el enemigo muera, bien tú)
    }
    public static void TurnoTuyo(bool contraataque)
    {
        Console.WriteLine("\n--- TU TURNO ---");
        Console.ReadLine();

        if (tirada > tiradaEnemigo)
        {
            if (tirada == 20) // crítico atk
            {
                if (enemigo.Vida - (pj.Ataque + 1) <= 0) // crítico y lo matas
                {
                    Console.WriteLine($"Has sacado un CRÍTICO (+1 a tu ataque).");
                    Console.WriteLine("¡Has derrotado a tu enemigo!");
                    enemigo.Vida = 0;
                    Console.ReadLine();
                }
                else // crítico pero no lo matas
                {
                    Console.WriteLine($"Has sacado un CRÍTICO (+1 a tu ataque).");
                    enemigo.Vida -= pj.Ataque + 1;
                    Console.ReadLine();
                }
            }
            else if (enemigo.Vida - pj.Ataque <= 0) // ataque normal pero lo matas
            {
                Console.WriteLine($"Has sacado un {tirada} de ataque frente a una defensa de: {tiradaEnemigo} del enemigo.");
                Console.WriteLine("¡Has derrotado a tu enemigo!");
                enemigo.Vida = 0;
                Console.ReadLine();
            }

            else // ataque normal pero no lo matas
            {
                Console.WriteLine($"Has sacado un {tirada} de ataque frente a una defensa de: {tiradaEnemigo} del enemigo.");
                Console.WriteLine("¡Atravesaste la defensa de tu enemigo! Le has infligido: " + pj.Ataque + " puntos de daño.");
                enemigo.Vida -= pj.Ataque;
                Console.ReadLine();
            }
        }

        else if (tiradaEnemigo == 20) // defensa crítica del enemigo
        {
            Console.WriteLine($"Has sacado un {tirada} de ataque frente a una defensa CRÍTICA del enemigo.");
            Console.WriteLine("El enemigo resistió tu acometida Y TE CONTRAATACA.");
            TurnoOponente(true); // hace que los 2 siguientes ataques sean del enemigo. FALLA
            contraataqueEnemigo = false;
            Console.ReadLine();
        }
        else // defensa normal enemiga
        {
            Console.WriteLine($"Has sacado un {tirada} de ataque frente a una defensa de: {tiradaEnemigo} del enemigo.");
            Console.WriteLine("El enemigo resistió tu acometida.");
            Console.ReadLine();
        }

        if (contraataque) // atacamos de nuevo
        {
            Console.WriteLine("Inicias tu contraataque.");
            TurnoTuyo(true);
        }
    }

    public static void TurnoOponente(bool contraataqueEnemigo)
    {
        Console.WriteLine("\n--- TURNO DEL OPONENTE ---");
        Console.ReadLine();

        if (tiradaEnemigo > tirada)
        {
            if (tiradaEnemigo == 20) // crítico atk enemigo
            {
                if (pj.Vida - (enemigo.Ataque + 1) <= 0) // atk enemigo crítico que te mata
                {
                    Console.WriteLine($"El enemigo ha sacado un CRÍTICO (+1 a su ataque).");
                    Console.WriteLine("¡Has sido derrotado! ¡Fin de la partida!");
                    // pj.Vida = 0;
                    System.Environment.Exit(0);
                }
                else // atk crítico enemigo que no te mata
                {
                    Console.WriteLine($"El enemigo ha sacado un CRÍTICO (+1 a su ataque).");
                    pj.Vida -= enemigo.Ataque + 1;
                    Console.WriteLine($"Te quedan: {pj.Vida} puntos de vida.");
                    Console.ReadLine();
                }
            }
            else if (pj.Vida - enemigo.Ataque <= 0) // ataque normal enemigo que te mata
            {
                Console.WriteLine($"El enemigo ha sacado una tirada de {tiradaEnemigo} frente a tu defensa de {tirada}.");
                Console.WriteLine("¡Has sido derrotado! ¡Fin de la partida!");
                // pj.Vida = 0;
                System.Environment.Exit(0);
            }
            else // ataque normal enemigo que no te mata
            {
                Console.WriteLine($"El enemigo ha sacado una tirada de {tiradaEnemigo} frente a tu defensa de {tirada}.");
                Console.WriteLine("¡El enemigo ha atravesado tus defensas! Recibes: " + enemigo.Ataque + " puntos de daño.");
                pj.Vida -= enemigo.Ataque;
                Console.WriteLine($"Te quedan: {pj.Vida} puntos de vida.");
                Console.ReadLine();
            }
        }

        else if (tirada == 20) // defensa crítica tuya
        {
            Console.WriteLine($"Has sacado una DEFENSA CRÍTICA frente al ataque de {tiradaEnemigo} de tu enemigo.");
            Console.WriteLine("CONTRAATACAS A TU ENEMIGO.");
            TurnoTuyo(true);
            contraataque = false; // hace que los 2 siguientes ataques sean tuyos
            Console.ReadLine();
        }

        else // defensa normal tuya
        {
            Console.WriteLine($"El enemigo ha sacado una tirada de {tiradaEnemigo} frente a tu defensa de {tirada}.");
            Console.WriteLine("Tu defensa ha sido más fuerte que el ataque de tu enemigo.");
            Console.ReadLine();
        }
    }

}

