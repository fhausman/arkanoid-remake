using Godot;
using System;

public class PathFollow : PathFollow2D
{
    [Export]
    public int Speed = 500;

    public override void _Process(float delta)
    {
        Offset += Speed*delta;
    }
}
