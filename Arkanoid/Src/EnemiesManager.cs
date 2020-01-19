using Godot;
using System.Collections.Generic;

public struct LevelInfo
{
    public string enemyType;
    public int maxEnemiesOnLevel;
    public int maxSpawnTime;

    public LevelInfo(string enemyType, int maxEnemiesOnLevel, int maxSpawnTime)
    {
        this.enemyType = enemyType;
        this.maxEnemiesOnLevel = maxEnemiesOnLevel;
        this.maxSpawnTime = maxSpawnTime;
    }
}

public class EnemiesManager : Node2D
{
    [Export]
    public int MaxEnemiesOnLevel { get; set; }= 3;
    public string EnemyToSpawn { get; set; } = "triangle";
    private int enemiesAmountOnLevel = 1;
    private int spawnedCounter = 0;
    private Arena arena;
    private PackedScene enemyScene;
    private Timer spawnTimer;
    private Timer enableTimer;
    private Timer maxTimer;
    private Node2D levelRoot;

    Dictionary<Lvl, LevelInfo> enemiesInfo = new Dictionary<Lvl, LevelInfo>()
    {
        {Lvl.LEVEL1, new LevelInfo("triangle", 2, 30)},
        {Lvl.LEVEL2, new LevelInfo("square", 1, 1)},
        {Lvl.LEVEL3, new LevelInfo("folded", 3, 1)},
        {Lvl.LEVEL4, new LevelInfo("origami", 2, 1)},
        {Lvl.LEVEL5, new LevelInfo("triangle", 3, 1)}
    };

    public void EnableSpawning()
    {
        spawnTimer.Start();
    }

    public void DisableSpawning()
    {
        enemiesAmountOnLevel = 1;
        spawnTimer.Stop();
    }

    public void MaxAmountOfEnemies()
    {
        enemiesAmountOnLevel = MaxEnemiesOnLevel;
    }

    public void SetLevel(Lvl level)
    {
        var info = enemiesInfo[level];
        EnemyToSpawn = info.enemyType;
        MaxEnemiesOnLevel = info.maxEnemiesOnLevel;
        enableTimer.WaitTime = info.maxSpawnTime;
        maxTimer.WaitTime = info.maxSpawnTime;
    }

    public void Reset()
    {
        spawnTimer.Stop();
        enableTimer.Stop();
        maxTimer.Stop();
        enableTimer.Start();
        maxTimer.Start();
    }

    public void Spawn()
    {
        if(GetTree().GetNodesInGroup("ENEMIES").Count < enemiesAmountOnLevel)
        {
            var instance = (Enemy) enemyScene.Instance();
            instance.EnemyType = EnemyToSpawn;
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
        enableTimer = GetNode<Timer>("EnableTimer");
        maxTimer = GetNode<Timer>("MaxTimer");
        levelRoot = GetNode<Node2D>("../LevelRoot");
    }
}
