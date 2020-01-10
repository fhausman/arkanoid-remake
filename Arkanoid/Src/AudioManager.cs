using Godot;
using System;

public class AudioManager : Node2D
{
    AudioStreamPlayer2D blockHit;
    AudioStreamPlayer2D undestructableHit;
    AudioStreamPlayer2D boardHit;
    AudioStreamPlayer2D laserShot;
    AudioStreamPlayer2D deathSound;
    AudioStreamPlayer2D enemyDeath;
    AudioStreamPlayer2D music;
    AudioStreamPlayer2D teleport;
    AudioStreamPlayer2D boardExtend;
    AudioStream teleportOpen;
    AudioStream teleportWarp;

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
        teleportOpen = (AudioStream) GD.Load("res://Sounds/teleport_open.wav");
        teleportWarp = (AudioStream) GD.Load("res://Sounds/teleport.wav");
    }
}
