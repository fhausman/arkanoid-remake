public class Teleport : BasePowerUp
{
    public override void OnCollect()
    {
        PowerupManager.ActivateTeleport();
        base.OnCollect();
    }

    public override void _Ready()
    {
        base._Ready();
        animation.Play("teleport");
    }
}
