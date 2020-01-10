using Godot;
using System;

public class MainMenu : Control
{
    PackedScene mainScene;
    PackedScene credits;
    AudioStreamPlayer2D menuSound;

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

    public void OnFocusEnter()
    {
        menuSound.Play();
    }

    public override void _Ready()
    {
        mainScene = GD.Load<PackedScene>("MainScene.tscn");
        credits = GD.Load<PackedScene>("Credits.tscn");
        menuSound = GetNode<AudioStreamPlayer2D>("MenuSound");
        GetNode<Button>("VBoxContainer/Start").GrabFocus();
    }
}
