using MeshBuilding.Geometry;

namespace MeshBuilding.Mesh;

public class Mesh
{
    public Point[] Points { get; }
    public FiniteElement[] Elements { get; }
    public AreaProperty[] Materials { get; }
    public Dirichlet[] Dirichlet { get; }
    public Neumann[]? Neumann { get; }
    public int[]? FictitiousNodes { private get; set; }
    public int[]? FictitiousElements { private get; set; }

    public Mesh(IEnumerable<Point> points, IEnumerable<FiniteElement> elements, IEnumerable<AreaProperty> materials,
        IEnumerable<Dirichlet> dirichlet, IEnumerable<Neumann>? neumann = null)
    {
        Points = points.ToArray();
        Elements = elements.ToArray();
        Materials = materials.ToArray();
        Dirichlet = dirichlet.ToArray();
        Neumann = neumann?.ToArray();
    }

    public bool IsNodeFictitious(int node)
        => FictitiousNodes?.Any(n => n == node) ?? false;
    
    public bool IsElementFictitious(int ielem)
        => FictitiousElements?.Any(elem => elem == ielem) ?? false;
}