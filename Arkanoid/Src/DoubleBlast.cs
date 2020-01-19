using Godot;

public class DoubleBlast : Node2D
{
    private Node2D leftBlast;
    private Node2D rightBlast;
    private AnimationPlayer animation;
    private bool hitted = false;
    
    public void OnHit()
    {
        hitted = true;
        animation.Play("hit");
    }

    public void OnAnimationFinished(string name)
    {
        GD.Print("HIT");
        if(name == "hit")
            QueueFree();
    }

    public override void _Ready()
    {
        animation = GetNode<AnimationPlayer>("AnimationPlayer");

        leftBlast = GetNode<Node2D>("LeftBlast");
        rightBlast = GetNode<Node2D>("RightBlast");
    }

    public override void _Process(float delta)
    {
        if(hitted)
        {
            leftBlast.SetPhysicsProcess(false);
            rightBlast.SetPhysicsProcess(false);
        }
    }
}
