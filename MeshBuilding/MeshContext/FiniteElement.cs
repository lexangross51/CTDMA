using MeshBuilding.Geometry;

namespace MeshBuilding.MeshContext;

public class FiniteElement
{
    public IReadOnlyList<int> Nodes { get; }
    public int AreaNumber { get; set; }
    public IReadOnlyList<Edge> Edges { get; }

    public FiniteElement(int[] nodes, int areaNumber = 0)
    {
        Nodes = nodes;
        AreaNumber = areaNumber;
        Edges = new Edge[]
        {
            new(nodes[0], nodes[1]),
            new(nodes[0], nodes[2]),
            new(nodes[1], nodes[3]),
            new(nodes[2], nodes[3])
        };
    }
}