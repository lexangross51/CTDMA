using MeshBuilding.FemContext.BasisInfo;
using MeshBuilding.FemContext.BasisInfo.Interfaces;
using MeshBuilding.Geometry;

namespace MeshBuilding.MathHelper;

public static class FemHelper
{
    public static Matrix CalculateJacobiMatrix(Point[] elementPoints, IBasis basis, BasisInfoCollection basisInfo,
        double x, double y)
    {
        throw new NotImplementedException();
    }

    public static double Jacobian(Matrix jacobiMatrix)
    {
        throw new NotImplementedException();
    }
    
    public static void InvertJacobiMatrix(Matrix jacobiMatrix)
    {
        throw new NotImplementedException();
    }
}