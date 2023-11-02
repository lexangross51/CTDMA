using MeshBuilding.FemContext.BasisInfo;
using MeshBuilding.FemContext.BasisInfo.Interfaces;
using MeshBuilding.MeshContext;

namespace MeshBuilding.MathHelper;

public static class FemHelper
{
    private static readonly double[] Dx = new double[2];
    private static readonly double[] Dy = new double[2];
    
    public static Matrix CalculateJacobiMatrix(Mesh mesh, int ielem, IBasis basis, BasisInfoCollection basisInfo,
        double x, double y)
    {
        Array.Fill(Dx, 0.0);
        Array.Fill(Dy, 0.0);

        var element = mesh.Elements[ielem];

        for (int i = 0; i < basis.BasisSize; i++)
        {
            var bf = basisInfo[ielem, i];

            switch (bf.Type)
            {
                case BasisFunctionType.ByGeometricNode:
                    Dx[0] += mesh.Points[bf.Index].X * basis.DPhi(i, 0, x, y);
                    Dx[1] += mesh.Points[bf.Index].X * basis.DPhi(i, 1, x, y);
                    Dy[0] += mesh.Points[bf.Index].Y * basis.DPhi(i, 0, x, y);
                    Dy[1] += mesh.Points[bf.Index].Y * basis.DPhi(i, 1, x, y);
                    break;
                
                case BasisFunctionType.ByInnerNode:
                    var nodes = element.Nodes;
                    double xCenter = 0.0, yCenter = 0.0;

                    foreach (var node in nodes)
                    {
                        xCenter += mesh.Points[node].X;
                        yCenter += mesh.Points[node].Y;
                    }

                    xCenter /= nodes.Count;
                    yCenter /= nodes.Count;
                    
                    Dx[0] += xCenter * basis.DPhi(i, 0, x, y);
                    Dx[1] += xCenter * basis.DPhi(i, 1, x, y);
                    Dy[0] += yCenter * basis.DPhi(i, 0, x, y);
                    Dy[1] += yCenter * basis.DPhi(i, 1, x, y);
                    
                    break;
                
                case BasisFunctionType.ByEdgeNode:
                    var edge = element.Edges[bf.Index];
                    double edgeCenterX = (mesh.Points[edge.Node1].X + mesh.Points[edge.Node2].X) / 2.0; 
                    double edgeCenterY = (mesh.Points[edge.Node1].Y + mesh.Points[edge.Node2].Y) / 2.0; 
                    
                    Dx[0] += edgeCenterX * basis.DPhi(i, 0, x, y);
                    Dx[1] += edgeCenterX * basis.DPhi(i, 1, x, y);
                    Dy[0] += edgeCenterY * basis.DPhi(i, 0, x, y);
                    Dy[1] += edgeCenterY * basis.DPhi(i, 1, x, y);
                    
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException(nameof(i), i,
                        $"Function with number {i} doesn't match interval [0, {basis.BasisSize - 1}]");
            }
        }

        return new Matrix(2, 2)
        {
            [0, 0] = Dx[0],
            [0, 1] = Dy[0],
            [1, 0] = Dx[1],
            [1, 1] = Dy[1]
        };
    }

    public static double Jacobian(Matrix jacobiMatrix)
        => jacobiMatrix[0, 0] * jacobiMatrix[1, 1] - jacobiMatrix[0, 1] * jacobiMatrix[1, 0];
    
    public static void InvertJacobiMatrix(Matrix jacobiMatrix)
    {
        double jacobian = Jacobian(jacobiMatrix);

        (jacobiMatrix[0, 0], jacobiMatrix[1, 1]) = (jacobiMatrix[1, 1], jacobiMatrix[0, 0]);
        jacobiMatrix[0, 0] /= jacobian;
        jacobiMatrix[1, 1] /= jacobian;
        jacobiMatrix[0, 1] = -jacobiMatrix[0, 1] / jacobian;
        jacobiMatrix[1, 0] = -jacobiMatrix[1, 0] / jacobian;
    }
}