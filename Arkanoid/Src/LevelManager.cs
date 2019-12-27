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

    public LevelManager(MainScene scene, Vector2 levelLoadingPoint)
    {
        this.scene = scene;
        this.levelLoadingPoint = levelLoadingPoint;
    }

    public void Init()
    {
        ball = GD.Load<PackedScene>("res://Resources/Ball/Ball.tscn");
        board = GD.Load<PackedScene>("res://Resources/Board/Board.tscn");
    }

    public void LoadLevel(Lvl level)
    {
        currentLevel = level;
        ballInstance = (Ball) ball.Instance();
        boardInstance = (Board) board.Instance();

        var levelScene = GD.Load<PackedScene>(string.Format("res://Resources/Levels/{0}", levels[level]));
        levelInstance = (Node2D) levelScene.Instance();
        levelInstance.Position = levelLoadingPoint;
        scene.AddChild(levelInstance);
        scene.Blocks = scene.GetNode<Node2D>("Blocks");
    }

    public void AdvanceToNextLevel()
    {
        this.Cleanup();
    }

    private void Cleanup()
    {

    }
}
