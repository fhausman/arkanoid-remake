using Godot;
using System;

public class BasePowerUp : KinematicBody2D
{
    [Export]
    public int Speed { get; set; } = 200;

    public override void _PhysicsProcess(float delta)
    {
        var col = MoveAndCollide(Vector2.Down*Speed*delta);
        if(col != null && col.Collider is Board)
        {
            GD.Print("Power up collected!");
            Free();
        }
    }
}
