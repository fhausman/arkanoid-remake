using Godot;

public class AudioManager : Node2D
{
    private AudioStreamPlayer2D blockHit;
    private AudioStreamPlayer2D undestructableHit;
    private AudioStreamPlayer2D boardHit;
    private AudioStreamPlayer2D laserShot;
    private AudioStreamPlayer2D deathSound;
    private AudioStreamPlayer2D enemyDeath;
    private AudioStreamPlayer2D music;
    private AudioStreamPlayer2D teleport;
    private AudioStreamPlayer2D boardExtend;
    private AudioStreamPlayer2D powerUp;
    private AudioStream teleportOpen;
    private AudioStream teleportWarp;

    public void DestroyHit()
    {
        blockHit.Play();
    }

    public void NotDestructibleHit()
    {
        undestructableHit.Play();
    }

    public void BoardHit()
    {
        boardHit.Play();
    }

    public void LaserShot()
    {
        laserShot.Play();
    }

    public void Death()
    {
        deathSound.Play();
    }

    public void EnemyDeath()
    {
        enemyDeath.Play();
    }

    public void StartPlayingMusic()
    {
        music.Play();
    }

    public bool IsMusicPlaying()
    {
        return music.IsPlaying();
    }

    public void OpenTeleport()
    {
        teleport.Stream = teleportOpen;
        teleport.Play();
    }

    public void Warp()
    {
        teleport.Stream = teleportWarp;
        teleport.Play();
    }

    public void Extend()
    {
        boardExtend.Play();
    }

    public void PowerUp()
    {
        powerUp.Play();
    }

    public override void _Ready()
    {
        blockHit = GetNode<AudioStreamPlayer2D>("BlockHit");
        undestructableHit = GetNode<AudioStreamPlayer2D>("UndestructableHit");
        boardHit = GetNode<AudioStreamPlayer2D>("BoardHit");
        laserShot = GetNode<AudioStreamPlayer2D>("LaserShot");
        deathSound = GetNode<AudioStreamPlayer2D>("DeathSound");
        enemyDeath = GetNode<AudioStreamPlayer2D>("EnemyDeath");
        music = GetNode<AudioStreamPlayer2D>("Music");
        boardExtend = GetNode<AudioStreamPlayer2D>("BoardExtend");
        teleport = GetNode<AudioStreamPlayer2D>("Teleport");
        powerUp = GetNode<AudioStreamPlayer2D>("PowerUp");
        
        teleportOpen = (AudioStream) GD.Load("res://Sounds/teleport_open.wav");
        teleportWarp = (AudioStream) GD.Load("res://Sounds/teleport.wav");
    }
}
