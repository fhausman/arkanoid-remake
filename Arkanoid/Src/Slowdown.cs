public class Slowdown : BasePowerUp
{
    public override void OnCollect()
    {
        PowerupManager.ResetPowerups();

        GetTree().CallGroup("BALLS", "ResetSpeed");
        base.OnCollect();
    }

    public override void _Ready()
    {
        base._Ready();
        animation.Play("slow");
    }
}
