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
            stateMachine.ChangeState("MoveLeft");
        else if (right_pressed)
            stateMachine.ChangeState("MoveRight");
    }
}

public class MoveBase
{
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
            else if(col.Collider is StaticBody2D)
                board.Dir = Vector2.Zero;
        }
    }
}

public class MoveLeft : MoveBase, IState
{
    private Board board;
    private StateMachine stateMachine;
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
            stateMachine.ChangeState("Idle");
    }

    public void PhysicsProcess(float dt)
    {
        base.Move(board, dir, dt);
    }
}

public class MoveRight : MoveBase, IState
{
    private Board board;
    private StateMachine stateMachine;
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
            stateMachine.ChangeState("Idle");
    }

    public void PhysicsProcess(float dt)
    {
        base.Move(board, dir, dt);
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
    private int xOffset = 25;
    private int yOffset = 10;

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
        blast = GD.Load<PackedScene>("res://Resources/Board/Blast.tscn");
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
        InstanceBlast(new Vector2(middle.x + xOffset, middle.y + yOffset));
        InstanceBlast(new Vector2(middle.x - xOffset, middle.y + yOffset));
    }

    private void InstanceBlast(Vector2 position)
    {
        var blastInstance = (Blast) blast.Instance();
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
    private Timer laserDelay;
    private bool extended { get; set; } = false;
    private bool laserActivated { get; set; } = false;

    public void Extend()
    {
        this.ChangeSize(2.0f, 1.0f);
        extended = true;
    }

    public void Shrink()
    {
        this.ChangeSize(0.5f, 1.0f);
        extended = false;
    }

    public void EnableLaser()
    {
        laserActivated = true;
        //implement animation
    }

    public void DisableLaser()
    {
        laserActivated = false;
        //implement animation
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
        if(extended)
            this.Shrink();

        laserActivated = false;
        blastManager.LaserReady();
        Position = GetNode<Node2D>("../BoardSpawnPoint").Position;
    }

    private void ChangeSize(float xScale, float yScale)
    {
        var newTransform = GetTransform();
        newTransform.Scale = new Vector2(newTransform.Scale.x*xScale, newTransform.Scale.y*yScale);
        SetTransform(newTransform);
    }

    public override void _Ready()
    {
        stateMachine.Add("Idle", new Idle(this, stateMachine));
        stateMachine.Add("MoveLeft", new MoveLeft(this, stateMachine));
        stateMachine.Add("MoveRight", new MoveRight(this, stateMachine));
        stateMachine.ChangeState("Idle");

        shape = (RectangleShape2D) this.GetNode<CollisionShape2D>("col").GetShape();

        blastManager = new BlastManager(this, this.GetNode<Timer>("LaserDelay"));
        blastManager.Prepare();
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
