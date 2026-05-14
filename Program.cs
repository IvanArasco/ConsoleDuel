using System.Text.RegularExpressions;

internal class Program
{
    static int Round = 0;
    static int Roll, EnemyRoll;
    static Character Pj = null;
    static Character EnemyCharacter = null;
    static Random Rolls = new Random();

    static void Main(string[] args)
    {
        Pj = CreateCharacter();
        Menu();
    }

    private static Character CreateCharacter()
    {
        while (true)
        {
            Console.WriteLine("\n--- Inserta nombre del personaje ---");
            string input = Console.ReadLine();

            if (Regex.IsMatch(input, @"^[a-zA-Z]+$"))
            {
                Console.WriteLine("\n--- ¡EMPIEZA EL JUEGO! ---");
                return new Character(input, 20, 1);
            }

            Console.WriteLine("El nombre solo puede contener letras.");
        }
    }

    private static int CheckOp(string input)
    {
        if (int.TryParse(input, out int result))
        {
            return result;
        }

        Console.WriteLine("No se ha reconocido la operación solicitada. Por favor, seleccione una válida.");
        return -1;
    }

    private static void Menu()
    {
        int op = -1;

        while (op != 0)
        {
            Console.WriteLine("\n--- SELECCIONA UNA OPERACIÓN ---");
            Console.WriteLine("1. Darse de piñas.");
            Console.WriteLine("2. Ver estadísticas.");
            Console.WriteLine("0. Salir del juego.");

            op = CheckOp(Console.ReadLine());

            switch (op)
            {
                case 1:
                    NextRound();
                    break;

                case 2:
                    Console.WriteLine("\n" + Pj.CheckStats());
                    Console.ReadLine();
                    break;

                case 0:
                    Environment.Exit(0);
                    break;
            }
        }
    }

    private static void NextRound()
    {
        int roundType;

        do
        {
            roundType = Rolls.Next(0, 3);

            Console.WriteLine("\n--- RONDA: " + Round + " ---");
            Round++;
            Console.ReadLine();

            switch (roundType)
            {
                case 0:
                    Console.WriteLine("¡Has sido atacado! Empieza la batalla... ¡Sobrevive!");
                    StartFight(false); // no te han emboscado
                    break;

                case 1:
                    Console.WriteLine("¡Has sido emboscado! Pierdes 1 punto de vida automáticamente...");
                    Pj.Life--;
                    StartFight(true); // te emboscaron
                    break;

                case 2:
                    Console.WriteLine("Caminas con tranquilidad... no hay amenazas cerca. Descansas. +1 vida.");
                    Pj.Life++;
                    break;
            }

        } while (roundType == 2); // repetir mientras no haya combate
    }

    private static void StartFight(bool emboscado)
    {
        EnemyCharacter = new Character("NPC", Rolls.Next(2, 4), Rolls.Next(1, 3));

        Roll = RollDice();
        EnemyRoll = RollDice();

        bool yourRound = Roll > EnemyRoll; // resultado de iniciativa, quien ataca primero

        while (EnemyCharacter.Life > 0)
        {
            Console.WriteLine("\n--- ESTADÍSTICAS DE TU OPONENTE ---");
            Console.WriteLine(EnemyCharacter.CheckStats());

            if (yourRound)
            {
                YourRound();
            }
            else
            {
                EnemyRound();
            }

            yourRound = !yourRound;

        }

        Menu();
    }

    private static void YourRound()
    {
        Console.WriteLine("\n--- TU TURNO ---");
        Console.ReadLine();

        int atkRoll = RollDice();
        int enemyDefRoll = RollDice();

        Console.WriteLine($"Tu tirada de ataque: {atkRoll}. El enemigo saca: {enemyDefRoll} de defensa.");

        if (atkRoll > enemyDefRoll)
        {
            int atk = atkRoll == 20 ? Pj.Atk + 1 : Pj.Atk;

            if (atkRoll == 20)
                Console.WriteLine("¡CRÍTICO! (+1 de daño)");

            if (EnemyCharacter.Life - atk <= 0)
            {
                Console.WriteLine("¡Has derrotado a tu enemigo!");
                EnemyCharacter.Life = 0;
            }
            else
            {
                EnemyCharacter.Life -= atk;
                Console.WriteLine($"Atravesaste la defensa del enemigo. Daño infligido: {atk}. Vida del enemigo: {EnemyCharacter.Life}");
            }
        }
        else if (enemyDefRoll == 20)
        {
            Console.WriteLine("¡DEFENSA CRÍTICA del enemigo! Te contraataca.");
            EnemyRound();
            return;
        }
        else
        {
            Console.WriteLine("El enemigo resistió tu acometida.");
        }

        Console.ReadLine();
    }

    private static void EnemyRound()
    {
        Console.WriteLine("\n--- TURNO DEL OPONENTE ---");
        Console.ReadLine();

        int enemyAttkRoll = RollDice();
        int defRoll = RollDice();

        Console.WriteLine($"Tirada de ataque enemiga: {enemyAttkRoll}. Tu defensa: {defRoll}.");

        if (enemyAttkRoll > defRoll)
        {
            int attkEnemy = enemyAttkRoll == 20 ? EnemyCharacter.Atk + 1 : EnemyCharacter.Atk;

            if (enemyAttkRoll == 20)
                Console.WriteLine("¡CRÍTICO ENEMIGO! (+1 de daño)");

            if (Pj.Life - attkEnemy <= 0)
            {
                Console.WriteLine("¡Has sido derrotado! ¡Fin de la partida!");
                Environment.Exit(0);
            }
            else
            {
                Pj.Life -= attkEnemy;
                Console.WriteLine($"¡El enemigo atravesó tus defensas! Daño recibido: {attkEnemy}. Tu vida: {Pj.Life}");
            }
        }
        else if (defRoll == 20)
        {
            Console.WriteLine("¡DEFENSA CRÍTICA! Contraatacas al enemigo.");
            YourRound();
            return;
        }
        else
        {
            Console.WriteLine("Resististe el ataque del enemigo.");
        }

        Console.ReadLine();
    }

    private static int RollDice()
    {
        return Rolls.Next(1, 21);
    }
}