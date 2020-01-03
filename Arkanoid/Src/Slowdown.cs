using Godot;
using System;

public class Slowdown : BasePowerUp
{
    public override void OnCollect()
    {
        PowerupManager.ResetPowerups();

        GetTree().CallGroup("BALLS", "ResetSpeed");
        base.OnCollect();
    }
}
