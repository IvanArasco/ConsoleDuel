
internal class Character(string nombre, int vida, int ataque)
{
    public string Nombre { get; init; } = nombre;
    public int Life { get; set; } = vida;
    public int Atk { get; set; } = ataque;
    private Random Dados { get; set; } = new Random();
    public string CheckStats()
    {
        return Nombre + " tiene: " + Life + " puntos de vida.\nUn índice de ataque de: " + Atk;
    }
}

