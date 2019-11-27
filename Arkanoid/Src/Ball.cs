using Godot;
using System;

#region Ball States
public class Moving : IState
{
    private Ball ball;
    private StateMachine stateMachine;
    public Vector2 Dir { get; set; } = new Vector2(-1, -1);

    public Moving(Ball ball, StateMachine stateMachine)
    {
        this.ball = ball;
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
            Dir = Dir.Bounce(col.GetNormal());
            if(col.Collider is IHittable obj)
            {
                obj.OnHit();
            }
        }
    }
}

public class Attached : IState
{
    private Ball ball;
    private RectangleShape2D ball_shape;
    private Board board;
    private RectangleShape2D board_shape;
    private StateMachine stateMachine;
    public Vector2 Dir { get; set; } = new Vector2(-1, -1);

    public Attached(Ball ball, Board board, StateMachine stateMachine)
    {
        this.ball = ball;
        this.board = board;
        this.stateMachine = stateMachine;
    }

    public void Exit() {}
    public void PhysicsProcess(float dt) {}

    public void Init()
    {
        if (ball.GetNode<CollisionShape2D>("col").GetShape() is RectangleShape2D ball_shape)
        {
            this.ball_shape = ball_shape;
        }

        if (board.GetNode<CollisionShape2D>("col").GetShape() is RectangleShape2D board_shape)
        {
            this.board_shape = board_shape;
        }
    }

    public void HandleInput()
    {
        if(Input.IsActionPressed("ui_accept"))
        {
            var movingState = stateMachine.ChangeState("Moving") as Moving;
            movingState.Dir = new Vector2(board.Velocity.x > 0 ? 1 : -1,-1);
        }
    }

    public void Process(float dt)
    {
        var board_pos = board.GetPosition();
        var board_width = board_shape.GetExtents().x;
        var ball_height = ball_shape.GetExtents().y;
        ball.SetPosition(
            new Vector2(board_pos.x + board_width, board_pos.y - ball_height*2));
    }
}
#endregion

public class Ball : KinematicBody2D
{
    [Export]
    public float InitialSpeed { get; set; } = 300.0f;
    [Export]
    public float MaxSpeed { get; set; } = 1000.0f;
    public float CurrentSpeed { get; set; } = 0.0f;

    private StateMachine stateMachine = new StateMachine();
    private Board board = null;


    public override void _Ready()
    {
        board = GetNode("/root/Main/Board") as Board;
        if(board == null)
            throw new Exception("Something went wrong, board shouldn't be null");

        stateMachine.Add("Moving", new Moving(this, stateMachine));
        stateMachine.Add("Attached", new Attached(this, board, stateMachine));
        stateMachine.ChangeState("Attached");

        CurrentSpeed = InitialSpeed;
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
