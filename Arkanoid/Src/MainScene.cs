using Godot;
using System.Linq;
using System;

public class MainScene : Node2D
{
    static public Lvl CurrentLevel { get; set; }

    [Export]
    public Lvl Level { get; set; } = Lvl.LEVEL1;
    public int NumberOfLives { get; set; } = 2;
    public int Score { get; set; } = 0;
    private int highScore = 0;
    private bool gameFinished = false;
    private Control ui;
    private Control livesContainer;
    private PackedScene boardIcon;
    private RichTextLabel scoreLabel;
    private RichTextLabel highScoreLabel;
    private StateMachine stateMachine = new StateMachine();    
    private LevelManager levelManager;
    private Timer winDelay;
    private GodMode gm;

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
        var blocks_count = GetNode<Node2D>("LevelRoot/Blocks").GetChildren().Cast<Block>().Count(b => b.Destructable);
        GD.Print("Checkin... ", blocks_count);
        if(blocks_count == 0)
        {
            Win();
        }
    }

    public void Win()
    {
        levelManager.Pause();
        winDelay.Start();
        GD.Print("Woohoo, level won, going to the next stage");
    }

    public void OnWin()
    {
        levelManager.AdvanceToNextLevel();
    }

    public void OnVictory()
    {
        var screen = GetNode<Control>("VictoryScreen");
        var scoreLabel = screen.GetNode<Label>("CenterContainer/VBoxContainer/Score");
        var scoreStr = GD.Str("SCORE: ", Score);
        if(Score > highScore)
        {
            SaveHighscore(Score);
            scoreStr += "\nNEW HIGHSCORE!!!";
        }
        scoreLabel.Text = scoreStr;

        gameFinished = true;
        screen.Visible = true;
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
        else if(body is Node2D obj)
        {
            obj.QueueFree();
        }
    }

    public void PostDestroy()
    {
        if(!gm.GodModeEnabled)
            NumberOfLives--;
        
        if(NumberOfLives >= 0)
        {
            if(!gm.GodModeEnabled)
                DecreaseNumberOfLifeIcons();
            
            levelManager.SoftReload();
            GD.Print("Ball entered death zone. ", NumberOfLives, " chances left!");
        }
        else
        {
            if(Score > highScore)
                SaveHighscore(Score);

            levelManager.Cleanup();
            var continueScreen = GetNode<Control>("ContinueScreen");
            continueScreen.Visible = true;
            continueScreen.GetNode<Timer>("Countdown").Start();
            continueScreen.GetNode<Control>("CenterContainer/VBoxContainer/HBoxContainer/YES").GrabFocus();
        }
    }

    public int GetHighscore()
    {
        var file = new File();
        if(!file.FileExists("user://arkanoid.data"))
        {
            return 5000;
        }

        file.Open("user://arkanoid.data", (int) File.ModeFlags.Read);
        var obj = file.GetLine();
        file.Close();

        return Int32.Parse(obj);
    }

    public void SaveHighscore(int highScore)
    {
        var file = new File();
        file.Open("user://arkanoid.data", (int) File.ModeFlags.Write);
        file.StoreLine(highScore.ToString());
        file.Close();
    }

    public override void _Ready()
    {
        levelManager = LevelManager.Init(this, GetNode<Round>("Round"));
        winDelay = GetNode<Timer>("WinDelay");

        //todo: UI manager should handle it
        boardIcon = GD.Load<PackedScene>("res://Resources/UI/BoardIcon.tscn");

        ui = GetNode<Control>("UI");
        livesContainer = ui.GetNode<Control>("LivesContainer");
        foreach(var i in Enumerable.Range(1, NumberOfLives))
        {
            IncreaseNumberOfLifeIcons();
        }

        highScore = GetHighscore();
        scoreLabel = GetNode<Control>("UI").GetNode<RichTextLabel>("Score");
        highScoreLabel = GetNode<Control>("UI").GetNode<RichTextLabel>("HighScore");
        highScoreLabel.Text = GD.Str("HIGH SCORE: ", System.Environment.NewLine, highScore);
        gm = GetNode<GodMode>("GodMode");

        levelManager.StartLoading(Level);

        PauseMode = PauseModeEnum.Process;
    }

    public override void _Process(float delta)
    {
        scoreLabel.Text = GD.Str("SCORE: ", Score);
        if(Score > highScore)
            highScoreLabel.Text = GD.Str("HIGH SCORE: ", System.Environment.NewLine, Score);
    }

    public override void _Input(InputEvent e)
    {
        if(e is InputEventKey && gameFinished)
        {
            GetTree().Paused = false;
            GetTree().ChangeScene("MainMenu.tscn");
        }
    }
}
