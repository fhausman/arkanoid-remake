using Godot;
using System;

public class ExtraLife : BasePowerUp
{
    MainScene scene;

    public override void OnCollect()
    {
        scene.AddExtraLife();
        base.OnCollect();
    }

    public override void _Ready()
    {
        scene = GetNode<MainScene>("/root/Main");
        base._Ready();
    }
}
