using Godot;
using System.Collections.Generic;
using System.Linq;

public enum Lvl
{
    LEVEL1,
    LEVEL2,
    LEVEL3,
    LEVEL4,
    LEVEL5
}

public class MainScene : Node2D
{
    [Export]
    public Lvl Level { get; set; } = Lvl.LEVEL1;
    public int NumberOfLives { get; set; } = 2;
    public int Score { get; set; } = 0;
    private Node2D blocks;
    private Control ui;
    private Control livesContainer;
    private RichTextLabel scoreLabel;
    private PackedScene boardIcon;
    private Vector2 levelLoadingPoint { get => new Vector2(544,160); }
    
    private Dictionary<Lvl, string> levels = new Dictionary<Lvl, string>()
    {
        {Lvl.LEVEL1, "1 level.tscn"},
        {Lvl.LEVEL2, "2 level.tscn"},
        {Lvl.LEVEL3, "3 level.tscn"},
        {Lvl.LEVEL4, "4 level.tscn"},
        {Lvl.LEVEL5, "5 level.tscn"}
    };
    void LoadLevel(Lvl level)
    {
        GD.Print("Loading level ", string.Format("res://Resources/Levels/{0}", levels[level]));
        var levelScene = GD.Load<PackedScene>(string.Format("res://Resources/Levels/{0}", levels[level]));
        var levelInstance = (Node2D) levelScene.Instance();
        levelInstance.Position = levelLoadingPoint;
        AddChild(levelInstance);
    }
    void CheckIfPlayerDestroyedAllBlocks()
    {
        var blocks_count = blocks.GetChildren().Cast<Block>().Count(b => b.Destructable);
        GD.Print("Checkin... ", blocks_count);
        if(blocks_count == 0)
        {
            GD.Print("Woohoo, level won, going to the next stage");
        }
    }

    void DecreaseNumberOfIcons()
    {
        var children = livesContainer.GetChildren().Cast<TextureRect>().ToArray();
        children[children.Length - 1].QueueFree();
    }

    public void OnDeathAreaEntered(PhysicsBody2D body)
    {
        if (body is Ball ball)
        {
            NumberOfLives--;
            if(NumberOfLives >= 0)
            {
                DecreaseNumberOfIcons();
                ball.ResetState();
                GD.Print("Ball entered death zone. ", NumberOfLives, " chances left!");
            }
            else
            {
                GD.Print("Game over");
                GetTree().ReloadCurrentScene();
            }
        }
    }

    public override void _Ready()
    {

        GetNode("Ball").Connect("CheckWin", this, nameof(CheckIfPlayerDestroyedAllBlocks));
        
        ui = GetNode<Control>("UI");
        boardIcon = GD.Load<PackedScene>("res://Resources/UI/BoardIcon.tscn");
        livesContainer = ui.GetNode<Control>("LivesContainer");
        foreach(var i in Enumerable.Range(1, NumberOfLives))
        {
            livesContainer.AddChild(boardIcon.Instance());
        }

        scoreLabel = ui.GetNode<RichTextLabel>("Score");
        
        LoadLevel(Level);
        blocks = GetNode<Node2D>("Blocks");
    }

    public override void _Process(float delta)
    {
        scoreLabel.Text = GD.Str("Score: ", Score);
    }
}
