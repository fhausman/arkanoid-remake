using Godot;

public class Laser : BasePowerUp
{
     public override void OnCollect()
    {
        var board = GetNode<Board>("/root/Main/Board");
        board.EnableLaser();
        base.OnCollect();
    }
}
