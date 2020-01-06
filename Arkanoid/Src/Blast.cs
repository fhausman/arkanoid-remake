using Godot;

public class Blast : KinematicBody2D
{
    [Export]
    public int Speed { get; set; } = 200;

    public override void _PhysicsProcess(float delta)
    {
        var col = MoveAndCollide(Vector2.Up*Speed*delta);
        if(col != null)
        {
            if(col.Collider is IHittable obj)
                obj.OnHit();
            
            GetParent().QueueFree();
        }
    }

    public override void _Ready()
    {
        PauseMode = PauseModeEnum.Stop;
    }
}
