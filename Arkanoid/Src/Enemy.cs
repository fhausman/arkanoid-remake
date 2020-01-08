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
            if(col.Collider is Board)
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
    }
}

public class Enemy : KinematicBody2D, IHittable
{
    [Export]
    public float MoveSpeed { get; set; } = 0.0f;
    public Area2D BelowArea { get; set; }
    private Timer changeStateTimer;
    private StateMachine stateMachine = new StateMachine();

    public void OnHit()
    {
        //play destroy animation
        Free();
    }

    public override void _Ready()
    {
        stateMachine.Add(nameof(EnemyMoveSteady), new EnemyMoveSteady(this, stateMachine, MoveSpeed));
        stateMachine.ChangeState(nameof(EnemyMoveSteady));

        BelowArea = GetNode<Area2D>("Area2D");

        changeStateTimer = GetNode<Timer>("ChangeStateTimer");
        changeStateTimer.Start();

        PauseMode = PauseModeEnum.Stop;
    }

    public override void _PhysicsProcess(float delta)
    {
        stateMachine.PhysicsProcess(delta);
    }
}
