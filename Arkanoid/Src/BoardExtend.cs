using Godot;
using System;

public class BoardExtend : BasePowerUp
{
    public override void OnCollect()
    {
        PowerupManager.ResetPowerups();

        var board = GetNode<Board>("/root/Main/Board");
        board.Extend();
        base.OnCollect();
    }

    public override void _Ready()
    {
        base._Ready();
        animation.Play("extend");
    }
}
