using Godot;
using System;

#region Ball States
public class Moving : IState
{
    private Ball ball;
    private Board board;
    public Vector2 Dir { get; set; } = new Vector2(-1, -1);

    public Moving(Ball ball, Board board)
    {
        this.ball = ball;
        this.board = board;
    }

    public void Init(params object[] args) { Dir = (Vector2) args[0]; }
    public void Exit() {}
    public void HandleInput() {}
    public void Process(float dt) {}

    public void PhysicsProcess(float dt)
    {
        var col = ball.MoveAndCollide(Dir*ball.CurrentSpeed*dt);
        if(col != null)
        {
            if(col.Collider is Board)
            {
                if(ball.GlueToBoard)
                {
                    var state = ball.SetAttached(Vector2.Zero, ball.Position);
                    //state
                    return;
                }

                Dir = Bounce.BoardBounce(ball, board.Position, board.Extents, col.Position);
            }
            else
            {
                Dir = Dir.Bounce(col.GetNormal());
            }

            if(col.Collider is IHittable obj)
            {
                obj.OnHit();
                ball.SpeedUp(ball.BlockHitSpeedUp);
                ball.CheckWinConditions();
            }
        }
    }
}

public class Attached : IState
{
    private Ball ball;
    private Board board;
    private float GetVelocityOffset { get { return (board.Velocity.x / board.Speed) * board.Extents.x * 0.005f; }}
    public Vector2 Dir { get; set; } = Vector2.Zero;
    public Vector2 AttachPosition { get; set; } = Vector2.Zero;

    public Attached(Ball ball, Board board)
    {
        this.ball = ball;
        this.board = board;
    }

    public void Exit() {}
    public void Process(float dt) {}

    public void Init(params object[] args)
    {
        Dir = (Vector2) args[0];
        AttachPosition = (Vector2) args[1] - board.Position;
    }

    public void HandleInput()
    {
        if(Input.IsActionPressed("ui_accept"))
        {
            ball.SetMoving(GetDispatchDir());
        }
    }

    public void PhysicsProcess(float dt)
    {
        var board_pos = board.Position;
        var board_width = board.Extents.x;
        var ball_height = ball.GetExtents.y;

        var new_y = board_pos.y - ball_height*2 + 8.0f; //todo: dispose magic numbers
        var new_x = ball.Position.LinearInterpolate(
            new Vector2(board_pos.x + AttachPosition.x - GetVelocityOffset, new_y),
            dt*ball.SlideSpeed).x;
 
        ball.SetPosition(new Vector2(new_x, new_y));
    }

    private Vector2 GetDispatchDir()
    {
        if(ball.GlueToBoard)
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
    public Vector2 CurrentDir { get => stateMachine.GetState<Moving>().Dir; }
    public Vector2 GetExtents { get => shape.GetExtents(); }
    public bool MovingAtStart { get; set; } = false;
    public bool GlueToBoard { get; set; } = false;

    [Signal]
    public delegate void CheckWin();

    private StateMachine stateMachine = new StateMachine();
    private Board board;
    private RectangleShape2D shape;

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

    public void CheckWinConditions()
    {
        EmitSignal(nameof(CheckWin));
    }

    public Attached SetAttached(Vector2 dir, Vector2 attachPos)
    {
        return (Attached) stateMachine.ChangeState(nameof(Attached), dir, attachPos);
    }

    public Moving SetMoving(Vector2 dir)
    {
        return (Moving) stateMachine.ChangeState(nameof(Moving), dir);
    }

    public void ResetState()
    {
        ResetPowerups();
        ResetSpeed();
        SetAttached(Bounce.AngleToDir(Bounce.FirstAngle), board.Middle);
    }

    public void ResetSpeed()
    {
        GD.Print("Resetting speed");
        SetSpeed(InitialSpeed);
    }

    public void ResetPowerups()
    {
        GlueToBoard = false;
    }

    public override void _Ready()
    {
        board = (Board) GetNode("../Board");
        stateMachine.Add(nameof(Moving), new Moving(this, board));
        stateMachine.Add(nameof(Attached), new Attached(this, board));
        if(MovingAtStart)
        {
            SetMoving(StartingDir);
        }
        else
        {
            SetAttached(Bounce.AngleToDir(Bounce.FirstAngle), board.Middle);
        }

        SetSpeed(InitialSpeed);
        shape = (RectangleShape2D) this.GetNode<CollisionShape2D>("col").GetShape();
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
