using MeshBuilding.Algorithms;
using MeshBuilding.FemContext.BasisInfo;
using MeshBuilding.FemContext.BasisInfo.Interfaces;
using MeshBuilding.MathHelper;
using MeshBuilding.MeshContext;

namespace MeshBuilding.FemContext.SLAEAssembler;

public abstract class BaseAssembler
{
    protected readonly SparseMatrix Matrix;
    protected readonly double[] Vector;
    protected readonly Mesh Mesh;
    protected readonly IBasis Basis;
    protected readonly BasisInfoCollection BasisInfo;

    protected BaseAssembler(Mesh mesh, IBasis basis, BasisInfoCollection basisInfo)
    {
        Mesh = mesh;
        Basis = basis;
        BasisInfo = basisInfo;

        var portrait = PortraitBuilder.GeneratePortrait(basisInfo);
        
        Matrix = new SparseMatrix(portrait.Ig, portrait.Jg);
        Vector = new double[portrait.Ig.Length - 1];
    }

    protected abstract void BuildLocalMatrix(int ielem);
    protected abstract void BuildLocalVector(int ielem);

    public abstract (SparseMatrix Matrix, double[] Vector) GetSlae();
}