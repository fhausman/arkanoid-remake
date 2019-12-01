using Godot;
using System;

public class MainScene : Node2D
{
    private Node2D blocks;
    void CheckIfPlayerDestroyedAllBlocks()
    {
        var blocks_count = blocks.GetChildCount();
        GD.Print("Checkin... ", blocks_count);
        if(blocks_count == 0)
        {
            GD.Print("Woohoo, level won, going to the next stage");
        }
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
        blocks = GetNode<Node2D>("Blocks");
        GetNode("Ball").Connect("CheckWin", this, nameof(CheckIfPlayerDestroyedAllBlocks));
    }
}
