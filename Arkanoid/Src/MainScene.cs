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
    public bool LoadNextLevel { get; set; } = false;
    private int highScore { get; set; } = 0;
    private Control ui;
    private Control livesContainer;
    private PackedScene boardIcon;
    private RichTextLabel scoreLabel;
    private RichTextLabel highScoreLabel;
    private StateMachine stateMachine = new StateMachine();    
    private LevelManager levelManager;
    private EnemiesManager enemiesManager;

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
                levelManager.Pause();
                GetNode<Board>("Board").Destroy();
            }
        }
        else if(body is BasePowerUp powerUp)
        {
            PowerupManager.RegainPowerup(powerUp);
        }
    }

    public void PostDestroy()
    {
        NumberOfLives--;
        if(NumberOfLives >= 0)
        {
            DecreaseNumberOfLifeIcons();
            levelManager.SoftReload();
            GD.Print("Ball entered death zone. ", NumberOfLives, " chances left!");
        }
        else
        {
            GetTree().ReloadCurrentScene();

            GD.Print("Game over");
        }
    }

    public override void _Ready()
    {
        levelManager = LevelManager.Init(this, GetNode<Round>("Round"));
        enemiesManager = GetNode<EnemiesManager>("EnemiesManager");
        enemiesManager.EnableSpawning();

        //todo: UI manager should handle it
        boardIcon = GD.Load<PackedScene>("res://Resources/UI/BoardIcon.tscn");

        ui = GetNode<Control>("UI");
        livesContainer = ui.GetNode<Control>("LivesContainer");
        foreach(var i in Enumerable.Range(1, NumberOfLives))
        {
            IncreaseNumberOfLifeIcons();
        }

        scoreLabel = GetNode<Control>("UI").GetNode<RichTextLabel>("Score");
        highScoreLabel = GetNode<Control>("UI").GetNode<RichTextLabel>("HighScore");
        highScoreLabel.Text = GD.Str("HIGH SCORE: ", System.Environment.NewLine, highScore);

        levelManager.StartLoading(Level);

        PauseMode = PauseModeEnum.Process;
    }

    public override void _Process(float delta)
    {
        scoreLabel.Text = GD.Str("SCORE: ", Score);
        if(LoadNextLevel)
        {
            LoadNextLevel = false;
            levelManager.AdvanceToNextLevel();
        }
    }
}
