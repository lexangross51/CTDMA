using MeshBuilding.Geometry;

namespace MeshBuilding.Mesh;

public struct Area
{
    public int ParameterNumber { get; init; }
    public int LeftBorderNumber { get; init; }
    public int RightBorderNumber { get; init; }
    public int BottomBorderNumber { get; init; }
    public int TopBorderNumber { get; init; }

    public Area(int parameterNumber, int leftBorderNumber, int rightBorderNumber, int bottomBorderNumber, int topBorderNumber)
    {
        ParameterNumber = parameterNumber;
        LeftBorderNumber = leftBorderNumber;
        RightBorderNumber = rightBorderNumber;
        BottomBorderNumber = bottomBorderNumber;
        TopBorderNumber = topBorderNumber;
    }
}

public class MeshParameters
{
    public int AbscissaPointsCount { get; init; }
    public int OrdinatePointsCount { get; init; }
    public Point[] ControlPoints { get; init; }
    public Area[] Areas { get; init; }
    public int[] AbscissaSplits { get; init; }
    public int[] OrdinateSplits { get; init; }
    public double[] AbscissaK { get; init; }
    public double[] OrdinateK { get; init; }
}