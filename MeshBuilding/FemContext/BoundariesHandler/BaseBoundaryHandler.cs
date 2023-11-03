using MeshBuilding.FemContext.BasisInfo;
using MeshBuilding.MathHelper;
using MeshBuilding.MeshContext;

namespace MeshBuilding.FemContext.BoundariesHandler;

public abstract class BaseBoundaryHandler
{
    protected readonly Mesh Mesh;
    protected readonly BasisInfoCollection BasisInfo;

    protected BaseBoundaryHandler(Mesh mesh, BasisInfoCollection basisInfo)
    {
        Mesh = mesh;
        BasisInfo = basisInfo;
    }

    public abstract void ApplyDirichlet(IEnumerable<Dirichlet> dirichlet, SparseMatrix matrix, double[] vector);
    public abstract void ApplyNeumann(IEnumerable<Neumann> neumann, SparseMatrix matrix, double[] vector);
}