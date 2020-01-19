public class ExtraLife : BasePowerUp
{
    private MainScene scene;

    public override void OnCollect()
    {
        scene.AddExtraLife();
        scene.GetNode<AudioManager>("AudioManager").PowerUp();
        base.OnCollect();
    }

    public override void _Ready()
    {
        base._Ready();
        scene = GetNode<MainScene>("/root/Main");
        animation.Play("life");
    }
}
