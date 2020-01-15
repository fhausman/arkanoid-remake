using Godot;
using System;

public class Credits : Control
{
    public override void _Input(InputEvent e)
    {
        if(e.IsActionPressed("ui_cancel"))
        {
            QueueFree();
            GetNode<Button>("../VBoxContainer/Credits").GrabFocus();
        }
    }
}
