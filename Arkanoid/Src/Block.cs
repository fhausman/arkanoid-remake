using Godot;
using System;

public class Block : StaticBody2D, IHittable
{
    [Export]
    public bool Destructable { get; set; } = true;

    [Export]
    public int NumOfHits { get; set; } = 1;

    public void OnHit()
    {
        NumOfHits--;
        if(NumOfHits <= 0)
        {
            Free();
        }
    }
}
