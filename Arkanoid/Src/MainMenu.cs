using Godot;

public class MainMenu : Control
{
    private PackedScene mainScene;
    private PackedScene credits;
    private AudioStreamPlayer2D menuSound;
    private AudioStreamPlayer2D startSound;
    private bool startGame = false;

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
        GetNode<Button>("VBoxContainer/Credits").ReleaseFocus();
    }

    public void OnFocusEnter()
    {
        menuSound.Play();
    }

    public override void _Ready()
    {
        Input.SetMouseMode(Input.MouseMode.Hidden);

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
