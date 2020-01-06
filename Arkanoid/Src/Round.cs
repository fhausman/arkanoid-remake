using Godot;
using System;

public class Round : Control
{
    private Label round;
    private Label ready;
    private Timer readyDelay;
    private Timer mainTimer;
    public delegate void SequenceEnd();
    public SequenceEnd OnSequenceEnd;

    private void SequenceFinished()
    {
        OnSequenceEnd();
        Clear();
    }

    private void Clear()
    {
        round.Text = "";
        ready.Text = "";
    }

    public void Play(int levelNumber)
    {
        round.Text = GD.Str("ROUND ", levelNumber);
        readyDelay.Start();
        mainTimer.Start();
    }

    public void ShowReady()
    {
        ready.Text = "READY";
    }

    public override void _Ready()
    {
        round = GetNode<Label>("VBoxContainer/Text");
        ready = GetNode<Label>("VBoxContainer/Ready");
        readyDelay = GetNode<Timer>("ReadyDelay");
        mainTimer = GetNode<Timer>("MainTimer");

        readyDelay.Connect("timeout", this, "ShowReady");
        mainTimer.Connect("timeout", this, "SequenceFinished");

        Clear();
    }
}
