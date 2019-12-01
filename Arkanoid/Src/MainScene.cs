using Godot;
using System;
using System.Linq;

public class MainScene : Node2D
{
    public int NumberOfLives { get; set; } = 2;
    private Node2D blocks;
    private Control ui;
    private Control livesContainer;
    private PackedScene boardIcon;
    void CheckIfPlayerDestroyedAllBlocks()
    {
        var blocks_count = blocks.GetChildCount();
        GD.Print("Checkin... ", blocks_count);
        if(blocks_count == 0)
        {
            GD.Print("Woohoo, level won, going to the next stage");
        }
    }

    void DecreaseNumberOfIcons()
    {
        var children = livesContainer.GetChildren().Cast<TextureRect>().ToArray();
        children[children.Length - 1].QueueFree();
    }

    public void OnDeathAreaEntered(PhysicsBody2D body)
    {
        if (body is Ball ball)
        {
            NumberOfLives--;
            if(NumberOfLives >= 0)
            {
                DecreaseNumberOfIcons();
                ball.ResetState();
                GD.Print("Ball entered death zone. ", NumberOfLives, " chances left!");
            }
            else
            {
                GD.Print("Game over");
                GetTree().ReloadCurrentScene();
            }
        }
    }

    public override void _Ready()
    {

        boardIcon = GD.Load<PackedScene>("res://Resources/UI/BoardIcon.tscn");
        blocks = GetNode<Node2D>("Blocks");
        GetNode("Ball").Connect("CheckWin", this, nameof(CheckIfPlayerDestroyedAllBlocks));
        ui = GetNode<Control>("UI");
        livesContainer = ui.GetNode<Control>("LivesContainer");
        foreach(var i in Enumerable.Range(1, NumberOfLives))
        {
            livesContainer.AddChild(boardIcon.Instance());
        }
    }
}
