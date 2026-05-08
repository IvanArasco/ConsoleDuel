
internal class Personaje(string nombre, int vida, int ataque, int defensa)
{
    public string Nombre { get; init; } = nombre;
    public int Vida { get; set; } = vida;
    public int Ataque { get; set; } = ataque;
    public int Defensa { get; set; } = defensa;
    private Random Dados { get; set; }

    public int TirarDado()
    {
        Dados = new Random();
        return Dados.Next(1, 21);
    }
    public string VerStats()
    {
        return Nombre + " tiene: " + Vida + " puntos de vida.\nUn índice de ataque de: " + Ataque + "\nUn índice de defensa de: " + Defensa;
    }
}

