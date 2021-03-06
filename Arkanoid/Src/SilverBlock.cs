using Godot;

public class SilverBlock : Block
{
    private AnimationPlayer animationPlayer;

    public override void _Ready()
    {
        animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        base._Ready();
    }

    public override void OnHit()
    {
        animationPlayer.Play("damaged");
        if(NumOfHits > 1)
            audio.NotDestructibleHit();
            
        base.OnHit();
    }
}
