using Godot;

public class EnemiesManager : Node2D
{
    [Export]
    public int MaxEnemiesOnLevel = 3;
    private int enemiesAmountOnLevel = 1;
    private Arena arena;
    private PackedScene enemyScene;
    private Timer spawnTimer;
    private Node2D levelRoot;
    private int spawnedCounter = 0;

    public void EnableSpawning()
    {
        spawnTimer.Start();
    }

    public void DisableSpawning()
    {
        spawnTimer.Stop();
    }

    public void MaxAmountOfEnemies()
    {
        enemiesAmountOnLevel = MaxEnemiesOnLevel;
    }

    public void Spawn()
    {
        if(GetTree().GetNodesInGroup("ENEMIES").Count < enemiesAmountOnLevel)
        {
            var instance = (Enemy) enemyScene.Instance();
            if(spawnedCounter % 2 == 0)
            {
                arena.OpenLeftGate();
                instance.Position = arena.LeftSpawnPoint;
            }
            else
            {
                arena.OpenRightGate();
                instance.Position = arena.RightSpawnPoint;
            }

            levelRoot.AddChild(instance);
            ++spawnedCounter;
        }
    }

    public override void _Ready()
    {
        enemyScene = GD.Load<PackedScene>("res://Resources/Enemies/Enemy.tscn");
        arena = GetNode<Arena>("../Arena");
        spawnTimer = GetNode<Timer>("SpawnTimer");
        levelRoot = GetNode<Node2D>("../LevelRoot");
    }
}
