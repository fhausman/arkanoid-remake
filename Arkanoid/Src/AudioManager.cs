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

    public override void _Ready()
    {
        blockHit = GetNode<AudioStreamPlayer2D>("BlockHit");    
        undestructableHit = GetNode<AudioStreamPlayer2D>("UndestructableHit");    
        boardHit = GetNode<AudioStreamPlayer2D>("BoardHit");
        laserShot = GetNode<AudioStreamPlayer2D>("LaserShot");
        deathSound = GetNode<AudioStreamPlayer2D>("DeathSound");
        enemyDeath = GetNode<AudioStreamPlayer2D>("EnemyDeath");
    }
}
