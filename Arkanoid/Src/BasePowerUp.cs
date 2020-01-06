using Godot;
using System;

public abstract class BasePowerUp : KinematicBody2D
{
    [Export]
    public int Speed { get; set; } = 200;
    protected AnimationPlayer animation;

    public virtual void OnCollect()
    {
        Free();
    }

    public override void _PhysicsProcess(float delta)
    {
        var col = MoveAndCollide(Vector2.Down*Speed*delta);
        if(col != null && col.Collider is Board)
        {
            GD.Print("Power up collected! ", this);
            OnCollect();
        }
    }

    public override void _Ready()
    {
        animation = GetNode<AnimationPlayer>("PowerUpAnimation/TypeAnimation");
        PauseMode = PauseModeEnum.Stop;

    }
}
