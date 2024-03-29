﻿using System.Globalization;
using MeshBuilding.FemContext.BasisInfo;
using MeshBuilding.MathHelper;
using MeshBuilding.MeshContext;
using System.Linq.Expressions;
using MeshBuilding.FemContext.BasisInfo.Interfaces;
using MeshBuilding.Geometry;

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

    public static IEnumerable<Point> GeneratePoints(double xMin, double xMax, double yMin, double yMax, int nx, int ny)
    {
        var points = new List<Point>(nx * ny);

        double hx = (xMax - xMin) / (nx - 1);
        double hy = (yMax - yMin) / (ny - 1);
        
        for (int i = 0; i < ny; i++)
        {
            for (int j = 0; j < nx; j++)
            {
                points.Add(new Point(xMin + j * hx, yMin + i * hy));
            }
        }
        
        return points;
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

public class Newton
{
    private readonly BasisInfoCollection _basisInfo;
    private readonly Vector _vector = new(2);
    private readonly Vector _result;
    private readonly Vector _slaeResult;
    private Matrix _jacobiMatrix;
    private readonly IBasis _basis;
    private readonly Mesh _mesh;

    public int NumberElement { get; set; }
    public Point Point { get; set; }
    public Point Result => new(_result[0], _result[1]);

    public Newton(Mesh mesh, IBasis basis, BasisInfoCollection basisInfo)
    {
        _mesh = mesh;
        _basis = basis;
        _basisInfo = basisInfo;
        
        _jacobiMatrix = new Matrix(2, 2);
        _slaeResult = new Vector(2);
        _result = new Vector(2)
        {
            [0] = 0.5,
            [1] = 0.5
        };
    }

    public void Compute()
    {
        const int maxIterations = 1000;
        const double eps = 1E-12;

        CalculateEquationsValues();

        var primaryNorm = _vector.Norm() + 1E-30;
        var currentNorm = primaryNorm;

        for (int iter = 0; iter < maxIterations && currentNorm / primaryNorm >= eps; iter++)
        {
            var previousNorm = _vector.Norm();

            var beta = 1.0;

            CalculateJacobiMatrix();

            for (int i = 0; i < _vector.Length; i++)
            {
                _vector[i] = -_vector[i];
            }

            GaussMethod();

            var temp = new Vector(_vector.Length);
            Vector.Copy(_result, temp);

            do
            {
                for (int i = 0; i < _slaeResult.Length; i++)
                {
                    _slaeResult[i] *= beta;
                }

                for (int i = 0; i < _slaeResult.Length; i++)
                {
                    _result[i] = temp[i] + _slaeResult[i];
                }

                CalculateEquationsValues();
                currentNorm = _vector.Norm();

                if (currentNorm > previousNorm) beta /= 2.0;
                else break;
            } while (beta > eps);
        }
    }

    private void CalculateEquationsValues()
    {
        _vector.Fill();

        var p = (_result[0], _result[1]);

        for (int i = 0; i < _basis.BasisSize; i++)
        {
            var bf = _basisInfo[NumberElement, i];
            
            FemHelper.TryGetPointForBasisFunction(_mesh, NumberElement, bf, out var x, out var y);

            _vector[0] += _basis.Phi(i, p.Item1, p.Item2) * x;
            _vector[1] += _basis.Phi(i, p.Item1, p.Item2) * y;
        }

        _vector[0] -= Point.X;
        _vector[1] -= Point.Y;
    }

    private void CalculateJacobiMatrix()
    {
        _jacobiMatrix = FemHelper.CalculateJacobiMatrix(_mesh, NumberElement, _basis, _basisInfo, _result[0], _result[1]);
    }

    private void GaussMethod()
    {
        const double eps = 1E-14;

        for (int k = 0; k < 2; k++)
        {
            var max = Math.Abs(_jacobiMatrix[k, k]);
            int index = k;

            for (int i = k + 1; i < 2; i++)
            {
                if (Math.Abs(_jacobiMatrix[i, k]) > max)
                {
                    max = Math.Abs(_jacobiMatrix[i, k]);
                    index = i;
                }
            }

            for (int j = 0; j < 2; j++)
            {
                (_jacobiMatrix[k, j], _jacobiMatrix[index, j]) =
                    (_jacobiMatrix[index, j], _jacobiMatrix[k, j]);
            }

            (_vector[k], _vector[index]) = (_vector[index], _vector[k]);

            for (int i = k; i < 2; i++)
            {
                var temp = _jacobiMatrix[i, k];

                if (Math.Abs(temp) < eps) continue;

                for (int j = 0; j < 2; j++)
                {
                    _jacobiMatrix[i, j] /= temp;
                }

                _vector[i] /= temp;

                if (i == k) continue;
                {
                    for (int j = 0; j < 2; j++)
                        _jacobiMatrix[i, j] -= _jacobiMatrix[k, j];

                    _vector[i] -= _vector[k];
                }
            }
        }

        for (int k = 2 - 1; k >= 0; k--)
        {
            _slaeResult[k] = _vector[k];

            for (int i = 0; i < k; i++)
            {
                _vector[i] -= _jacobiMatrix[i, k] * _slaeResult[k];
            }
        }
    }
}