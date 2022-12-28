namespace Sharposhop.Core.Model;

public readonly record struct PlaneCoordinate(AxisCoordinate X, AxisCoordinate Y)
{
    public static PlaneCoordinate Padded(int x, int y, PictureSize size)
    {
        AxisCoordinate xCoordinate;
        AxisCoordinate yCoordinate;

        if (x < 0)
        {
            xCoordinate = 0;
        }
        else if (x >= size.Width)
        {
            xCoordinate = size.Width - 1;
        }
        else
        {
            xCoordinate = x;
        }

        if (y < 0)
        {
            yCoordinate = 0;
        }
        else if (y >= size.Height)
        {
            yCoordinate = size.Height - 1;
        }
        else
        {
            yCoordinate = y;
        }

        return new PlaneCoordinate(xCoordinate, yCoordinate);
    }
    
    public static PlaneCoordinate operator +(PlaneCoordinate left, PlaneCoordinate right)
        => new PlaneCoordinate(left.X + right.X, left.Y + right.Y);
}