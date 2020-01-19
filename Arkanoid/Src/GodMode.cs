using Godot;

public class GodMode : Node2D
{
    private Timer keyDelay;
    private bool enableInput = true;

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

    public void Spawn(Node2D node)
    {
        node.Position = Position;
        GetNode<MainScene>("..").AddChild(node);
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
        else if(Input.IsKeyPressed((int) KeyList.Key1))
        {
            Spawn(PowerupManager.SpawnLaser());
            DelayInput();
        }
        else if(Input.IsKeyPressed((int) KeyList.Key2))
        {
            Spawn(PowerupManager.SpawnMultiball());
            DelayInput();
        }
        else if(Input.IsKeyPressed((int) KeyList.Key3))
        {
            Spawn(PowerupManager.SpawnExtraLife());
            DelayInput();
        }
        else if(Input.IsKeyPressed((int) KeyList.Key4))
        {
            Spawn(PowerupManager.SpawnExtension());
            DelayInput();
        }
        else if(Input.IsKeyPressed((int) KeyList.Key5))
        {
            Spawn(PowerupManager.SpawnSlowdown());
            DelayInput();
        }
        else if(Input.IsKeyPressed((int) KeyList.Key6))
        {
            Spawn(PowerupManager.SpawnGlue());
            DelayInput();
        }
        else if(Input.IsKeyPressed((int) KeyList.Key7))
        {
            Spawn(PowerupManager.SpawnTeleport());
            DelayInput();
        }
    }
}
