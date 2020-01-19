using Godot;
using System;

public class Block : StaticBody2D, IHittable
{
    [Export]
    public bool Destructable { get; set; } = true;
    [Export]
    public int NumOfHits { get; set; } = 1;
    [Export]
    public int Points { get; set; } = 100;
    protected AudioManager audio;
    private MainScene scene;


    public override void _Ready()
    {
        scene = GetNode<MainScene>("/root/Main");
        audio = scene.GetNode<AudioManager>("AudioManager");

        Points += ((int) MainScene.CurrentLevel)*Points;
    }

    public virtual void OnHit()
    {
        if(!Destructable)
        {
            audio.NotDestructibleHit();
            return;
        }

        NumOfHits--;
        if(NumOfHits <= 0)
        {
            audio.DestroyHit();

            scene.Score += Points;
            PowerupManager.SpawnPowerup(GlobalPosition + new Vector2(32, 16));
            Free();
            scene.CheckIfPlayerDestroyedAllBlocks();
        }
    }
}
