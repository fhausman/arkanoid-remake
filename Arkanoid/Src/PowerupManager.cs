using Godot;
using System;

public class PowerupManager : Node2D
{
    [Export(PropertyHint.Range, "0,1")]
    public float PowerupSpawnProbability = 0.0f;
    private MainScene scene;
    private PackedScene laser;
    private PackedScene multiball;
    private PackedScene extraLife;
    private PackedScene boardExtension;
    private PackedScene slowdown;
    private PackedScene glue;
    //todo private PackedScene teleport;
    private bool IsMultiballActive { get => scene.GetTree().GetNodesInGroup("BALLS").Count > 1; }

    static private PowerupManager powerupManager;

    static public void SpawnPowerup(Vector2 blockPosition)
    {
        if(powerupManager.IsMultiballActive == false)
        {

        }
    }

    public override void _Ready()
    {
        powerupManager = this;

        scene = GetNode<MainScene>("/root/Main");
        laser = GD.Load<PackedScene>("res://Resources/PowerUps/Laser.tscn");
        multiball = GD.Load<PackedScene>("res://Resources/PowerUps/Multiball.tscn");
        extraLife = GD.Load<PackedScene>("res://Resources/PowerUps/ExtraLife.tscn");
        boardExtension = GD.Load<PackedScene>("res://Resources/PowerUps/BoardExtend.tscn");
        slowdown = GD.Load<PackedScene>("res://Resources/PowerUps/Slowdown.tscn");
        glue = GD.Load<PackedScene>("res://Resources/PowerUps/glue.tscn");
    }
}
