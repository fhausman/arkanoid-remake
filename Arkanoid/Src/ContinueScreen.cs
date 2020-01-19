using Godot;

public class ContinueScreen : Control
{
    private int SecondsLeft = 10;
    private Label TimeLeft;
    private PackedScene menu;

    public void Countdown()
    {
        SecondsLeft--;
        if(SecondsLeft < 0)
        {
            LoadMenu();
        }
        TimeLeft.Text = SecondsLeft.ToString();
    }

    public void OnYesPressed()
    {
        GetTree().ReloadCurrentScene();
    }

    public void OnNoPressed()
    {
        LoadMenu();
    }

    private void LoadMenu()
    {
        GetTree().ChangeSceneTo(menu);
        GetTree().Paused = false;
    }

    public override void _Ready()
    {
        menu = GD.Load<PackedScene>("res://MainMenu.tscn");
        TimeLeft = GetNode<Label>("CenterContainer/VBoxContainer/Countdown");
    }
}
