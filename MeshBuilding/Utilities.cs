using System.Globalization;
using MeshBuilding.FemContext.BasisInfo;
using MeshBuilding.MathHelper;
using MeshBuilding.MeshContext;
using System.Linq.Expressions;

namespace MeshBuilding;

public static class Utilities
{
    public static Func<double, double, double> GetFunctionFromString(string expression)
    {
        var x = Expression.Parameter(typeof(double), "x");
        var y = Expression.Parameter(typeof(double), "y");
        var body = System.Linq.Dynamic.Core.DynamicExpressionParser.ParseLambda(new[] { x, y }, null, expression).Body;

        return Expression.Lambda<Func<double, double, double>>(body, x, y).Compile();
    }
    
    public static void SaveMesh(Mesh mesh, string folder)
    {
        // Points
        var sw = new StreamWriter($"{folder}/points");
        foreach (var p in mesh.Points)
        {
            sw.WriteLine($"{p.X} {p.Y}", CultureInfo.CurrentCulture);
        }
        sw.Close();

        // Elements
        sw = new StreamWriter($"{folder}/elements");
        for (var ielem = 0; ielem < mesh.Elements.Length; ielem++)
        {
            if (mesh.IsElementFictitious(ielem)) continue;
            
            var element = mesh.Elements[ielem];
            var nodes = element.Nodes;
            sw.WriteLine($"{nodes[0]} {nodes[1]} {nodes[3]} {nodes[2]}");
        }
        sw.Close();
        
        // Dirichlet
        sw = new StreamWriter($"{folder}/dirichlet");
        foreach (var dir in mesh.Dirichlet)
        {
            sw.WriteLine($"{dir.Node} {dir.Value}");
        }
        sw.Close();
        
        if (mesh.Neumann is null) return;
        
        // Neumann
        sw = new StreamWriter($"{folder}/neumann");
        foreach (var n in mesh.Neumann)
        {
            var p1 = mesh.Points[n.Border.Node1];
            var p2 = mesh.Points[n.Border.Node2];
            
            sw.WriteLine($"{n.Border.Node1} {n.Border.Node2} {n.Theta(p1.X, p1.Y)} {n.Theta(p2.X, p2.Y)}");
        }
        sw.Close();
    }

    public static void SaveBasisInfo(Mesh mesh, BasisInfoCollection basisInfo, string folder)
    {
        var functions = basisInfo
            .SelectMany(e => e.Value
                .Select(f => f.FunctionNumber))
            .Distinct()
            .OrderBy(f => f)
            .ToList();
        
        using var sw = new StreamWriter($"{folder}/basisInfo");
        
        foreach (var bf in functions)
        {
            FemHelper.TryGetPointForBasisFunction(mesh, basisInfo, bf, out var x, out var y);
            sw.WriteLine($"{x} {y}");
        }
        
        sw.Close();
    }
    
    public static void SaveBiQuadDirichlet(Mesh mesh, BasisInfoCollection basisInfo, IEnumerable<Dirichlet> dirichlet, string folder)
    {
        using var sw = new StreamWriter($"{folder}/dirichletBiQuad");
        
        foreach (var d in dirichlet)
        {
            FemHelper.TryGetPointForBasisFunction(mesh, basisInfo, d.Node, out var x, out var y);
            sw.WriteLine($"{x} {y}");
        }
        
        sw.Close();
    }
    
    public static void SaveBiQuadNeumann(Mesh mesh, BasisInfoCollection basisInfo, IEnumerable<Neumann> neumann, string folder)
    {
        using var sw = new StreamWriter($"{folder}/neumannBiQuad");
        
        foreach (var n in neumann)
        {
            FemHelper.TryGetPointForBasisFunction(mesh, basisInfo, n.Border.Node1, out var x0, out var y0);
            FemHelper.TryGetPointForBasisFunction(mesh, basisInfo, n.Border.Node2, out var x1, out var y1);
            sw.WriteLine($"{x0} {y0} {x1} {y1}");
        }
        
        sw.Close();
    }
}

public static class EnumerableExtensions
{
    public static double Norm(this IEnumerable<double> collection)
        => Math.Sqrt(collection.Sum(item => item * item));
}