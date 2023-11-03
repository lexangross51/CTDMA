using System.Diagnostics.CodeAnalysis;
using MeshBuilding.FemContext.BasisInfo;
using MeshBuilding.MathHelper;
using MeshBuilding.MeshContext;

namespace MeshBuilding.FemContext.BoundariesHandler;

public class BoundaryHandler : BaseBoundaryHandler
{
    public BoundaryHandler(Mesh mesh, BasisInfoCollection basisInfo) 
        : base(mesh, basisInfo) {}
    
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

    public override void ApplyNeumann(IEnumerable<Neumann> neumann, SparseMatrix matrix, double[] vector)
    {
        
    }
}