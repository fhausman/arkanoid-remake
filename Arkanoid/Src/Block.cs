using Godot;
using System;

public class Block : StaticBody2D, IHittable
{
    [Export]
    public bool Destructable { get; set; } = true;

    [Export]
    public int NumOfHits { get; set; } = 1;

    [Export]
    public int Points { get; set; } = 100;

    public override void _Ready()
    {
        //todo: get high score reference
    }

    public void OnHit()
    {
        //todo: play hit animation

        if(!Destructable)
            return;

        NumOfHits--;
        if(NumOfHits <= 0)
        {
            //todo: increase points
            Free();
        }
    }
}
