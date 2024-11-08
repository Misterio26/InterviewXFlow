public class ExtPlayer : Player
{
    public delegate void HealthChangedDelegate(int oldHealth, int newHealth);

    public event HealthChangedDelegate HealthChanged;
}

class ExtProgram : Program
{
    // Виджет, отображающий игроку здоровье.
    private static TextView healthView = new TextView();

    public static void ExtMain(string[] args)
    {
        // Вызов кода по созданию игрока.
        Main(args);

        healthView.Text = player.Health.ToString();
        player.HealthChanged += OnPlayerHealthChanged;

        // Ударяем игрока.
        HitPlayer();
    }

    private static void OnPlayerHealthChanged(int oldHealth, int newHealth)
    {
        healthView.Text = newHealth.ToString();
        if (newHealth - oldHealth < -10)
        {
            healthView.Color = Color.Red;
        }
        else
        {
            healthView.Color = Color.White;
        }
    }
}
