using System;

public static class Utils
{
    public static Direction GetOppositeDirection(Direction direction)
    {
        switch (direction)
        {
            case Direction.Left:
                return Direction.Right;
            case Direction.Right:
                return Direction.Left;
            case Direction.Up:
                return Direction.Down;
            case Direction.Down:
                return Direction.Up;
            default:
                throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
        }
    }
}
