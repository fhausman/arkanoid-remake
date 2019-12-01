using Godot;
using System;

#region Ball States
public class Moving : IState
{
    private Ball ball;
    private Board board;
    private StateMachine stateMachine;
    public Vector2 Dir { get; set; } = new Vector2(-1, -1);

    public Moving(Ball ball, Board board, StateMachine stateMachine)
    {
        this.ball = ball;
        this.board = board;
        this.stateMachine = stateMachine;
    }

    public void Init() {}
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
                Dir = Bounce.BoardBounce(board.Position, board.GetExtents, col.Position);
            }
            else
            {
                Dir = Dir.Bounce(col.GetNormal());
            }

            if(col.Collider is IHittable obj)
            {
                obj.OnHit();
                ball.CheckWinConditions();
            }
        }
    }
}

public class Attached : IState
{
    private Ball ball;
    private Board board;
    private StateMachine stateMachine;
    private float GetVelocityOffset { get { return (board.Velocity.x / board.Speed) * board.GetExtents.x * 0.15f; }}
    public Vector2 Dir { get; set; } = new Vector2(-1, -1);

    public Attached(Ball ball, Board board, StateMachine stateMachine)
    {
        this.ball = ball;
        this.board = board;
        this.stateMachine = stateMachine;
    }

    public void Exit() {}
    public void Process(float dt) {}

    public void Init() {}

    public void HandleInput()
    {
        if(Input.IsActionPressed("ui_accept"))
        {
            var movingState = stateMachine.ChangeState("Moving") as Moving;
            movingState.Dir = Bounce.AngleToDir(Bounce.FirstAngle);
        }
    }

    public void PhysicsProcess(float dt)
    {
        var board_pos = board.GetPosition();
        var board_width = board.GetExtents.x;
        var ball_height = ball.GetExtents.y;

        var new_y = board_pos.y - ball_height*2 + 8.0f;
        var new_x = ball.Position.LinearInterpolate(
            new Vector2(board_pos.x + board_width - GetVelocityOffset, new_y),
            dt*ball.SlideSpeed).x;
 
        ball.SetPosition(new Vector2(new_x, new_y));
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
    public float CurrentSpeed { get; set; } = 0.0f;
    public Vector2 GetExtents { get => shape.GetExtents(); }
    [Signal]
    public delegate void CheckWin();

    private StateMachine stateMachine = new StateMachine();
    private Board board;
    private RectangleShape2D shape;

    public void CheckWinConditions()
    {
        EmitSignal(nameof(CheckWin));
    }

    public void ResetState()
    {
        CurrentSpeed = InitialSpeed;
        Position = new Vector2(board.Position.x + board.GetExtents.x, board.Position.y);
        stateMachine.ChangeState("Attached");
    }

    public override void _Ready()
    {
        board = (Board) GetNode("../Board");
        stateMachine.Add("Moving", new Moving(this, board, stateMachine));
        stateMachine.Add("Attached", new Attached(this, board, stateMachine));
        stateMachine.ChangeState("Attached");

        CurrentSpeed = InitialSpeed;
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
