using Godot;
using System;

public class GoldenBlock : Block
{
    AnimationPlayer animation;
    public override void _Ready()
    {
        animation = GetNode<AnimationPlayer>("AnimationPlayer");
        base._Ready();
    }

    public override void OnHit()
    {
        animation.Play("hitted");
        base.OnHit();
    }
}
