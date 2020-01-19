using Godot;

#region Board States
public static class BoardInput
{
    public static bool LeftPressed { get => Input.IsActionPressed("ui_left"); }
    public static bool RightPressed { get => Input.IsActionPressed("ui_right"); }
    public static bool ActionPressed { get => Input.IsActionPressed("ui_accept"); }
}
public class Idle : IState
{
    private Board board;
    private StateMachine stateMachine;
    private Vector2 dir = Vector2.Zero;

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
        var left_pressed = BoardInput.LeftPressed;
        var right_pressed = BoardInput.RightPressed;
        
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
            if(col.Collider is Ball ball)
            {
                ball.Audio.BoardHit();
                ball.CurrentDir = Bounce.BoardBounce(ball, board.Position, board.Extents, col.Position);
            }
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
            else if(col.Collider is IHittable obj)
            {
                obj.OnHit();
            }
        }
    }
}

public class MoveLeft : MoveBase, IState
{
    private Board board;
    private Vector2 dir = Vector2.Left;

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
        if(!BoardInput.LeftPressed || BoardInput.RightPressed)
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
    private Vector2 dir = Vector2.Right;

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
        if(!BoardInput.RightPressed || BoardInput.LeftPressed)
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
    private float WarpSpeed = 100.0f;

    public Warping(Board board)
    {
        this.board = board;
    }

    public void Exit() {}
    public void HandleInput() {}
    public void Process(float dt) {}
    public void Init(params object[] args)
    {
        board.StartWarp();
    }

    public void PhysicsProcess(float dt)
    {
        board.Position += Vector2.Right*WarpSpeed*dt;
    }
}
#endregion

#region Laser
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
            scene.GetTree().GetNodesInGroup("BLASTS").Count < 3;
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
#endregion

public class Board : KinematicBody2D
{
    [Export]
    public float Speed { get; set; } = 0.0f;
    public Vector2 Dir { get; set; } = Vector2.Zero;
    public Vector2 Velocity { get => Speed*Dir; }
    public Vector2 Extents { get => shape.GetExtents()*col.Scale; }
    public Vector2 Middle { get => Position; }
    public bool IsLaserActive { get; private set; } = false;
    public bool IsExtended { get; private set; } = false;
    private StateMachine stateMachine = new StateMachine();
    private BlastManager blastManager;
    private RectangleShape2D shape;
    private CollisionShape2D col;
    private Timer warpTimer;
    private AnimationPlayer animation;
    private Node2D spawnPoint;
    private AudioManager audio;

    public void Extend()
    {
        if(animation.CurrentAnimation != "laserDeactivate")
        {   
            audio.Extend();
            animation.Play("extend");
        }
        IsExtended = true;
    }

    public void Shrink()
    {
        animation.Play("shrink");
        IsExtended = false;
    }

    public void EnableLaser()
    {
        IsLaserActive = true;
        if(animation.CurrentAnimation != "shrink")
            animation.Play("laserActivate");
    }

    public void DisableLaser()
    {
        IsLaserActive = false;
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
            audio.LaserShot();
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
        if(IsExtended)
            Shrink();
        else if(IsLaserActive)
            DisableLaser();

        blastManager.LaserReady();
    }

    public void StartWarp()
    {
        audio.Warp();
        warpTimer.Start();
        LevelManager.Instance.Pause();
    }

    public void OnWarpEnd()
    {
        GetNode<MainScene>("/root/Main").Win();
        PowerupManager.DectivateTeleport();
    }

    public void Spawn()
    {
        animation.Play("spawn");
    }

    public void Destroy()
    {
        audio.Death();
        animation.Play("destroy");
        stateMachine.ChangeState(nameof(EmptyState));
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
            if(IsLaserActive)
                animation.Play("laserActivate");
        }
        else if(name == "laserActivate")
        {
            animation.Play("laserIdle");
        }
        else if(name == "laserDeactivate")
        {
            if(IsExtended)
            {
                audio.Extend();
                animation.Play("extend");
            }
        }
        else if(name == "destroy")
        {
            GetNode<MainScene>("..").PostDestroy();
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

        col = GetNode<CollisionShape2D>("col");
        shape = (RectangleShape2D) col.GetShape();
        warpTimer = GetNode<Timer>("WarpTimer");
        audio = GetNode<AudioManager>("../AudioManager");

        blastManager = new BlastManager(this, this.GetNode<Timer>("LaserDelay"));
        blastManager.Prepare();

        animation = GetNode<AnimationPlayer>("AnimationPlayer");
        spawnPoint = GetNode<Node2D>("../BoardSpawnPoint");

        PauseMode = PauseModeEnum.Process;
    }

    public override void _Process(float dt)
    {
        if(IsLaserActive)
        {
            ShootLaser();
        }

        stateMachine.HandleInput();
        stateMachine.Process(dt);
    }

    public override void _PhysicsProcess(float dt)
    {
        stateMachine.PhysicsProcess(dt);
        Position = new Vector2(Position.x, spawnPoint.Position.y);
    }
}
