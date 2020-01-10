using Godot;
using System.Collections.Generic;

public class EnemyMoveUp : IState
{
    public void Exit()
    {
        throw new System.NotImplementedException();
    }

    public void HandleInput()
    {
        throw new System.NotImplementedException();
    }

    public void Init(params object[] args)
    {
        throw new System.NotImplementedException();
    }

    public void PhysicsProcess(float dt)
    {
        throw new System.NotImplementedException();
    }

    public void Process(float dt)
    {
        throw new System.NotImplementedException();
    }
}
public class EnemyMoveSteady : IState
{
    private StateMachine stateMachine;
    private Enemy enemy;
    private float speed;
    private Vector2 dir = Vector2.Down;
    private Vector2 previousHorizontalDir = Vector2.Right;

    public EnemyMoveSteady(Enemy enemy, StateMachine stateMachine, float speed)
    {
        this.enemy = enemy;
        this.stateMachine = stateMachine;
        this.speed = speed;
    }

    public virtual void Exit()
    {
    }

    public virtual void HandleInput()
    {
    }

    public virtual void Init(params object[] args)
    {
    }

    public virtual void PhysicsProcess(float dt)
    {
        var col = enemy.MoveAndCollide(dir*speed*dt);
        if(col != null)
        {
            if(col.Collider is Board || col.ColliderShape is Ball)
            {
               enemy.OnHit();
               return;
            }
            else if(col.Collider is Enemy && dir == Vector2.Zero)
            {
                return;
            }

            //if was moving down move horizontally in previous direction
            //if not move in opposite direction
            dir = (dir != Vector2.Down) ?
                -dir : previousHorizontalDir;
        }
        //if nothing below move down
        else if(dir != Vector2.Down && enemy.BelowArea.GetOverlappingBodies().Count == 0)
        {
            previousHorizontalDir = dir;
            dir = Vector2.Down;
        }
    }

    public virtual void Process(float dt)
    {
        if(enemy.GlobalPosition.y > 640)
        {
            stateMachine.ChangeState(nameof(CrazyMode));
        }
    }
}

public class CrazyMode : IState
{
    private Enemy enemy;
    private float speed;
    private Vector2 velocity = Vector2.Zero;
    public CrazyMode(Enemy enemy, float speed)
    {
        this.enemy = enemy;
        this.speed = speed;
    }

    public void Exit()
    {
    }

    public void HandleInput()
    {
    }

    public void Init(params object[] args)
    {
        enemy.SetCollisionMask(9);
    }

    public void PhysicsProcess(float dt)
    {
        velocity = ArriveTo(
            velocity,
            enemy.GlobalPosition,
            enemy.FollowNode.GlobalPosition,
            speed,
            100.0f,
            2.0f
            );
        

        var col = enemy.MoveAndCollide(velocity*dt);
        if(col != null)
        {
            if(col.Collider is Board || col.Collider is Ball)
            {
                enemy.OnHit();
                return;
            }
        }
    }

    public void Process(float dt)
    {
    }

    private Vector2 ArriveTo(
        Vector2 velocity,
        Vector2 globalPosition,
        Vector2 targetPosition,
        float maxSpeed,
        float slowRadius,
        float mass
    )
    {
        var toTarget = globalPosition.DistanceTo(targetPosition);
        var desired_velocity = (targetPosition - globalPosition).Normalized()*maxSpeed;
        if(toTarget < slowRadius)
            desired_velocity *= (toTarget / slowRadius) * 0.75f + 0.25f;
        
        var steering = (desired_velocity - velocity) / mass;
        return velocity + steering;
    }
}

public class Enemy : KinematicBody2D, IHittable
{
    [Export]
    public float MoveSpeed { get; set; } = 0.0f;
    [Export]
    public string EnemyType { get; set; } = "folded";
    public Area2D BelowArea { get; set; }
    public Node2D FollowNode { get; set; }
    private StateMachine stateMachine = new StateMachine();
    private AnimationPlayer animation;
    private Sprite triangle;
    private Sprite square;
    private Sprite folded;
    private Sprite origami;
    private Dictionary<string, Sprite> animations;

    public void OnHit()
    {
        foreach(var anim in animations)
        {
            anim.Value.Visible = false;
        }

        stateMachine.ChangeState(nameof(EmptyState));
        SetCollisionLayer(0);
        SetCollisionMask(0);
        animation.Play("death");
    }

    public void OnAnimationFinished(string name)
    {
        if(name == "death")
        {
            QueueFree();
        }
    }

    public override void _Ready()
    {
        stateMachine.Add(nameof(EmptyState), new EmptyState());
        stateMachine.Add(nameof(EnemyMoveSteady), new EnemyMoveSteady(this, stateMachine, MoveSpeed));
        stateMachine.Add(nameof(CrazyMode), new CrazyMode(this, 4*MoveSpeed));
        stateMachine.ChangeState(nameof(EnemyMoveSteady));

        BelowArea = GetNode<Area2D>("Area2D");
        FollowNode = GetNode<Node2D>("../../EnemiesPath/PathFollow/FollowingPoint");

        animation = GetNode<AnimationPlayer>("AnimationPlayer");
        triangle = GetNode<Sprite>("Triangle");
        square = GetNode<Sprite>("Square");
        folded = GetNode<Sprite>("Folded");
        origami = GetNode<Sprite>("Origami");
        animations = new Dictionary<string, Sprite>()
        {
            {nameof(triangle), triangle},
            {nameof(square), square},
            {nameof(folded), folded},
            {nameof(origami), origami},
        };
        animations[EnemyType].Visible = true;
        animation.Play(EnemyType);

        PauseMode = PauseModeEnum.Stop;
    }

    public override void _Process(float delta)
    {
        stateMachine.Process(delta);
    }

    public override void _PhysicsProcess(float delta)
    {
        stateMachine.PhysicsProcess(delta);
    }
}
