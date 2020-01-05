using Godot;
using System.Collections.Generic;

public enum Lvl
{
    LEVEL1,
    LEVEL2,
    LEVEL3,
    LEVEL4,
    LEVEL5
}

class LevelManager
{
    static public LevelManager Instance { get; private set; } = null;
    MainScene scene;
    PackedScene ball;
    Ball ballInstance;
    PackedScene board;
    Board boardInstance;
    Node2D levelInstance;
    Lvl currentLevel;
    Vector2 levelLoadingPoint;
    Dictionary<Lvl, string> levels = new Dictionary<Lvl, string>()
    {
        {Lvl.LEVEL1, "1 level.tscn"},
        {Lvl.LEVEL2, "2 level.tscn"},
        {Lvl.LEVEL3, "3 level.tscn"},
        {Lvl.LEVEL4, "4 level.tscn"},
        {Lvl.LEVEL5, "5 level.tscn"}
    };

    public LevelManager(MainScene scene)
    {
        this.scene = scene;
    }

    static public LevelManager Init(MainScene scene)
    {
        Instance = new LevelManager(scene);

        Instance.ball = GD.Load<PackedScene>("res://Resources/Ball/Ball.tscn");
        Instance.board = GD.Load<PackedScene>("res://Resources/Board/Board.tscn");

        return Instance;
    }

    public void LoadLevel(Lvl level)
    {
        currentLevel = level;

        boardInstance = (Board) board.Instance();
        boardInstance.Position = scene.GetNode<Node2D>("BoardSpawnPoint").Position;

        ballInstance = (Ball) ball.Instance();
        ballInstance.Board = boardInstance;
        ballInstance.Position = scene.GetNode<Node2D>("BallSpawnPoint").Position;

        var levelScene = GD.Load<PackedScene>(string.Format("res://Resources/Levels/{0}", levels[level]));
        levelInstance = (Node2D) levelScene.Instance();
        levelInstance.Position = scene.GetNode<Node2D>("LevelSpawnPoint").Position;;

        scene.AddChild(boardInstance);
        scene.AddChild(ballInstance);
        scene.AddChild(levelInstance);

        scene.Blocks = scene.GetNode<Node2D>("Blocks");
    }

    public void AdvanceToNextLevel()
    {
        Cleanup();
        LoadLevel(++currentLevel);
        Unpause();
    }

    public void SoftReload(Ball ball)
    {
        PowerupManager.ResetPowerups();

        boardInstance.ResetState();
        ball.ResetState();
        FreeGroups(new string[]{"POWERUPS", "BLASTS", "ENEMIES"});
        Unpause();
    }
    
    private void CleanGroup(string group)
    {
        var tree = scene.GetTree();
        foreach(Node2D obj in tree.GetNodesInGroup(group))
        {
            obj.QueueFree();
        }
    }

    public void Pause()
    {
        scene.GetTree().Paused = true;
    }

    public void Unpause()
    {
        scene.GetTree().Paused = false;
    }

    private void Cleanup()
    {
        PowerupManager.ResetPowerups();

        boardInstance.Free();
        levelInstance.Free();
        FreeGroups(new string[]{"BALLS", "POWERUPS", "BLASTS", "ENEMIES"});
    }

    private void FreeGroups(string[] groups)
    {
        foreach(var group in groups)
        {
            CleanGroup(group);
        }
    }
}
