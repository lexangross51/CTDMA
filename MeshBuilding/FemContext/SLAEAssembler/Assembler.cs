using MeshBuilding.FemContext.BasisInfo;
using MeshBuilding.FemContext.BasisInfo.Interfaces;
using MeshBuilding.MathHelper;
using MeshBuilding.MathHelper.Integration;
using MeshBuilding.MeshContext;

namespace MeshBuilding.FemContext.SLAEAssembler;

public class Assembler : BaseAssembler
{
    private Integration _integrator;

    public Assembler(Mesh mesh, IBasis basis, BasisInfoCollection basisInfo)
        : base(mesh, basis, basisInfo)
    {
        _integrator = new Integration(Quadratures.GaussOrder3());
    }

    protected override void BuildLocalMatrix(int ielem)
    {
        throw new NotImplementedException();
    }

    protected override void BuildLocalVector(int ielem)
    {
        throw new NotImplementedException();
    }

    public override (SparseMatrix Matrix, double[] Vector) GetSlae()
    {
        throw new NotImplementedException();
    }
}