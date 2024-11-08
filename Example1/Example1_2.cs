[Serializable]
public class Player
{
    public int Health { get; private set; }

    public Player() { }

    public void Hit(int damage)
    {
        Health -= damage;
    }
}

[Serializable]
public class Settings
{
    public int Damage { get; }
}

public class Program
{
    private const string NewPlayerPath = "NewPlayer.json";
    private const string SettingsPath = "Settings.json";

    protected static Player player;

    public static void Main(string[] args)
    {
        // Создаем нового игрока.
        player = Serializer.LoadFromFile<Player>(NewPlayerPath);
        // Ударяем игрока.
        var settings = Serializer.LoadFromFile<Settings>(SettingsPath);
        player.Hit(settings.Damage);
    }
}
