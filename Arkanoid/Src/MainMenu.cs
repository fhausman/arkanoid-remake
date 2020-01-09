using Godot;
using System;

public class MainMenu : Control
{
    PackedScene mainScene;

    public void OnQuitPressed()
    {
        GetTree().Quit();
    }

    public void OnStartPressed()
    {
        GetTree().ChangeSceneTo(mainScene);
    }

    public void OnCreditsPressed()
    {
        GetTree().ChangeScene("Credits.tscn");
    }

    public override void _Ready()
    {
        mainScene = GD.Load<PackedScene>("MainScene.tscn");
        GetNode<Button>("VBoxContainer/Start").GrabFocus();
    }
}
