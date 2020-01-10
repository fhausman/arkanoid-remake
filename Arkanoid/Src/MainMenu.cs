using Godot;
using System;

public class MainMenu : Control
{
    PackedScene mainScene;
    PackedScene credits;

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
        AddChild(credits.Instance());
    }

    public override void _Ready()
    {
        mainScene = GD.Load<PackedScene>("MainScene.tscn");
        credits = GD.Load<PackedScene>("Credits.tscn");
        GetNode<Button>("VBoxContainer/Start").GrabFocus();
    }
}
