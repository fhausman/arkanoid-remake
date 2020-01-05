using Godot;
using System;

public class Credits : Control
{
    public override void _Input(InputEvent e)
    {
        if(e is InputEvent && e.IsPressed())
        {
            GetTree().ChangeScene("MainMenu.tscn");
        }
    }
}
