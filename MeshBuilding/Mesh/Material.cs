namespace MeshBuilding.Mesh;

public struct Material
{
    public double Lambda { get; set; }
    public double Gamma { get; set; }

    public Material(double lambda, double gamma)
    {
        Lambda = lambda;
        Gamma = gamma;
    }
}