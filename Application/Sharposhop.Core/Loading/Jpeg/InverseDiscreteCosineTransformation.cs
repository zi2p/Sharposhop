namespace Sharposhop.Core.Loading.Jpeg;

public class InverseDiscreteCosineTransformation
{
    private const int IdctSize = 8;
    private static readonly double[][] ZigZagIterations =
    {
        new double[] { 00, 01, 05, 06, 14, 15, 27, 28 },
        new double[] { 02, 04, 07, 13, 16, 26, 29, 42 },
        new double[] { 03, 08, 12, 17, 25, 30, 41, 43 },
        new double[] { 09, 11, 18, 24, 31, 40, 44, 53 },
        new double[] { 10, 19, 23, 32, 39, 45, 52, 54 },
        new double[] { 20, 22, 33, 38, 46, 51, 55, 60 },
        new double[] { 21, 34, 37, 47, 50, 56, 59, 61 },
        new double[] { 35, 36, 48, 49, 57, 58, 62, 63 }
    };

    private readonly double[][] _zigZag;
    private readonly double[] _baseMatrix = new double[IdctSize * IdctSize];
    private readonly double[,] _idctMatrix = new double[IdctSize, IdctSize];

    public InverseDiscreteCosineTransformation()
    {
        _zigZag = ZigZagIterations.ToArray();
        for (var i = 0; i < IdctSize; i++)
        {
            for (var j = 0; j < IdctSize; j++)
            {
                _idctMatrix[i, j] = (i != 0 ? 1 : 1 / Math.Sqrt(2)) * Math.Cos((2 * j + 1) * i * Math.PI / (2.0 * IdctSize));
            }
        }
    }

    public void PerformZigZag()
    {
        for (var i = 0; i < IdctSize; i++)
        {
            for (var j = 0; j < IdctSize; j++)
            {
                _zigZag[i][j] = _baseMatrix[(int)_zigZag[i][j]];
            }
        }
    }

    public double[,] PerformIdct()
    {
        var result = new double[8, 8];

        for (var x = 0; x < IdctSize; x++)
        {
            for (var y = 0; y < IdctSize; y++)
            {
                var tempSum = 0.0;
                for (var u = 0; u < IdctSize; u++)
                {
                    for (var v = 0; v < IdctSize; v++)
                    {
                        tempSum += _idctMatrix[u, x] * _idctMatrix[v, y] * _zigZag[v][u];
                    }
                }

                result[y, x] = Math.Floor(tempSum / 4.0);
            }
        }

        return result;
    }
}