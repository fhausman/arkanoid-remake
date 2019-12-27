using Godot;
using System.Linq;

public class MainScene : Node2D
{
    [Export]
    public Lvl Level { get; set; } = Lvl.LEVEL1;
    public Lvl CurrentLevel { get; set; }
    public int NumberOfLives { get; set; } = 2;
    public int Score { get; set; } = 0;
    public Node2D Blocks { private get; set; }
    private Control ui;
    private Control livesContainer;
    private PackedScene boardIcon;
    private RichTextLabel scoreLabel;
    private StateMachine stateMachine = new StateMachine();    
    private LevelManager levelManager;

    void DecreaseNumberOfLifeIcons()
    {
        var children = livesContainer.GetChildren().Cast<TextureRect>().ToArray();
        children[children.Length - 1].QueueFree();
    }
    
    void IncreaseNumberOfLifeIcons()
    {
        livesContainer.AddChild(boardIcon.Instance());
    }

    public void AddExtraLife()
    {
        NumberOfLives++;
        IncreaseNumberOfLifeIcons();
    }

    public void CheckIfPlayerDestroyedAllBlocks()
    {
        var blocks_count = Blocks.GetChildren().Cast<Block>().Count(b => b.Destructable);
        GD.Print("Checkin... ", blocks_count);
        if(blocks_count == 0)
        {
            GD.Print("Woohoo, level won, going to the next stage");
            levelManager.AdvanceToNextLevel();
        }
    }

    public void OnDeathAreaEntered(PhysicsBody2D body)
    {
        if (body is Ball ball)
        {
            if(GetTree().GetNodesInGroup("BALLS").Count() > 1)
            {
                ball.Free();
            }
            else
            {
                NumberOfLives--;
                if(NumberOfLives >= 0)
                {
                    DecreaseNumberOfLifeIcons();
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
    }

    public override void _Ready()
    {
        levelManager = new LevelManager(this);
        levelManager.Init();

        boardIcon = GD.Load<PackedScene>("res://Resources/UI/BoardIcon.tscn");

        ui = GetNode<Control>("UI");
        livesContainer = ui.GetNode<Control>("LivesContainer");
        foreach(var i in Enumerable.Range(1, NumberOfLives))
        {
            IncreaseNumberOfLifeIcons();
        }

        scoreLabel = GetNode<Control>("UI").GetNode<RichTextLabel>("Score");

        levelManager.LoadLevel(Level);
    }

    public override void _Process(float delta)
    {
        scoreLabel.Text = GD.Str("Score: ", Score);
    }
}
