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

public class LevelLoading : IState
{
    Lvl levelToLoad;
    MainScene scene;
    StateMachine stateMachine;
    Vector2 levelLoadingPoint { get; set; }
    private Dictionary<Lvl, string> levels = new Dictionary<Lvl, string>()
    {
        {Lvl.LEVEL1, "1 level.tscn"},
        {Lvl.LEVEL2, "2 level.tscn"},
        {Lvl.LEVEL3, "3 level.tscn"},
        {Lvl.LEVEL4, "4 level.tscn"},
        {Lvl.LEVEL5, "5 level.tscn"}
    };


    public LevelLoading(MainScene scene, StateMachine stateMachine)
    {
        this.scene = scene;
        this.stateMachine = stateMachine;
    }
    public void Exit()
    {
    }

    public void HandleInput()
    {
    }

    public void Init(params object[] args)
    {
        levelToLoad = (Lvl) args[0];
        levelLoadingPoint = (Vector2) args[1];
    }

    public void PhysicsProcess(float dt)
    {
    }

    public void Process(float dt)
    {
        var levelScene = GD.Load<PackedScene>(string.Format("res://Resources/Levels/{0}", levels[levelToLoad]));
        var levelInstance = (Node2D) levelScene.Instance();
        levelInstance.Position = levelLoadingPoint;
        scene.AddChild(levelInstance);

        scene.Blocks = scene.GetNode<Node2D>("Blocks");
        stateMachine.ChangeState(nameof(Game));
    }
}

public class Game : IState
{
    private RichTextLabel scoreLabel { get; set; }
    private MainScene scene { get; set; }

    public Game(MainScene scene)
    {
        this.scene = scene;
    }

    public void Exit()
    {
    }

    public void HandleInput()
    {
    }

    public void Init(params object[] args)
    {
        scoreLabel =
            scene.GetNode<Control>("UI")
            .GetNode<RichTextLabel>("Score");
    }

    public void PhysicsProcess(float dt)
    {
    }

    public void Process(float dt)
    {
        scoreLabel.Text = GD.Str("Score: ", scene.Score);
    }
}

public class MainScene : Node2D
{
    [Export]
    public Lvl Level { get; set; } = Lvl.LEVEL1;
    [Export]
    public Vector2 LevelLoadingPoint { get; set; } =  new Vector2(544,160);
    public Lvl CurrentLevel { get; set; }
    public int NumberOfLives { get; set; } = 2;
    public int Score { get; set; } = 0;
    public Node2D Blocks { private get; set; }
    private Control ui;
    private Control livesContainer;
    private PackedScene boardIcon;
    private StateMachine stateMachine = new StateMachine();    

    void CheckIfPlayerDestroyedAllBlocks()
    {
        var blocks_count = Blocks.GetChildren().Cast<Block>().Count(b => b.Destructable);
        GD.Print("Checkin... ", blocks_count);
        if(blocks_count == 0)
        {
            GD.Print("Woohoo, level won, going to the next stage");
        }
    }

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
        stateMachine.Add(nameof(LevelLoading), new LevelLoading(this, stateMachine));
        stateMachine.Add(nameof(Game), new Game(this));

        GetNode("Ball").Connect("CheckWin", this, nameof(CheckIfPlayerDestroyedAllBlocks));
        
        boardIcon = GD.Load<PackedScene>("res://Resources/UI/BoardIcon.tscn");

        ui = GetNode<Control>("UI");
        livesContainer = ui.GetNode<Control>("LivesContainer");
        foreach(var i in Enumerable.Range(1, NumberOfLives))
        {
            IncreaseNumberOfLifeIcons();
        }

        CurrentLevel = Level;
        stateMachine.ChangeState(nameof(LevelLoading), Level, LevelLoadingPoint);
    }

    public override void _Process(float delta)
    {
        stateMachine.Process(delta);
    }
}
