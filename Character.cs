
internal class Character(string nombre, int vida, int ataque)
{
    public string Nombre { get; init; } = nombre;
    public int Vida { get; set; } = vida;
    public int Ataque { get; set; } = ataque;
    private Random Dados { get; set; } = new Random();
    public string VerStats()
    {
        return Nombre + " tiene: " + Vida + " puntos de vida.\nUn índice de ataque de: " + Ataque;
    }
}

