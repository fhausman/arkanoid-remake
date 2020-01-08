using Godot;

public class EnemyMoveSteady : IState
{
    private StateMachine stateMachine;
    private Enemy enemy;
    private float speed;
    private RandomNumberGenerator randGen = new RandomNumberGenerator();
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
    }

    public void PhysicsProcess(float dt)
    {
        velocity = ArriveTo(
            velocity,
            enemy.GlobalPosition,
            enemy.GetViewport().GetMousePosition(),
            speed,
            200.0f,
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
        Vector2 global_position,
        Vector2 target_position,
        float max_speed,
        float slow_radius,
        float mass
    )
    {
        var toTarget = global_position.DistanceTo(target_position);
        var desired_velocity = (target_position - global_position).Normalized()*max_speed;
        if(toTarget < slow_radius)
            desired_velocity *= (toTarget / slow_radius) * 0.75f + 0.25f;
        
        var steering = (desired_velocity - velocity) / mass;
        return velocity + steering;
    }
}

public class Enemy : KinematicBody2D, IHittable
{
    [Export]
    public float MoveSpeed { get; set; } = 0.0f;
    public Area2D BelowArea { get; set; }
    private StateMachine stateMachine = new StateMachine();

    public void OnHit()
    {
        //play destroy animation
        Free();
    }

    public override void _Ready()
    {
        stateMachine.Add(nameof(EnemyMoveSteady), new EnemyMoveSteady(this, stateMachine, MoveSpeed));
        stateMachine.Add(nameof(CrazyMode), new CrazyMode(this, 4*MoveSpeed));
        stateMachine.ChangeState(nameof(CrazyMode));

        BelowArea = GetNode<Area2D>("Area2D");

        PauseMode = PauseModeEnum.Stop;
    }

    public override void _PhysicsProcess(float delta)
    {
        stateMachine.PhysicsProcess(delta);
    }
}
