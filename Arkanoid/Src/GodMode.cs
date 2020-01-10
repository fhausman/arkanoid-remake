using Godot;
using System;

public class GodMode : Node2D
{
    Timer keyDelay;
    bool enableInput = true;

    public bool GodModeEnabled { get; private set; } = false;

    public void EnableInput()
    {
        enableInput = true;
    }

    public void DelayInput()
    {
        enableInput = false;
        keyDelay.Start();
    }

    public override void _Ready()
    {
        keyDelay = GetNode<Timer>("KeyDelay");
    }

    public override void _Process(float delta)
    {
        if(!enableInput)
            return;
        
        if(Input.IsKeyPressed((int) KeyList.G))
        {
            GodModeEnabled = !GodModeEnabled;
            DelayInput();

            GD.Print("God mode: ", GodModeEnabled);
        }
    }
}
