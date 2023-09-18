using System.Globalization;
using MeshBuilding.Geometry;

namespace MeshBuilding.Mesh;

public class MeshBuilder : IMeshBuilder
{
    private readonly MeshParameters _meshParameters;
    private Point[] _points;
    private FiniteElement[] _elements;

    public MeshBuilder(MeshParameters meshParameters)
    {
        var totalNx = meshParameters.AbscissaSplits.Sum();
        var totalNy = meshParameters.OrdinateSplits.Sum();
        
        _meshParameters = meshParameters;
        _points = new Point[(totalNx + 1) * (totalNy + 1)];
        _elements = new FiniteElement[totalNx * totalNy];
    }
    
    public void CreatePoints()
    {
        int totalNx = _meshParameters.AbscissaSplits.Sum() + 1;  // Nodes count on abscissa with splits
        int totalNy = _meshParameters.OrdinateSplits.Sum() + 1;  // Nodes count on ordinate with splits

        var primaryNx = _meshParameters.AbscissaPointsCount;   // Primary abscissa points count
        var primaryNy = _meshParameters.OrdinatePointsCount;   // Primary ordinate points count
        
        int ordinateSplits = 0;
        int abscissaSplits;
        // Forming nodes on main horizontal lines
        
        for (int i = 0; i < primaryNy; i++)
        {
            abscissaSplits = 0;
            
            for (int j = 0; j < primaryNx - 1; j++)
            {
                var a = _meshParameters.ControlPoints[i * primaryNx + j];       // Start
                var b = _meshParameters.ControlPoints[i * primaryNx + j + 1];   // End
                int splits = _meshParameters.AbscissaSplits[j];
                double k = _meshParameters.AbscissaK[j] < 0
                    ? -1.0 / _meshParameters.AbscissaK[j]
                    : _meshParameters.AbscissaK[j];
                double h = (Math.Abs(k - 1.0) < 1E-14)
                    ? (b.X - a.X) / splits
                    : (b.X - a.X) * (1.0 - k) / (1.0 - Math.Pow(k, splits));

                _points[totalNx * ordinateSplits + abscissaSplits] = a;
                
                for (int l = 1; l < splits + 1; l++)
                {
                    double frac = l * h / (b.X - a.X);
                    double x = _points[totalNx * ordinateSplits + abscissaSplits + l - 1].X + h * Math.Pow(k, l - 1);
                    double y = a.Y + frac * (b.Y - a.Y);
                    
                    _points[totalNx * ordinateSplits + abscissaSplits + l] = new Point(x, y);
                }

                abscissaSplits += splits;
            }

            if (i < primaryNy - 1)
                ordinateSplits += _meshParameters.OrdinateSplits[i];
        }
        
        // Forming nodes on main vertical lines
        abscissaSplits = 0;
        
        for (int i = 0; i < primaryNx; i++)
        {
            ordinateSplits = 0;
            
            for (int j = 0; j < primaryNy - 1; j++)
            {
                var a = _meshParameters.ControlPoints[j * primaryNx + i];
                var b = _meshParameters.ControlPoints[j * primaryNx + i + primaryNx];
                int splits = _meshParameters.OrdinateSplits[j];
                double k = _meshParameters.OrdinateK[j] < 0
                    ? -1.0 / _meshParameters.OrdinateK[j]
                    : _meshParameters.OrdinateK[j];
                double h = (Math.Abs(k - 1.0) < 1E-14)
                    ? (b.Y - a.Y) / splits
                    : (b.Y - a.Y) * (1.0 - k) / (1.0 - Math.Pow(k, splits));

                _points[totalNx * ordinateSplits + abscissaSplits] = a;
                
                for (int l = 1; l < splits + 1; l++)
                {
                    double frac = l * h / (b.Y - a.Y);
                    double x = a.X + frac * (b.X - a.X);
                    double y = _points[totalNx * ordinateSplits + abscissaSplits + (l - 1) * totalNx].Y + h * Math.Pow(k, l - 1);
                    
                    _points[totalNx * ordinateSplits + abscissaSplits + l * totalNx] = new Point(x, y);
                }

                ordinateSplits += splits;
            }

            if (i < primaryNx - 1)
                abscissaSplits += _meshParameters.AbscissaSplits[i];
        }
        
        // Form inner nodes
        for (int i = 1; i < totalNy; i++)
        {
            abscissaSplits = 0;
            
            for (int j = 0; j < primaryNx - 1; j++)
            {
                int splits = _meshParameters.AbscissaSplits[j];
                double k = _meshParameters.AbscissaK[j] < 0
                    ? -1.0 / _meshParameters.AbscissaK[j]
                    : _meshParameters.AbscissaK[j];
        
                var a = _points[i * totalNx + abscissaSplits];
                var b = _points[i * totalNx + splits + abscissaSplits];
                
                double h = (Math.Abs(k - 1.0) < 1E-14)
                    ? (b.X - a.X) / splits
                    : (b.X - a.X) * (1.0 - k) / (1.0 - Math.Pow(k, splits));
        
                for (int l = 1; l < splits; l++)
                {
                    double frac = l * h / (b.X - a.X);
                    double x = _points[i * totalNx + abscissaSplits + l - 1].X + h * Math.Pow(k, l - 1);
                    double y = a.Y + frac * (b.Y - a.Y);
                    
                    _points[i * totalNx + abscissaSplits + l] = new Point(x, y);
                }
        
                abscissaSplits += splits;
            }
        }
    }

    public void CreateElements()
    {
        Span<int> nodes = stackalloc int[4];

        int nx = _meshParameters.AbscissaSplits.Sum() + 1;
        int ny = _meshParameters.OrdinateSplits.Sum() + 1;
        
        for (int i = 0, ielem = 0; i < ny - 1; i++)
        {
            for (int j = 0; j < nx - 1; j++)
            {
                nodes[0] = i * nx + j;
                nodes[1] = i * nx + 1 + j;
                nodes[2] = (i + 1) * nx + j;
                nodes[3] = (i + 1) * nx + 1 + j;

                _elements[ielem++] = new FiniteElement(nodes.ToArray());
            }
        }
    }

    public Mesh GetMesh()
    {
        return new Mesh(_points.ToArray(), _elements.ToArray());
    }
}