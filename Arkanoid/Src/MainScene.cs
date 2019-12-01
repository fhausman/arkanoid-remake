using Godot;
using System;

public class MainScene : Node2D
{
    void CheckIfPlayerDestroyedAllBlocks()
    {
        GD.Print("Checkin...");
    }
    public void OnDeathAreaEntered(PhysicsBody2D body)
    {
        if (body is Ball ball)
        {
            ball.ResetState();
            GD.Print("Ball entered death zone");
        }
    }

    public override void _Ready()
    {
        GetNode("Ball").Connect("CheckWin", this, nameof(CheckIfPlayerDestroyedAllBlocks));
    }
}
