using Godot;

public class Arena : Node2D
{
    private Node2D gateUpperLeft;
    private Node2D gateUpperRight;
    private Node2D gateTeleport;
    private Timer leftGateTimer;
    private Timer rightGateTimer;
    private AudioManager audio;

    public Vector2 LeftSpawnPoint { get; private set; }
    public Vector2 RightSpawnPoint { get; private set; }

    public void OpenTeleport()
    {
        audio.OpenTeleport();
        OpenGate(gateTeleport);
    }
    
    public void CloseTeleport()
    {
        CloseGate(gateTeleport);
    }

    public void OpenLeftGate()
    {
        leftGateTimer.Start();
        OpenGate(gateUpperLeft);
    }

    public void CloseLeftGate()
    {
        CloseGate(gateUpperLeft);
    }

    public void OpenRightGate()
    {
        rightGateTimer.Start();
        OpenGate(gateUpperRight);

    }

    public void CloseRightGate()
    {
        CloseGate(gateUpperRight);
    }

    private void OpenGate(Node2D gate)
    {
        gate.GetNode<AnimationPlayer>("AnimationPlayer").Play("open");
    }

    private void CloseGate(Node2D gate)
    {
        gate.GetNode<AnimationPlayer>("AnimationPlayer").Play("close");
    }

    public override void _Ready()
    {
        audio = GetNode<AudioManager>("../AudioManager");

        gateUpperLeft = GetNode<Node2D>("GateUpperLeft");
        gateUpperRight = GetNode<Node2D>("GateUpperRight");
        gateTeleport = GetNode<Node2D>("GateTeleport");

        leftGateTimer = GetNode<Timer>("LeftGateTimer");
        rightGateTimer = GetNode<Timer>("RightGateTimer");

        LeftSpawnPoint = GetNode<Node2D>("LeftSpawnPoint").Position;
        RightSpawnPoint = GetNode<Node2D>("RightSpawnPoint").Position;
    }
}
