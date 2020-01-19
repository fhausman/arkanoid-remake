using Godot;

public class Multiball : BasePowerUp
{
    [Export]
    public float AngleBetweenBalls = 10.0f;
    private MainScene scene;
    private Ball ball;
    private PackedScene ballScene;

    private void SpawnBall(Ball existingBall, float angleOffset)
    {
        var extraBall = (Ball) ballScene.Instance();
        extraBall.MovingAtStart = true;
        extraBall.StartingDir = Utils.RotateVector(ball.CurrentDir, angleOffset);
        extraBall.Board = scene.GetNode<Board>("Board");
        extraBall.Position = ball.Position;
        scene.AddChild(extraBall);
        extraBall.SetSpeed(ball.CurrentSpeed);
    }

    private void SpawnBalls()
    {
        foreach(var i in new int[]{-1, 1})
        {
            SpawnBall(ball, AngleBetweenBalls*i);
        }
    }

    public override void OnCollect()
    {
        PowerupManager.ResetPowerups();

        ball = (Ball) GetTree().GetNodesInGroup("BALLS")[0];

        SpawnBalls();
        GD.Print(GetTree().GetNodesInGroup("BALLS").Count);

        base.OnCollect();
    }

    public override void _Ready()
    {
        scene = GetNode<MainScene>("/root/Main");
        ballScene = GD.Load<PackedScene>("res://Resources/Ball/Ball.tscn");

        base._Ready();
        animation.Play("multi");
    }
}
