using System.Diagnostics.CodeAnalysis;
using MeshBuilding.FemContext.BasisInfo;
using MeshBuilding.MathHelper;
using MeshBuilding.MeshContext;

namespace MeshBuilding.FemContext.BoundariesHandler;

public class BoundaryHandler : BaseBoundaryHandler
{
    private readonly Matrix _localMassMatrix;
    private readonly double[] _thetaVector;
    private readonly double[] _localVector;

    public BoundaryHandler(Mesh mesh, BasisInfoCollection basisInfo)
        : base(mesh, basisInfo)
    {
        _localMassMatrix = new Matrix(3, 3)
        {
            [0, 0] = 4.0 / 30.0,
            [0, 1] = 2.0 / 30.0,
            [0, 2] = -1.0 / 30.0,
            [1, 0] = 2.0 / 30.0,
            [1, 1] = 16.0 / 30.0,
            [1, 2] = 2.0 / 30.0,
            [2, 0] = -1.0 / 30.0,
            [2, 1] = 2.0 / 30.0,
            [2, 2] = 4.0 / 30.0,
        };

        _thetaVector = new double[3];
        _localVector = new double[3];
    }
    
    [SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
    public override void ApplyDirichlet(IEnumerable<Dirichlet> dirichlet, SparseMatrix matrix, double[] vector)
    {
        var fCount = BasisInfo.FunctionsCount;
        var bc1 = new int[fCount];
        
        Array.Fill(bc1, -1);

        var dir = dirichlet.ToArray();
        
        for (int i = 0; i < dir.Length; i++)
        {
            bc1[dir[i].Node] = i;
        }

        for (int i = 0; i < fCount; i++)
        {
            int k;

            if (bc1[i] != -1)
            {
                var (node, value) = dir[bc1[i]];

                matrix.Di[i] = 1.0;
                
                FemHelper.TryGetPointForBasisFunction(Mesh, BasisInfo, node, out var x, out var y);

                vector[i] = value(x, y);

                for (int j = matrix.Ig[i]; j < matrix.Ig[i + 1]; j++)
                {
                    k = matrix.Jg[j];
                    if (bc1[k] == -1)
                    {
                        vector[k] -= matrix.Gg[j] * vector[i];
                    }
                    matrix.Gg[j] = 0.0;
                }
            }
            else
            {
                for (int j = matrix.Ig[i]; j < matrix.Ig[i + 1]; j++)
                {
                    k = matrix.Jg[j];
                    if (bc1[k] != -1)
                    {
                        vector[i] -= matrix.Gg[j] * vector[k];
                        matrix.Gg[j] = 0.0;
                    }
                }
            }
        }
    }

    public override void ApplyNeumann(IEnumerable<Neumann> neumann, double[] vector)
    {
        foreach (var n in neumann)
        {
            int node1 = n.Border.Node1;
            int node3 = n.Border.Node2;
            int node2 = (node1 + node3) / 2;
            
            FemHelper.TryGetPointForBasisFunction(Mesh, BasisInfo, node1, out var x0, out var y0);
            FemHelper.TryGetPointForBasisFunction(Mesh, BasisInfo, node2, out var x1, out var y1);
            FemHelper.TryGetPointForBasisFunction(Mesh, BasisInfo, node3, out var x2, out var y2);
            
            _thetaVector[0] = n.Theta(x0, y0);
            _thetaVector[1] = n.Theta(x1, y1);
            _thetaVector[2] = n.Theta(x2, y2);

            double len = Math.Sqrt((x2 - x0) * (x2 - x0) + (y2 - y0) * (y2 - y0));
            
            Matrix.Dot(_localMassMatrix, _thetaVector, _localVector);

            vector[node1] += len * _localVector[0];
            vector[node2] += len * _localVector[1];
            vector[node3] += len * _localVector[2];
        }
    }
}