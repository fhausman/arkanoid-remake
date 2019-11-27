using Godot;
using System;

public class MainScene : Node2D
{
    public void OnDeathAreaEntered(PhysicsBody2D body)
    {
        if (body.Name == "Ball")
        {
            GD.Print("Ball entered death zone");
        }
    }
}
