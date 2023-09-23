using MeshBuilding.Geometry;

namespace MeshBuilding.Mesh;

public struct Area
{
    public int ParameterNumber { get; init; }
    public int LeftBorderNumber { get; init; }
    public int RightBorderNumber { get; init; }
    public int BottomBorderNumber { get; init; }
    public int TopBorderNumber { get; init; }

    public Area(int leftBorderNumber, int rightBorderNumber, int bottomBorderNumber, int topBorderNumber, int parameterNumber)
    {
        LeftBorderNumber = leftBorderNumber;
        RightBorderNumber = rightBorderNumber;
        BottomBorderNumber = bottomBorderNumber;
        TopBorderNumber = topBorderNumber;
        ParameterNumber = parameterNumber;
    }
}

public class Border
{
    public int[] PointsIndices { get; set; }
    public BoundaryType BoundaryType { get; set; }
    public int FormulaIndex { get; set; }

    public Border(int[] pointsIndices, BoundaryType boundaryType, int formulaIndex)
    {
        PointsIndices = pointsIndices;
        BoundaryType = boundaryType;
        FormulaIndex = formulaIndex;
    }
}

public class MeshParameters
{
    public int AbscissaPointsCount { get; init; }
    public int OrdinatePointsCount { get; init; }
    public Point[] ControlPoints { get; init; } = null!;
    public Area[] Areas { get; init; } = null!;
    public Border[] Borders { get; init; } = null!;
    public Material[] Materials { get; init; } = null!;
    public int[] AbscissaSplits { get; init; } = null!;
    public int[] OrdinateSplits { get; init; } = null!;
    public double[] AbscissaK { get; init; } = null!;
    public double[] OrdinateK { get; init; } = null!;
    public int Refinement { get; init; }
}