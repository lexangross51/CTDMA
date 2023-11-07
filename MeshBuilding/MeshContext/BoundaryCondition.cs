using MeshBuilding.Geometry;

namespace MeshBuilding.MeshContext;

public struct Dirichlet
{
    public int Node { get; set; }
    public Func<double, double, double> Value { get; set; }

    public Dirichlet(int node, Func<double, double, double> value)
    {
        Node = node;
        Value = value;
    }

    public void Deconstruct(out int node, out Func<double, double, double> value)
    {
        node = Node;
        value = Value;
    }
}

public struct Neumann
{
    public Edge Border { get; set; }
    public Func<double, double, double> Theta { get; set; }

    public Neumann(Edge border, Func<double, double, double> theta)
    {
        Border = border;
        Theta = theta;
    }
}