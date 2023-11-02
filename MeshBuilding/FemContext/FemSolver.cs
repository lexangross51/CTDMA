using MeshBuilding.Algorithms;
using MeshBuilding.FemContext.BasisInfo.Interfaces;
using MeshBuilding.FemContext.SLAEAssembler;
using MeshBuilding.MathHelper;
using MeshBuilding.MeshContext;

namespace MeshBuilding.FemContext;

public class FemSolver
{
    private readonly BaseAssembler _slaeAssembler;
    private readonly IterativeSolver _solver;
    private readonly Mesh _mesh;

    public FemSolver(Mesh mesh, IBasis basis)
    {
        var basisInfo = Numerator.NumerateBasisFunctions(mesh, basis);

        _mesh = mesh;
        _slaeAssembler = new Assembler(mesh, basis, basisInfo);
        _solver = new CGMCholesky(10_000, 1E-20);
    }

    public void Solve()
    {
        var slae = _slaeAssembler.GetSlae();
        
        ApplyNeumann();
        ApplyDirichlet();
        
        _solver.SetSystem(slae.Matrix, slae.Vector);
        _solver.Compute();
    }

    private void ApplyDirichlet()
    {
        throw new NotImplementedException();
    }
    
    private void ApplyNeumann()
    {
        if (_mesh.Neumann is null || _mesh.Neumann.Length == 0) return;

        foreach (var n in _mesh.Neumann)
        {
            
        }
    }

    // private void ApplyDirichlet()
    // {
    //     var bc1 = new int[_mesh.Points.Length].Select(_ => -1).ToArray();
    //     var dirichlet = _mesh.DirichletConditions;
    //
    //     for (int i = 0; i < dirichlet.Length; i++)
    //     {
    //         bc1[dirichlet[i].Node] = i;
    //     }
    //
    //     for (int i = 0; i < _mesh.Points.Length; i++)
    //     {
    //         int k;
    //
    //         if (bc1[i] != -1)
    //         {
    //             var (node, value) = dirichlet[bc1[i]];
    //
    //             _globalMatrix.Di[i] = 1.0;
    //             _globalVector[i] = _field?.Invoke(_mesh.Points[node]) ?? value;
    //
    //             for (int j = _globalMatrix.Ig[i]; j < _globalMatrix.Ig[i + 1]; j++)
    //             {
    //                 k = _globalMatrix.Jg[j];
    //                 if (bc1[k] == -1)
    //                 {
    //                     _globalVector[k] -= _globalMatrix.GGl[j] * _globalVector[i];
    //                 }
    //
    //                 _globalMatrix.GGl[j] = 0.0;
    //                 _globalMatrix.GGu[j] = 0.0;
    //             }
    //         }
    //         else
    //         {
    //             for (int j = _globalMatrix.Ig[i]; j < _globalMatrix.Ig[i + 1]; j++)
    //             {
    //                 k = _globalMatrix.Jg[j];
    //                 if (bc1[k] != -1)
    //                 {
    //                     _globalVector[i] -= _globalMatrix.GGl[j] * _globalVector[k];
    //                     _globalMatrix.GGl[j] = 0.0;
    //                     _globalMatrix.GGu[j] = 0.0;
    //                 }
    //             }
    //         }
    //     }
    // }
    //
    // private void ApplyNeumann()
    // {
    //     foreach (var (ielem, iedge, theta) in _mesh.NeumannConditions)
    //     {
    //         var edge = _mesh.Elements[ielem].Edges[iedge];
    //         var edgeLenght = _mesh.Points[edge.Node2].X - _mesh.Points[edge.Node1].X +
    //             _mesh.Points[edge.Node2].Y - _mesh.Points[edge.Node1].Y;
    //
    //         _globalVector[edge.Node1] += edgeLenght * theta / 2.0;
    //         _globalVector[edge.Node2] += edgeLenght * theta / 2.0;
    //     }
    // }
}