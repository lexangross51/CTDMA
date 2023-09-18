namespace MeshBuilding.Mesh;

public class FiniteElement
{
    public int[] Nodes { get; }
    public int AreaNumber { get; set; }

    public FiniteElement(int[] nodes, int areaNumber = 0)
    {
        Nodes = nodes;
        AreaNumber = areaNumber;
    }
}