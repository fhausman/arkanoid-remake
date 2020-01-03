using Godot;
using System;

public class Slowdown : BasePowerUp
{
    public override void OnCollect()
    {
        GetTree().CallGroup("BALLS", "ResetSpeed");
        base.OnCollect();
    }
}
