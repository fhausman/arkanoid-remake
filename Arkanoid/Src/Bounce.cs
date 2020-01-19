using Godot;

public static class Utils
{
    public static float DegreesToRadians(float angle)
    {
        return Mathf.Pi * angle / 180.0f;
    }

    public static Vector2 AngleToDir(float angle)
    {
        var radians = DegreesToRadians(angle);
        return new Vector2(Mathf.Cos(radians), -Mathf.Sin(radians));
    }

    public static Vector2 RotateVector(Vector2 vec, float angle)
    {
        var radians = DegreesToRadians(angle);
        var x = Mathf.Cos(radians)*vec.x - Mathf.Sin(radians)*vec.y;
        var y = Mathf.Sin(radians)*vec.x + Mathf.Cos(radians)*vec.y;

        var newVec = new Vector2(x,y);
        GD.Print("Old dir: ", vec, " New dir: ", newVec);

        return newVec;
    }
}

public static class Bounce
{
    public const float FirstAngle = 60.0f;
    public const float SecondAngle = 40.0f;
    public const float ThirdAngle = 20.0f;

    public static Vector2 BoardBounce(Ball ball, Vector2 board_pos, Vector2 board_extents, Vector2 col_pos)
    {
        Vector2 bounce_dir;
        var board_middle = board_pos.x;

        //right
        if(col_pos.x > board_middle)
        {
            //first angle
            if(col_pos.x < board_middle + board_extents.x * 0.60f)
            {
                GD.Print("First Angle");
                bounce_dir = Utils.AngleToDir(FirstAngle);
                ball.SpeedUp(ball.FirstAngleSpeedUp);
            }
            //second angle
            else if(col_pos.x < board_middle + board_extents.x)
            {
                GD.Print("Second Angle");
                bounce_dir = Utils.AngleToDir(SecondAngle);
                ball.SpeedUp(ball.SecondAngleSpeedUp);
            }
            //thrid angle (hit in side of board)
            else
            {
                GD.Print("Third Angle");
                bounce_dir = Utils.AngleToDir(ThirdAngle);
                ball.SpeedUp(ball.ThirdAngleSpeedUp);
            }
        }
        //left
        else
        {
            //first angle
            if(col_pos.x > board_middle - board_extents.x * 0.60f)
            {
                GD.Print("First Angle");
                bounce_dir = Utils.AngleToDir(180.0f - FirstAngle);
                ball.SpeedUp(ball.FirstAngleSpeedUp);
            }
            //second angle
            else if(col_pos.x > board_pos.x)
            {
                GD.Print("Second Angle");
                bounce_dir = Utils.AngleToDir(180.0f - SecondAngle);
                ball.SpeedUp(ball.SecondAngleSpeedUp);
            }
            //thrid angle (hit in side of board)
            else
            {
                GD.Print("Third Angle");
                bounce_dir = Utils.AngleToDir(180.0f - ThirdAngle);
                ball.SpeedUp(ball.ThirdAngleSpeedUp);
            }
        }

        GD.Print("Board position: ", board_pos, " collision position: ", col_pos);

        return bounce_dir;
    }
}