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
    private PackedScene teleport;
    private List<PackedScene> powerUps;
    private bool IsMultiballActive { get => scene.GetTree().GetNodesInGroup("BALLS").Count > 1; }
    private bool IsAnyPowerUpOnScene { get => scene.GetTree().GetNodesInGroup("POWERUPS").Count > 0; }

    static private PowerupManager powerupManager;
    static private MainScene scene;
    static private Arena arena;
    static private Vector2 levelSpawnPosition;
    static private RandomNumberGenerator randGen = new RandomNumberGenerator();
    static public bool IsTeleportActive { get; private set; } = false;
    
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
        if(powerupManager.IsMultiballActive == false &&
            powerupManager.IsAnyPowerUpOnScene == false &&
            randGen.RandfRange(0.0f, 1.0f) < powerupManager.PowerupSpawnProbability)
        {
            PackedScene powerUp = DrawPowerUp(GetAvailablePowerups());
            var instance = powerUp.Instance() as Node2D;
            if(instance is ExtraLife || instance is Teleport)
            {
                powerupManager.powerUps.Remove(powerUp);
            }

            instance.Position = levelSpawnPosition + blockPosition;
            scene.AddChild(instance);
        }
    }

    static private PackedScene DrawPowerUp(List<PackedScene> availablePowerUps)
    {
        var num = randGen.RandfRange(0.0f, 1.0f);
        var range = 1.0f / availablePowerUps.Count;
        for(int i = 0; i < availablePowerUps.Count; ++i)
        {
            if(i*range <= num && num < (i+1)*range)
            {
                return availablePowerUps[i];
            }
        }

        return null;
    }

    static private List<PackedScene> GetAvailablePowerups()
    {
        var activePowerUps = new List<PackedScene>(powerupManager.powerUps);
        var ball = (Ball) scene.GetTree().GetNodesInGroup("BALLS")[0];
        var board = scene.GetNode<Board>("Board");

        if(ball.GlueToBoard)
        {
            activePowerUps.Remove(powerupManager.glue);
        }
        else if(board.IsLaserActive)
        {
            activePowerUps.Remove(powerupManager.laser);
        }
        else if(board.IsExtended)
        {
            GD.Print("Fuck off");
            activePowerUps.Remove(powerupManager.boardExtension);
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

    public List<PackedScene> GetAllAvailablePowerUps()
    {
        return new List<PackedScene>() {laser, multiball, extraLife, boardExtension, slowdown, glue, teleport};
    }

    static public void ResetState()
    {
        powerupManager.powerUps = powerupManager.GetAllAvailablePowerUps();
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

        powerUps = GetAllAvailablePowerUps();

        levelSpawnPosition = GetNode<Node2D>("/root/Main/LevelSpawnPoint").Position;
    }
}
