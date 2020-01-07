using Godot;

#region Board States
public class Idle : IState
{
    private Board board;
    private StateMachine stateMachine;
    private Vector2 dir { get; } = new Vector2(0,0);


    public Idle(Board board, StateMachine stateMachine)
    {
        this.board = board;
        this.stateMachine = stateMachine;
    }

    public void Init(params object[] args) { board.Dir = dir; }
    public void Exit() {}
    public void Process(float dt) {}
    public void PhysicsProcess(float dt) {}

    public void HandleInput()
    {
        var left_pressed = Input.IsActionPressed("ui_left");
        var right_pressed = Input.IsActionPressed("ui_right");
        
        if(left_pressed && right_pressed)
            return;
        else if(left_pressed)
            stateMachine.ChangeState(nameof(MoveLeft));
        else if (right_pressed)
            stateMachine.ChangeState(nameof(MoveRight));
    }
}

public class MoveBase
{
    protected StateMachine stateMachine;

    protected void Move(Board board, Vector2 dir, float dt)
    {
        board.Dir = dir;

        var col = board.MoveAndCollide(board.Velocity*dt);
        if(col != null)
        {
            if(col.Collider is BasePowerUp powerUp)
            {
                powerUp.OnCollect();
            }
            else if(col.Collider is StaticBody2D body)
            {
                board.Dir = Vector2.Zero;
                if(PowerupManager.IsTeleportActive &&
                    body.Name == "right_col")
                {
                    stateMachine.ChangeState(nameof(Warping));
                }
            }
        }
    }
}

public class MoveLeft : MoveBase, IState
{
    private Board board;
    private Vector2 dir { get; } = new Vector2(-1, 0);

    public MoveLeft(Board board, StateMachine stateMachine)
    {
        this.board = board;
        this.stateMachine = stateMachine;
    }

    public void Init(params object[] args) { board.Dir = dir; }
    public void Exit() {}
    public void Process(float dt) {}

    public void HandleInput()
    {
        var left_pressed = Input.IsActionPressed("ui_left");
        var right_pressed = Input.IsActionPressed("ui_right");

        if(!left_pressed || right_pressed)
            stateMachine.ChangeState(nameof(Idle));
    }

    public void PhysicsProcess(float dt)
    {
        base.Move(board, dir, dt);
    }
}

public class MoveRight : MoveBase, IState
{
    private Board board;
    private Vector2 dir { get; } = new Vector2(1, 0);

    public MoveRight(Board board, StateMachine stateMachine)
    {
        this.board = board;
        this.stateMachine = stateMachine;
    }

    public void Init(params object[] args) { board.Dir = dir; }
    public void Exit() {}
    public void Process(float dt) {}

    public void HandleInput()
    {
        var left_pressed = Input.IsActionPressed("ui_left");
        var right_pressed = Input.IsActionPressed("ui_right");

        if(!right_pressed || left_pressed)
            stateMachine.ChangeState(nameof(Idle));
    }

    public void PhysicsProcess(float dt)
    {
        base.Move(board, dir, dt);
    }
}

public class Warping : IState
{
    private Board board;
    private float WarpSpeed = 100;

    public Warping(Board board)
    {
        this.board = board;
    }

    public void Exit()
    {
    }

    public void HandleInput()
    {
    }

    public void Init(params object[] args)
    {
        board.StartWarp();
    }

    public void PhysicsProcess(float dt)
    {
        board.Position += Vector2.Right*WarpSpeed*dt;
    }

    public void Process(float dt)
    {
    }
}
#endregion

public class BlastManager
{
    private Board board;
    private PackedScene blast;
    private MainScene scene;
    private Timer laserDelay;
    private bool laserReady = true;
    private int yOffset = -27;

    public void LaserReady()
    {
        laserReady = true;
    }
    public BlastManager(Board board, Timer laserDelay)
    {
        this.board = board;
        this.laserDelay = laserDelay;
    }

    public void Prepare()
    {
        blast = GD.Load<PackedScene>("res://Resources/Board/DoubleBlast.tscn");
        scene = board.GetNode<MainScene>("/root/Main");
    }

    public bool CanShoot()
    {
        return laserReady &&
            scene.GetTree().GetNodesInGroup("BLASTS").Count < 5;
    }

    public void Shoot()
    {
        laserReady = false;
        laserDelay.Start();

        var middle = board.Middle;
        InstanceBlast(new Vector2(middle.x, middle.y + yOffset));
    }

    private void InstanceBlast(Vector2 position)
    {
        var blastInstance = (Node2D) blast.Instance();
        blastInstance.Position = position;
        scene.AddChild(blastInstance);
    }
}

public class Board : KinematicBody2D
{
    [Export]
    public float Speed { get; set; } = 0.0f;
    public Vector2 Dir { get; set; } = new Vector2(0,0);
    public Vector2 Velocity { get => Speed*Dir; }
    public Vector2 Extents { get => shape.GetExtents()*Transform.Scale; }
    public Vector2 Middle { get => Position; }
    private StateMachine stateMachine = new StateMachine();
    private BlastManager blastManager;
    private RectangleShape2D shape;
    private Timer warpTimer;
    private AnimationPlayer animation;
    private bool extended { get; set; } = false;
    private bool laserActivated { get; set; } = false;

    public void Extend()
    {
        if(animation.CurrentAnimation != "laserDeactivate")
            animation.Play("extend");
        extended = true;
    }

    public void Shrink()
    {
        animation.Play("shrink");
        shape.SetExtents(new Vector2(Extents.x/1.5f, Extents.y));
        extended = false;
    }

    public void EnableLaser()
    {
        laserActivated = true;
        if(animation.CurrentAnimation != "extend")
            animation.Play("laserActivate");
    }

    public void DisableLaser()
    {
        laserActivated = false;
        animation.Play("laserDeactivate");
    }

    public void LaserReady()
    {
        blastManager.LaserReady();
    }

    public void ShootLaser()
    {
        if(Input.IsActionJustPressed("ui_accept")
            && blastManager.CanShoot()
        )
        {
            blastManager.Shoot();
            GD.Print("Psium, psium!");
        }
    }

    public void ResetState()
    {
        ResetPowerups();
        Position = GetNode<Node2D>("../BoardSpawnPoint").Position;
        stateMachine.ChangeState(nameof(Idle));
    }

    public void ResetPowerups()
    {
        if(extended)
            Shrink();
        else if(laserActivated)
            DisableLaser();

        blastManager.LaserReady();
    }

    public void StartWarp()
    {
        warpTimer.Start();
        LevelManager.Instance.Pause();
    }

    public void OnWarpEnd()
    {
        GetNode<MainScene>("/root/Main").LoadNextLevel = true;
        PowerupManager.DectivateTeleport();
    }

    public void Spawn()
    {
        animation.Play("spawn");
    }

    public void OnAnimationFinished(string name)
    {
        if(name == "spawn")
        {
            GD.Print("Spawn finished!");
            stateMachine.ChangeState(nameof(Idle));
        }
        else if(name == "shrink")
        {
            if(laserActivated)
                animation.Play("laserActivate");
        }
        else if(name == "laserActivate")
        {
            animation.Play("laserIdle");
        }
        else if(name == "laserDeactivate")
        {
            if(extended)
                animation.Play("extend");
        }
        else if(name == "extend")
        {
            shape.SetExtents(new Vector2(Extents.x*1.5f, Extents.y));
        }
    }

    public override void _Ready()
    {
        stateMachine.Add(nameof(Idle), new Idle(this, stateMachine));
        stateMachine.Add(nameof(MoveLeft), new MoveLeft(this, stateMachine));
        stateMachine.Add(nameof(MoveRight), new MoveRight(this, stateMachine));
        stateMachine.Add(nameof(Warping), new Warping(this));
        stateMachine.Add(nameof(EmptyState), new EmptyState());
        stateMachine.ChangeState(nameof(EmptyState));

        shape = (RectangleShape2D) this.GetNode<CollisionShape2D>("col").GetShape();
        warpTimer = GetNode<Timer>("WarpTimer");

        blastManager = new BlastManager(this, this.GetNode<Timer>("LaserDelay"));
        blastManager.Prepare();

        animation = GetNode<AnimationPlayer>("AnimationPlayer");

        PauseMode = PauseModeEnum.Process;
    }

    public override void _Process(float dt)
    {
        if(laserActivated)
        {
            ShootLaser();
        }

        stateMachine.HandleInput();
        stateMachine.Process(dt);
    }

    public override void _PhysicsProcess(float dt)
    {
        stateMachine.PhysicsProcess(dt);
    }
}
