namespace MeshBuilding.Mesh;

public struct AreaProperty
{
    public double Lambda { get; set; }
    public double Gamma { get; set; }
    public Func<double, double, double> Source { get; set; }

    public AreaProperty(double lambda, double gamma, Func<double, double, double> source)
    {
        Lambda = lambda;
        Gamma = gamma;
        Source = source;
    }
}