using Godot;
using System;

public class MainMenu : Control
{
    PackedScene mainScene;
    PackedScene credits;
    AudioStreamPlayer2D menuSound;
    AudioStreamPlayer2D startSound;
    bool startGame = false;

    public void OnQuitPressed()
    {
        GetTree().Quit();
    }

    public void OnStartPressed()
    {
        GetNode<AudioStreamPlayer2D>("StartSound").Play();
        startGame = true;
        GetTree().Paused = true;
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
        startSound = GetNode<AudioStreamPlayer2D>("StartSound");
        GetNode<Button>("VBoxContainer/Start").GrabFocus();
    }

    public override void _Process(float delta)
    {
        if(startGame && !startSound.IsPlaying())
        {
            GetTree().ChangeSceneTo(mainScene);
        }
    }
}
