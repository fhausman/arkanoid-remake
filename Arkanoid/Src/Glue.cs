using Godot;
using System;

public class Glue : BasePowerUp
{
    public override void OnCollect()
    {
        PowerupManager.ResetPowerups();

        var ball = (Ball) GetTree().GetNodesInGroup("BALLS")[0];
        ball.GlueToBoard = true;
        base.OnCollect();
    }
}
