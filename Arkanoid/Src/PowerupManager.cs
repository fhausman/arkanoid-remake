using Godot;
using System.Collections.Generic;

public class PowerupManager : Node2D
{
    [Export(PropertyHint.Range, "0,1")]
    public float PowerupSpawnProbability = 0.0f;
    private PackedScene laser;
    private PackedScene multiball;
    private PackedScene extraLife;
    private PackedScene boardExtension;
    private PackedScene slowdown;
    private PackedScene glue;
    //todo private PackedScene teleport;
    private bool IsMultiballActive { get => scene.GetTree().GetNodesInGroup("BALLS").Count > 1; }

    static private PowerupManager powerupManager;
    static private List<Node2D> readyInstances = new List<Node2D>();
    static private MainScene scene;
    static private Vector2 levelSpawnPosition;

    static public void SpawnPowerup(Vector2 blockPosition)
    {
        if(powerupManager.IsMultiballActive == false)
        {
            var instance = readyInstances[0];
            instance.Position = levelSpawnPosition + blockPosition;
            scene.AddChild(readyInstances[0]);
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
        glue = GD.Load<PackedScene>("res://Resources/PowerUps/Glue.tscn");

        readyInstances.Add(laser.Instance() as Node2D);
        readyInstances.Add(multiball.Instance() as Node2D);
        readyInstances.Add(extraLife.Instance() as Node2D);
        readyInstances.Add(boardExtension.Instance() as Node2D);
        readyInstances.Add(slowdown.Instance() as Node2D);
        readyInstances.Add(glue.Instance() as Node2D);

        levelSpawnPosition = GetNode<Node2D>("/root/Main/LevelSpawnPoint").Position;

        foreach(var powerup in readyInstances)
        {
            GD.Print(powerup);
        }
    }
}
