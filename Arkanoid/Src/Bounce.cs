using Godot;
using System;

public static class Bounce
{
    public const float FirstAngle = 60.0f;
    public const float SecondAngle = 40.0f;
    public const float ThirdAngle = 20.0f;

    public static Vector2 AngleToDir(float angle)
    {
        var radians = Mathf.Pi * angle / 180.0f;
        return new Vector2(Mathf.Cos(radians), -Mathf.Sin(radians));
    }

    public static Vector2 BoardBounce(Vector2 board_pos, Vector2 board_extents, Vector2 col_pos)
    {
        Vector2 bounce_dir;

        var board_middle = board_pos.x + board_extents.x;
        //right
        if(col_pos.x > board_middle)
        {
            //first angle
            if(col_pos.x < board_middle + board_extents.x * 0.60f)
            {
                GD.Print("First Angle");
                GD.Print("Board position: ", board_pos, " collision position: ", col_pos);
                bounce_dir = AngleToDir(FirstAngle);
            }
            //second angle
            else if(col_pos.x < board_middle + board_extents.x)
            {
                GD.Print("Second Angle");
                GD.Print("Board position: ", board_pos, " collision position: ", col_pos);
                bounce_dir = AngleToDir(SecondAngle);
            }
            //thrid angle (hit in side of board)
            else
            {
                GD.Print("Third Angle");
                GD.Print("Board position: ", board_pos, " collision position: ", col_pos);
                bounce_dir = AngleToDir(ThirdAngle);
            }
        }
        //left
        else
        {
            //first angle
            if(col_pos.x > board_middle - board_extents.x * 0.60f)
            {
                GD.Print("First Angle");
                GD.Print("Board position: ", board_pos, " collision position: ", col_pos);
                bounce_dir = AngleToDir(180.0f - FirstAngle);
            }
            //second angle
            else if(col_pos.x > board_pos.x)
            {
                GD.Print("Second Angle");
                GD.Print("Board position: ", board_pos, " collision position: ", col_pos);
                bounce_dir = AngleToDir(180.0f - SecondAngle);
            }
            //thrid angle (hit in side of board)
            else
            {
                GD.Print("Third Angle");
                GD.Print("Board position: ", board_pos, " collision position: ", col_pos);
                bounce_dir = AngleToDir(180.0f - ThirdAngle);
            }
        }


        return bounce_dir;
    }
}