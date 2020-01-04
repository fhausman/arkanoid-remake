using Godot;
using System.Collections.Generic;
using System.Linq;

public class EnemyMoveBase : IState
{
    protected StateMachine stateMachine { get; set; }
    protected Enemy enemy { get; set; }
    protected float speed { get; set; }
    protected RandomNumberGenerator randGen { get; set; } = new RandomNumberGenerator();

    public EnemyMoveBase(Enemy enemy, StateMachine stateMachine, float speed)
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
    }

    public virtual void Process(float dt)
    {
    }

    protected KinematicCollision2D Move(Vector2 dir, float dt)
    {
        return enemy.MoveAndCollide(dir*speed*dt);
    }

    protected void RandomChangeState(string currState)
    {
        randGen.Randomize();
        var remainingStates = stateMachine.States.Where(s => s.Key != currState);
        var idx = randGen.RandiRange(0, remainingStates.Count()-1);
        var state = remainingStates.ElementAt(idx).Key;

        GD.Print(state);
        stateMachine.ChangeState(state);
    }
}

public class EnemyMoveLeft : EnemyMoveBase
{
    public EnemyMoveLeft(Enemy enemy, StateMachine stateMachine, float speed)
        : base(enemy, stateMachine, speed)
    {
    }

    public override void PhysicsProcess(float dt)
    {
        var col = Move(Vector2.Left, dt);
        if(col != null)
        {
            RandomChangeState(nameof(EnemyMoveLeft));
        }
    }
}

public class EnemyMoveRight : EnemyMoveBase
{
    public EnemyMoveRight(Enemy enemy, StateMachine stateMachine, float speed)
        : base(enemy, stateMachine, speed)
    {
    }

    public override void PhysicsProcess(float dt)
    {
        var col = Move(Vector2.Right, dt);
        if(col != null)
        {
            RandomChangeState(nameof(EnemyMoveRight));
        }
    }
}

public class EnemyMoveDown : EnemyMoveBase
{
    public EnemyMoveDown(Enemy enemy, StateMachine stateMachine, float speed)
        : base(enemy, stateMachine, speed)
    {
    }

    public override void PhysicsProcess(float dt)
    {
        var col = Move(Vector2.Down, dt);
        if(col != null)
        {
            RandomChangeState(nameof(EnemyMoveDown));
        }
    }
}

public class Enemy : KinematicBody2D, IHittable
{
    [Export]
    public float MoveSpeed { get; set; } = 0.0f;
    private Timer changeStateTimer { get; set; }
    private StateMachine stateMachine { get; set; } = new StateMachine();

    public void OnHit()
    {
        //play destroy animation
        Free();
    }

    public override void _Ready()
    {
        stateMachine.Add(nameof(EnemyMoveLeft), new EnemyMoveLeft(this, stateMachine, MoveSpeed));
        stateMachine.Add(nameof(EnemyMoveRight), new EnemyMoveRight(this, stateMachine, MoveSpeed));
        stateMachine.Add(nameof(EnemyMoveDown), new EnemyMoveDown(this, stateMachine, MoveSpeed));

        stateMachine.ChangeState(nameof(EnemyMoveDown));

        changeStateTimer = GetNode<Timer>("ChangeStateTimer");
    }

    public override void _PhysicsProcess(float delta)
    {
        stateMachine.PhysicsProcess(delta);
    }
}
