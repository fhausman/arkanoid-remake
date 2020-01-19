using Godot;
using System.Collections.Generic;
using System.Linq;

public class PowerupManager : Node2D
{
    [Export(PropertyHint.Range, "0,1")]
    public float PowerupSpawnProbability { get; set; } = 0.0f;
    static private PackedScene laser;
    static private PackedScene multiball;
    static private PackedScene extraLife;
    static private PackedScene boardExtension;
    static private PackedScene slowdown;
    static private PackedScene glue;
    static private PackedScene teleport;
    static private Dictionary<PackedScene, float> powerUpsProbability;
    static private PowerupManager powerupManager;
    static private MainScene scene;
    static private Arena arena;
    static private Vector2 levelSpawnPosition;
    static private RandomNumberGenerator randGen = new RandomNumberGenerator();
    static public bool IsTeleportActive { get; private set; } = false;
    static private bool IsMultiballActive { get => scene.GetTree().GetNodesInGroup("BALLS").Count > 1; }
    static private bool IsAnyPowerUpOnScene { get => scene.GetTree().GetNodesInGroup("POWERUPS").Count > 0; }
    
    static public void ActivateTeleport()
    {
        IsTeleportActive = true;
        arena.OpenTeleport();
    }

    static public void DectivateTeleport()
    {
        IsTeleportActive = false;
        arena.CloseTeleport();
    }

    static public void SpawnPowerup(Vector2 blockPosition)
    {
        randGen.Randomize();
        if(IsMultiballActive == false &&
            IsAnyPowerUpOnScene == false &&
            randGen.RandfRange(0.0f, 1.0f) < powerupManager.PowerupSpawnProbability)
        {
            PackedScene powerUp = DrawPowerUp(GetAvailablePowerups());
            var instance = powerUp.Instance() as Node2D;
            if(instance is ExtraLife || instance is Teleport)
            {
                powerUpsProbability.Remove(powerUp);
            }

            instance.Position = blockPosition;
            scene.AddChild(instance);
        }
    }

    static private PackedScene DrawPowerUp(Dictionary<PackedScene, float> availablePowerUps)
    {
        randGen.Randomize();
        var range = availablePowerUps.Values.Sum(p => p);
        var num = randGen.RandfRange(0.0f, range);
        var sum = 0.0f;
        foreach(var powerUp in availablePowerUps)
        {
            var prob = powerUp.Value;
            if(sum <= num && num < sum + prob)
            {
                return powerUp.Key;
            }

            sum += prob;
        }

        return null;
    }

    static private Dictionary<PackedScene, float> GetAvailablePowerups()
    {
        var activePowerUps = new Dictionary<PackedScene, float>(powerUpsProbability);
        var ball = (Ball) scene.GetTree().GetNodesInGroup("BALLS")[0];
        var board = scene.GetNode<Board>("Board");

        if(ball.GlueToBoard)
        {
            activePowerUps.Remove(glue);
        }
        else if(board.IsLaserActive)
        {
            activePowerUps.Remove(laser);
        }
        else if(board.IsExtended)
        {
            GD.Print("Fuck off");
            activePowerUps.Remove(boardExtension);
        }

        return activePowerUps;
    }

    static public void RegainPowerup(Node2D powerUp)
    {
        GD.Print("Regaining power-up! ");
        powerUp.Free();
    }

    static public void ResetPowerups()
    {
        var ball = (Ball) scene.GetTree().GetNodesInGroup("BALLS")[0];
        var board = scene.GetNode<Board>("Board");

        ball.ResetPowerups();
        board.ResetPowerups();
    }

    static public Dictionary<PackedScene, float> GetAllAvailablePowerUps()
    {
        return new Dictionary<PackedScene, float>()
        {
            {laser, 0.168f},
            {multiball, 0.168f},
            {boardExtension, 0.168f},
            {slowdown, 0.168f},
            {glue, 0.168f},
            {teleport, 0.08f},
            {extraLife, 0.08f}
        };
    }

    static public void ResetState()
    {
        powerUpsProbability = GetAllAvailablePowerUps();
    }

    public static Laser SpawnLaser()
    {
        return (Laser) laser.Instance();
    }

    public static Multiball SpawnMultiball()
    {
        return (Multiball) multiball.Instance();
    }

    public static ExtraLife SpawnExtraLife()
    {
        return (ExtraLife) extraLife.Instance();
    }

    public static BoardExtend SpawnExtension()
    {
        return (BoardExtend) boardExtension.Instance();
    }

    public static Slowdown SpawnSlowdown()
    {
        return (Slowdown) slowdown.Instance();
    }

    public static Glue SpawnGlue()
    {
        return (Glue) glue.Instance();
    }

    public static Teleport SpawnTeleport()
    {
        return (Teleport) teleport.Instance();
    }
    public override void _Ready()
    {
        powerupManager = this;

        scene = GetNode<MainScene>("/root/Main");
        arena = scene.GetNode<Arena>("Arena");
        laser = GD.Load<PackedScene>("res://Resources/PowerUps/Laser.tscn");
        multiball = GD.Load<PackedScene>("res://Resources/PowerUps/Multiball.tscn");
        extraLife = GD.Load<PackedScene>("res://Resources/PowerUps/ExtraLife.tscn");
        boardExtension = GD.Load<PackedScene>("res://Resources/PowerUps/BoardExtend.tscn");
        slowdown = GD.Load<PackedScene>("res://Resources/PowerUps/Slowdown.tscn");
        glue = GD.Load<PackedScene>("res://Resources/PowerUps/Glue.tscn");
        teleport = GD.Load<PackedScene>("res://Resources/PowerUps/Teleport.tscn");

        powerUpsProbability = GetAllAvailablePowerUps();
    }
}
