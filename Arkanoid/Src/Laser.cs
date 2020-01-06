using Godot;

public class Laser : BasePowerUp
{
     public override void OnCollect()
    {
        PowerupManager.ResetPowerups();

        var board = GetNode<Board>("/root/Main/Board");
        board.EnableLaser();
        base.OnCollect();
    }

    public override void _Ready()
    {
        base._Ready();
        animation.Play("laser");
    }
}
