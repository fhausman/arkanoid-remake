using Godot;
using System;

public class Teleport : BasePowerUp
{
    public override void OnCollect()
    {
        PowerupManager.ActivateTeleport();
        base.OnCollect();
    }
}
