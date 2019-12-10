using Godot;
using System;

public class ExtraLife : BasePowerUp
{
    MainScene scene;

    public override void OnCollect()
    {
        scene.AddExtraLife();
    }

    public override void _Ready()
    {
        scene = GetNode<MainScene>("/root/Main");
    }
}
