public class PlayerDeathEvent
{

}

public class PlayerRebirthEvent
{

}

public class GameWinEvent
{

}

public class GameLoseEvent
{

}

public class GameStartEvent
{

}

public class BubbleUpdateEvent
{
    public Bubble bubble;

    public BubbleUpdateEvent(Bubble bubble)
    {
        this.bubble = bubble;
    }
}

public class PlayerHealthUpdateEvent
{
    public float healthRemainPercentage;

    public PlayerHealthUpdateEvent(float healthRemain, float maxHealth)
    {
        healthRemainPercentage = healthRemain / maxHealth;
    }
}

public class BossHealthUpdateEvent
{
    public int healthRemain;

    public BossHealthUpdateEvent(int health)
    {
        healthRemain = health;
    }
}
