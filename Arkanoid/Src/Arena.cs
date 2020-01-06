using Godot;
using System;

public class Arena : Node2D
{
    Node2D gateUpperLeft;
    Node2D gateUpperRight;
    Node2D gateTeleport;

    public void OpenTeleport()
    {
        OpenGate(gateTeleport);
    }
    
    public void CloseTeleport()
    {
        CloseGate(gateTeleport);
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
        gateUpperLeft = GetNode<Node2D>("GateUpperLeft");
        gateUpperRight = GetNode<Node2D>("GateUpperRight");
        gateTeleport = GetNode<Node2D>("GateTeleport");
    }
}
