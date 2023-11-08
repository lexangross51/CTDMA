namespace FieldsDrawer.Core.Graphics.Objects.Mesh;

public class Element
{
    public int[] Nodes { get; }
    public Edge[] Edges { get; }

    public Element(int[] nodes)
    {
        Nodes = nodes;
        Edges = new Edge[nodes.Length];
        
        MakeEdges();
    }

    private void MakeEdges()
    {
        for (int i = 0; i < Nodes.Length; i++)
        {
            Edges[i] = new Edge(Nodes[i], Nodes[(i + 1) % Nodes.Length]);
        }
    }

}