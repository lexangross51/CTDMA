using MeshBuilding.FemContext.BasisInfo;
using MeshBuilding.FemContext.BasisInfo.Interfaces;
using MeshBuilding.Geometry;
using MeshBuilding.MathHelper;
using MeshBuilding.MathHelper.Integration;
using MeshBuilding.MeshContext;

namespace MeshBuilding.FemContext.SLAEAssembler;

public class Assembler : BaseAssembler
{
    private readonly Integration _integrator;
    private readonly Matrix _stiffnessMatrix;
    private readonly Matrix _massMatrix;
    private readonly double[] _localVector;
    private readonly Point[] _elementPoints = new Point[4];
    private readonly Vector _gradPhiI = new(2);
    private readonly Vector _gradPhiJ = new(2);
    private readonly Vector _matrixGradI = new(2);
    private readonly Vector _matrixGradJ = new(2);
    private readonly Rectangle _masterElement = new(new Point(), new Point(1, 1));

    public Assembler(Mesh mesh, IBasis basis, BasisInfoCollection basisInfo)
        : base(mesh, basis, basisInfo)
    {
        _integrator = new Integration(Quadratures.GaussOrder3());

        _stiffnessMatrix = new Matrix(basis.BasisSize, basis.BasisSize);
        _massMatrix = new Matrix(basis.BasisSize, basis.BasisSize);
        _localVector = new double[basis.BasisSize];
    }

    protected override void AssembleLocalSlae(int ielem)
    {
        var nodes = Mesh.Elements[ielem].Nodes;
        _elementPoints[0] = Mesh.Points[nodes[0]];
        _elementPoints[1] = Mesh.Points[nodes[1]];
        _elementPoints[2] = Mesh.Points[nodes[2]];
        _elementPoints[3] = Mesh.Points[nodes[3]];
            
        for (int i = 0; i < Basis.BasisSize; i++)
        {
            int i1 = i;
            
            for (int j = 0; j <= i; j++)
            {
                int j1 = j;

                double ScalarFunc(double ksi, double eta)
                {
                    var jacobiMatrix = FemHelper.CalculateJacobiMatrix(Mesh, ielem, Basis, BasisInfo, ksi, eta);
                    double jacobian = FemHelper.Jacobian(jacobiMatrix);
                    FemHelper.InvertJacobiMatrix(jacobiMatrix);

                    _gradPhiI[0] = Basis.DPhi(i1, 0, ksi, eta);
                    _gradPhiI[1] = Basis.DPhi(i1, 1, ksi, eta);
                    _gradPhiJ[0] = Basis.DPhi(j1, 0, ksi, eta);
                    _gradPhiJ[1] = Basis.DPhi(j1, 1, ksi, eta);

                    MathHelper.Matrix.Dot(jacobiMatrix, _gradPhiI, _matrixGradI);
                    MathHelper.Matrix.Dot(jacobiMatrix, _gradPhiJ, _matrixGradJ);

                    return (_matrixGradI[0] * _matrixGradJ[0] + _matrixGradI[1] * _matrixGradJ[1]) * Math.Abs(jacobian);
                }


                _stiffnessMatrix[i, j] = _stiffnessMatrix[j, i] = _integrator.Integrate2D(ScalarFunc, _masterElement);
            }
        }
        
        for (int i = 0; i < Basis.BasisSize; i++)
        {
            int i1 = i;

            for (int j = 0; j <= i; j++)
            {
                int j1 = j;

                double ScalarFunc(double ksi, double eta)
                {
                    var jacobiMatrix = FemHelper.CalculateJacobiMatrix(Mesh, ielem, Basis, BasisInfo, ksi, eta);
                    double jacobian = FemHelper.Jacobian(jacobiMatrix);

                    return Basis.Phi(i1, ksi ,eta) * Basis.Phi(j1, ksi, eta) * Math.Abs(jacobian);
                }

                _massMatrix[i, j] = _massMatrix[j, i] = _integrator.Integrate2D(ScalarFunc, _masterElement);
            }
        }

        var source = Mesh.Materials[Mesh.Elements[ielem].AreaNumber].Source;
        for (int i = 0; i < Basis.BasisSize; i++)
        {
            _localVector[i] = 0.0;
                
            for (int j = 0; j < Basis.BasisSize; j++)
            {
                // _localVector[i] += _massMatrix[i, j] * source(Mesh.Points[nodes[j]]);
            }
        }
    }

    public override (SparseMatrix Matrix, double[] Vector) GetSlae()
    {
        Matrix.Clear();
        Array.Fill(Vector, 0.0);

        for (int ielem = 0; ielem < Mesh.Elements.Length; ielem++)
        {
            double lambda = Mesh.Materials[Mesh.Elements[ielem].AreaNumber].Lambda;
            double gamma = Mesh.Materials[Mesh.Elements[ielem].AreaNumber].Gamma;
            
            AssembleLocalSlae(ielem);

            for (int i = 0; i < Basis.BasisSize; i++)
            {
                int globalI = BasisInfo[ielem, i].FunctionNumber;
                    
                for (int j = 0; j < Basis.BasisSize; j++)
                {
                    int globalJ = BasisInfo[ielem, j].FunctionNumber;
                    
                    Matrix.Add(globalI, globalJ,lambda * _stiffnessMatrix[i, j] + gamma * _massMatrix[i, j]);
                }
            }
        }

        return (Matrix, Vector);
    }
}