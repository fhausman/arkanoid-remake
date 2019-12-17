using Godot;
using System;

public class Multiball : BasePowerUp
{
    MainScene scene;
    Ball ball;
    PackedScene ballScene;

    public override void OnCollect()
    {
        ball = (Ball) GetTree().GetNodesInGroup("BALLS")[0];
        var extra_ball1 = (Ball) ballScene.Instance();
        extra_ball1.MovingAtStart = true;
        extra_ball1.StartingDir = Bounce.RotateVector(ball.CurrentDir, 10);
        scene.AddChild(extra_ball1);
        extra_ball1.Position = ball.Position;
        extra_ball1.CurrentSpeed = ball.CurrentSpeed;

        GD.Print(GetTree().GetNodesInGroup("BALLS").Count);
    }

    public override void _Ready()
    {
        scene = GetNode<MainScene>("/root/Main");
        ballScene = GD.Load<PackedScene>("res://Resources/Ball/Ball.tscn");
    }
}
