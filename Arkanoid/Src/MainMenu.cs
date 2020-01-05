using Godot;
using System;

public class MainMenu : Control
{
    public void OnQuitPressed()
    {
        GetTree().Quit();
    }

    public void OnStartPressed()
    {
        GetTree().ChangeScene("MainScene.tscn");
    }

    public void OnCreditsPressed()
    {
        GetTree().ChangeScene("Credits.tscn");
    }

    public override void _Ready()
    {
        GetNode<Button>("CenterContainer/VBoxContainer/Start").GrabFocus();
    }
}
