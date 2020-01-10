using Godot;
using System;

public class SilverBlock : Block
{
    AnimationPlayer animationPlayer;

    public override void _Ready()
    {
        animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        base._Ready();
    }

    public override void OnHit()
    {
        animationPlayer.Play("damaged");
        base.OnHit();
    }
}