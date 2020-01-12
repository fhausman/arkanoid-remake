using Godot;
using System.Collections.Generic;

public class EnemyMoveUp : IState
{
    private StateMachine stateMachine;
    private Enemy enemy;
    private float speed;
    private Vector2 dir = Vector2.Up;
    private Vector2 previousHorizontalDir = Vector2.Right;

    public EnemyMoveUp(Enemy enemy, StateMachine stateMachine, float speed)
    {
        this.enemy = enemy;
        this.stateMachine = stateMachine;
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
        if(args.Length > 0)
        {
            previousHorizontalDir = (Vector2) args[0];
        }
    }

    public void PhysicsProcess(float dt)
    {
        if(enemy.SideChecker.AreBothSidesBlocked())
        {
            var col = enemy.MoveAndCollide(dir*speed*dt);
            
        }
        else
        {
            Vector2 dir = previousHorizontalDir;
            if(enemy.SideChecker.IsLeftSideBlocked())
            {
                dir = Vector2.Right;
            }
            else if(enemy.SideChecker.IsRightSideBlocked())
            {
                dir = Vector2.Left;
            }

            stateMachine.ChangeState(nameof(EnemyMoveSteady), dir, true);
        }
    }

    public void Process(float dt)
    {
    }
}
public class EnemyMoveSteady : IState
{
    private StateMachine stateMachine;
    private Enemy enemy;
    private float speed;
    private Vector2 dir = Vector2.Down;
    private Vector2 previousHorizontalDir = Vector2.Right;
    private bool forceHorizontal = false;

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
        if(args.Length > 0)
        {
            dir = (Vector2) args[0];
            forceHorizontal = (bool) args[1];
        }
    }

    private bool CanMoveDownwards()
    {
        return !forceHorizontal &&
                dir != Vector2.Down &&
                enemy.BelowArea.GetOverlappingBodies().Count == 0;
    }

    public virtual void PhysicsProcess(float dt)
    {
        var col = enemy.MoveAndCollide(dir*speed*dt);
        if(col != null)
        {
            forceHorizontal = false;
            if(col.Collider is Board)
            {
               enemy.OnHit();
               return;
            }
            else if(col.Collider is Enemy && dir == Vector2.Down)
            {
                enemy.MoveAndCollide(Vector2.Up*speed*dt);
                return;
            }
            else if(dir == Vector2.Down && enemy.SideChecker.AreBothSidesBlocked())
            {
                stateMachine.ChangeState(nameof(EnemyMoveUp), previousHorizontalDir);
            }

            //if was moving down move horizontally in previous direction
            //if not move in opposite direction
            dir = (dir != Vector2.Down) ?
                -dir : previousHorizontalDir;
        }
        //if nothing below move down
        else if(CanMoveDownwards())
        {
            previousHorizontalDir = dir;
            dir = Vector2.Down;
        }
    }

    public virtual void Process(float dt)
    {
        var crazyPosition = MainScene.CurrentLevel != Lvl.LEVEL2 ? 480 : 640;
        if(enemy.GlobalPosition.y > crazyPosition)
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
            if(col.Collider is Board)
            {
                enemy.OnHit();
            }
            else if(col.Collider is Ball ball)
            {
                if(ball.CurrentState is Attached)
                {
                    enemy.OnHit();
                }
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

public class SideChecker
{
    Node2D mainNode;
    Area2D leftSide;
    Area2D rightSide;

    public SideChecker(Node2D node)
    {
        mainNode = node;
        leftSide = node.GetNode<Area2D>("Left");
        rightSide = node.GetNode<Area2D>("Right");
    }

    public bool IsLeftSideBlocked()
    {
        return leftSide.GetOverlappingBodies().Count > 0;
    }

    public bool IsRightSideBlocked()
    {
        return rightSide.GetOverlappingBodies().Count > 0;
    }

    public bool AreBothSidesBlocked()
    {
        return IsLeftSideBlocked() && IsRightSideBlocked();
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
    public SideChecker SideChecker { get; set; }
    public AudioManager audio;
    private MainScene scene;
    private StateMachine stateMachine = new StateMachine();
    private AnimationPlayer animation;
    private Sprite triangle;
    private Sprite square;
    private Sprite folded;
    private Sprite origami;
    private Dictionary<string, Sprite> animations;
    private static int NumSpawned = 0;

    public void OnHit()
    {
        foreach(var anim in animations)
        {
            anim.Value.Visible = false;
        }

        scene.Score += 50*((int) scene.Level + 1);
        stateMachine.ChangeState(nameof(EmptyState));
        SetCollisionLayer(0);
        SetCollisionMask(0);
        animation.Play("death");
        audio.EnemyDeath();
    }

    public void OnAnimationFinished(string name)
    {
        if(name == "death")
        {
            QueueFree();
        }
    }

    private Node2D GetFollowingPoint()
    {
        var str = GD.Str("../../EnemiesPath", NumSpawned%2, "/PathFollow/FollowingPoint");
        return GetNode<Node2D>(str);
    }

    public override void _Ready()
    {
        NumSpawned++;
        stateMachine.Add(nameof(EmptyState), new EmptyState());
        stateMachine.Add(nameof(EnemyMoveSteady), new EnemyMoveSteady(this, stateMachine, MoveSpeed));
        stateMachine.Add(nameof(EnemyMoveUp), new EnemyMoveUp(this, stateMachine, MoveSpeed));
        stateMachine.Add(nameof(CrazyMode), new CrazyMode(this, 4*MoveSpeed));
        stateMachine.ChangeState(nameof(EnemyMoveSteady));

        BelowArea = GetNode<Area2D>("Area2D");
        FollowNode = GetFollowingPoint();
        SideChecker = new SideChecker(GetNode<Node2D>("SideChecker"));

        scene = GetNode<MainScene>("../..");
        audio = scene.GetNode<AudioManager>("AudioManager");

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
