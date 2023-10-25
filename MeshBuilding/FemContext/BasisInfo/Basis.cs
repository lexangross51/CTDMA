using MeshBuilding.FemContext.BasisInfo.Interfaces;

namespace MeshBuilding.FemContext.BasisInfo;

public struct BiQuadraticBasis : IBasis
{
    public int BasisSize => 9;
    
    public double Phi(int function, double x, double y)
    {
        return function switch
        {
            0 => 1.0,
            1 => 1.0,
            2 => 1.0,
            3 => 1.0,
            4 => 1.0,
            5 => 1.0,
            6 => 1.0,
            7 => 1.0,
            8 => 1.0,
            9 => 1.0,
            _ => throw new ArgumentOutOfRangeException(nameof(function), function, 
                $"Function {function} doesn't match interval [0, {BasisSize - 1}]")
        };
    }

    public double DPhi(int function, int variable, double x, double y)
    {
        throw new NotImplementedException();
    }
}