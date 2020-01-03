using Godot;
using System;

public class Glue : BasePowerUp
{
    public override void OnCollect()
    {
        var ball = (Ball) GetTree().GetNodesInGroup("BALLS")[0];
        ball.GlueToBoard = true;
        base.OnCollect();
    }
}
