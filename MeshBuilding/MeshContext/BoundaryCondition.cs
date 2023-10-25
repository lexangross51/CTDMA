namespace MeshBuilding.MeshContext;

public struct Dirichlet
{
    public int Node { get; }
    public double Value { get; }

    public Dirichlet(int node, double value)
    {
        Node = node;
        Value = value;
    }
}

public struct Neumann
{
    public int BorderStart { get; }
    public int BorderEnd { get; }
    public Func<double, double, double> Theta { get; }

    public Neumann(int borderStart, int borderEnd, Func<double, double, double> theta)
    {
        BorderStart = borderStart;
        BorderEnd = borderEnd;
        Theta = theta;
    }
}