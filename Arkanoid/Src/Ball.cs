using Godot;
using System;

#region Ball States
public class Moving : IState
{
    private Ball ball;
    public Vector2 Dir { get; set; } = new Vector2(-1, -1);

    public Moving(Ball ball)
    {
        this.ball = ball;
    }

    public void Init(params object[] args) { Dir = (Vector2)args[0]; }
    public void Exit() { }
    public void HandleInput() { }
    public void Process(float dt)
    {
        if (Dir == Vector2.Left || Dir == Vector2.Right)
            Dir = Utils.RotateVector(Dir, 5.0f);
    }

    public void PhysicsProcess(float dt)
    {
        var col = ball.MoveAndCollide(Dir * ball.CurrentSpeed * dt);
        if (col != null)
        {
            if (col.Collider is Board board)
            {
                if (ball.GlueToBoard)
                {
                    AttachBallToBoard(board);
                    return;
                }

                ball.Audio.BoardHit();
                Dir = BoardBounce(col, board);
            }
            else
            {
                Dir = NaturalBounce(col);
            }

            if (col.Collider is IHittable obj)
            {
                obj.OnHit();
                if (!(obj is GoldenBlock))
                    ball.SpeedUp(ball.BlockHitSpeedUp);
            }
        }
    }

    private void AttachBallToBoard(Board board)
    {
        var newBallX = Ball.ClampToBoardFlatPart(ball.Position.x, board);
        ball.SetAttached(Vector2.Zero, new Vector2(newBallX, ball.Position.y));
    }

    //the angle of incidence is equal to the angle of reflection
    private Vector2 NaturalBounce(KinematicCollision2D col)
    {
        return Dir.Bounce(col.GetNormal());
    }

    //bouncing from the board depends on collision place
    private Vector2 BoardBounce(KinematicCollision2D col, Board board)
    {
        return Bounce.BoardBounce(ball, board.Position, board.Extents, col.Position);
    }
}

public class Attached : IState
{
    private Ball ball;
    private Board board;
    private float velocityOffset { get => Mathf.Sign(board.Velocity.x) * 0.05f; }
    public Vector2 Dir { get; set; } = Vector2.Zero;
    public Vector2 AttachPosition { get; set; } = Vector2.Zero;

    public Attached(Ball ball, Board board)
    {
        this.ball = ball;
        this.board = board;
    }

    public void Exit() { }
    public void Process(float dt) { }

    public void Init(params object[] args)
    {
        Dir = (Vector2) args[0];
        AttachPosition = (Vector2) args[1] - board.Position;
    }

    public void HandleInput()
    {
        if (Input.IsActionPressed("ui_accept"))
        {
            ball.SetMoving(GetDispatchDir());
        }
    }

    public void PhysicsProcess(float dt)
    {
        var board_pos = board.Position;
        var board_width = board.Extents.x;
        var ball_height = ball.GetExtents.y;

        var new_y = board_pos.y - ball_height;
        var new_x = ball.Position.LinearInterpolate(
            new Vector2(board_pos.x + AttachPosition.x - velocityOffset, new_y),
            dt * ball.SlideSpeed).x;

        ball.SetPosition(new Vector2(new_x, new_y));
    }

    private Vector2 GetDispatchDir()
    {
        if (ball.GlueToBoard)
        {
            return Bounce.BoardBounce(ball, board.Position, board.Extents, ball.Position);
        }
        else
        {
            return Dir;
        }
    }
}
#endregion

public class Ball : KinematicBody2D
{
    [Export]
    public float InitialSpeed { get; set; } = 300.0f;
    [Export]
    public float MaxSpeed { get; set; } = 1000.0f;
    [Export]
    public float SlideSpeed { get; set; } = 25.0f;
    [Export]
    public float FirstAngleSpeedUp { get; set; } = 0.0f;
    [Export]
    public float SecondAngleSpeedUp { get; set; } = 0.0f;
    [Export]
    public float ThirdAngleSpeedUp { get; set; } = 0.0f;
    [Export]
    public float BlockHitSpeedUp { get; set; } = 0.0f;
    public float CurrentSpeed { get; private set; } = 0.0f;
    public Vector2 StartingDir { private get; set; } = Vector2.Zero;
    public Vector2 CurrentDir { get => stateMachine.GetState<Moving>().Dir; set { stateMachine.GetState<Moving>().Dir = value; } }
    public Vector2 GetExtents { get => shape.GetExtents(); }
    public bool MovingAtStart { get; set; } = false;
    public bool GlueToBoard { get; set; } = false;
    public IState CurrentState { get => stateMachine.GetState(); }
    public AudioManager Audio { get; set; }
    public Board Board { get; set; }
    private StateMachine stateMachine = new StateMachine();
    private RectangleShape2D shape;
    private Timer attachTimer;

    public static float ClampToBoardFlatPart(float x, Board board)
    {
        return Mathf.Clamp(x, board.Middle.x - 51.0f, board.Middle.x + 49.0f);
    }
    public void SpeedUp(float speedUp)
    {
        GD.Print("Speeding up! ", speedUp);
        SetSpeed(CurrentSpeed + speedUp);
    }

    public void SetSpeed(float speed)
    {
        GD.Print("Current speed: ", speed);
        CurrentSpeed = speed;
    }

    public void SetAttached(Vector2 dir, Vector2 attachPos)
    {
        attachTimer.Start();
        stateMachine.ChangeState(nameof(Attached), dir, attachPos);
    }

    public void SetMoving(Vector2 dir)
    {
        Audio.BoardHit();
        attachTimer.Stop();
        stateMachine.ChangeState(nameof(Moving), dir);
    }

    public void SetMoving()
    {
        SetMoving(Utils.AngleToDir(Bounce.FirstAngle));
    }

    public void ResetState()
    {
        ResetPowerups();
        ResetSpeed();
        Position = Board.Middle;
        SetAttached(Utils.AngleToDir(Bounce.FirstAngle), Position);
    }

    public void ResetSpeed()
    {
        GD.Print("Resetting speed");
        SetSpeed(InitialSpeed);
    }

    public void ResetPowerups()
    {
        if (stateMachine.GetState() is Attached)
        {
            SetMoving(Utils.AngleToDir(Bounce.FirstAngle));
        }
        GlueToBoard = false;
    }

    public override void _Ready()
    {
        attachTimer = GetNode<Timer>("AttachTimer");
        stateMachine.Add(nameof(Moving), new Moving(this));
        stateMachine.Add(nameof(Attached), new Attached(this, Board));

        Audio = GetNode<AudioManager>("../AudioManager");

        if (MovingAtStart)
        {
            SetMoving(StartingDir);
        }
        else
        {
            SetAttached(Utils.AngleToDir(Bounce.FirstAngle), Board.Middle);
        }

        SetSpeed(InitialSpeed);
        shape = (RectangleShape2D)this.GetNode<CollisionShape2D>("col").GetShape();

        PauseMode = PauseModeEnum.Stop;
    }

    public override void _Process(float dt)
    {
        stateMachine.HandleInput();
        stateMachine.Process(dt);
    }

    public override void _PhysicsProcess(float dt)
    {
        stateMachine.PhysicsProcess(dt);
    }
}
