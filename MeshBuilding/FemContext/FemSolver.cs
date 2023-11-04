﻿using MeshBuilding.Algorithms;
using MeshBuilding.FemContext.BasisInfo;
using MeshBuilding.FemContext.BasisInfo.Interfaces;
using MeshBuilding.FemContext.BoundariesHandler;
using MeshBuilding.FemContext.SLAEAssembler;
using MeshBuilding.Geometry;
using MeshBuilding.MathHelper;
using MeshBuilding.MeshContext;

namespace MeshBuilding.FemContext;

public class FemSolver
{
    private readonly BaseAssembler _slaeAssembler;
    private readonly BaseBoundaryHandler _boundaryHandler;
    private readonly IterativeSolver _solver;
    private readonly Mesh _mesh;
    private readonly BasisInfoCollection _basisInfo;
    private List<Dirichlet> _dirichlet;
    private List<Neumann>? _neumann;
    
    public FemSolver(Mesh mesh, IBasis basis)
    {
        _basisInfo = Numerator.NumerateBasisFunctions(mesh, basis);
        _mesh = mesh;
        _slaeAssembler = new Assembler(mesh, basis, _basisInfo);
        _boundaryHandler = new BoundaryHandler(mesh, _basisInfo);
        _solver = new CGMCholesky(10_000, 1E-20);

        _dirichlet = new List<Dirichlet>();
        
        RenumerateDirichletNodes();
        RenumerateNeumannNodes();

        Utilities.SaveBiQuadDirichlet(mesh, _basisInfo, _dirichlet, @"C:\Users\lexan\source\repos\Python");
    }

    private void RenumerateDirichletNodes()
    {
        // Edges on the outer border (only dirichlet edges)
        var dEdges = _mesh.Elements
            .SelectMany(elem => elem.Edges)
            .GroupBy(e => e)
            .Where(g => g.Count() == 1)
            .SelectMany(g => g)
            .Where(e => _mesh.Dirichlet.Any(d => d.Node == e.Node1 || d.Node == e.Node2));

        var edgesPerElems = new Dictionary<int, List<Edge>>();
        
        // Find elements for edges
        foreach (var edge in dEdges)
        {
            int ielem = Array.FindIndex(_mesh.Elements, el => el.Edges.Contains(edge));
            
            if (!edgesPerElems.ContainsKey(ielem))
                edgesPerElems.Add(ielem, new List<Edge>());
            
            edgesPerElems[ielem].Add(edge);
        }
        
        foreach (var pair in edgesPerElems)
        {
            int ielem = pair.Key;
            var element = _mesh.Elements[ielem];
            var edges = pair.Value;

            foreach (var edge in edges)
            {
                int iedge = element.Edges.IndexOf(edge);

                var bf = _basisInfo.GetFunctionAtNode(ielem, edge.Node1);
                var index = Array.FindIndex(_mesh.Dirichlet, d => d.Node == edge.Node1);
                _dirichlet.Add(new Dirichlet(bf.FunctionNumber, _mesh.Dirichlet[index].Value));

                bf = _basisInfo.GetFunctionAtNode(ielem, edge.Node2);
                index = Array.FindIndex(_mesh.Dirichlet, d => d.Node == edge.Node2);
                _dirichlet.Add(new Dirichlet(bf.FunctionNumber, _mesh.Dirichlet[index].Value));
                
                bf = _basisInfo.GetFunctionAtEdge(ielem, iedge);
                _dirichlet.Add(new Dirichlet(bf.FunctionNumber, _mesh.Dirichlet[index].Value));
            }
        }

        _dirichlet = _dirichlet.DistinctBy(d => d.Node).ToList();
    }

    private void RenumerateNeumannNodes()
    {
        if (_mesh.Neumann is null) return;

        _neumann = new List<Neumann>();
    }
    
    public double Solve()
    {
        var slae = _slaeAssembler.GetSlae();

        ApplyBoundaries(slae.Matrix, slae.Vector);
        
        _solver.SetSystem(slae.Matrix, slae.Vector);
        _solver.Compute();

        return RootMeanSquare();
    }

    private void ApplyBoundaries(SparseMatrix matrix, double[] vector)
    {
        if (_neumann is not null) 
            _boundaryHandler.ApplyNeumann(_neumann, matrix, vector);
        
        _boundaryHandler.ApplyDirichlet(_dirichlet, matrix, vector);
    }

    private double RootMeanSquare()
    {
        var func = _dirichlet[0].Value;
        int count = _basisInfo.FunctionsCount;
        double dif = 0.0;

        for (int i = 0; i < count; i++)
        {
            FemHelper.TryGetPointForBasisFunction(_mesh, _basisInfo, i, out var x, out var y);
            double value = func(x, y);

            dif += (value - _solver.Solution![i]) * (value - _solver.Solution![i]);
        }

        return Math.Sqrt(dif / count);
    }
}