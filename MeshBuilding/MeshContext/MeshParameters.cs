using MeshBuilding.Geometry;
using MeshBuilding.MeshContext.JsonConverter;
using Newtonsoft.Json;

namespace MeshBuilding.MeshContext;

public struct Area
{
    public int ParameterNumber { get; set; }
    public int LeftBorderNumber { get; set; }
    public int RightBorderNumber { get; set; }
    public int BottomBorderNumber { get; set; }
    public int TopBorderNumber { get; set; }

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

[JsonConverter(typeof(MeshParametersJsonConverter))]
public class MeshParameters
{
    public int AbscissaPointsCount { get; init; }
    public int OrdinatePointsCount { get; init; }
    public Point[] ControlPoints { get; init; } = null!;
    public Area[] Areas { get; init; } = null!;
    public Border[] Borders { get; init; } = null!;
    public Func<double, double, double>[] BoundaryFormulas { get; init; } = null!;
    public AreaProperty[] AreaProperties { get; init; } = null!;
    public int[] AbscissaSplits { get; init; } = null!;
    public int[] OrdinateSplits { get; init; } = null!;
    public double[] AbscissaK { get; init; } = null!;
    public double[] OrdinateK { get; init; } = null!;
    public int Refinement { get; init; }

    public static MeshParameters ReadJson(string path)
    {
        using var sr = new StreamReader(path);
        return JsonConvert.DeserializeObject<MeshParameters>(sr.ReadToEnd());
    }
}