using MeshBuilding.FemContext.BasisInfo;
using MeshBuilding.FemContext.BasisInfo.Interfaces;

namespace MeshBuilding.Algorithms;

public static class Numerator
{
    public static BasisInfoCollection NumerateBasisFunctions(MeshContext.Mesh mesh, IBasis basis)
    {
        var basisInfo = new BasisInfoCollection(mesh.Elements.Length, basis.BasisSize);
        int nx = mesh.Elements[0].Nodes[2];

        for (var ielem = 0; ielem < mesh.Elements.Length; ielem++)
        {
            int k = 2 * ielem / (nx - 1) * (2 * nx - 1) + 2 * (ielem % (nx - 1));
            var nodes = mesh.Elements[ielem].Nodes;

            basisInfo[ielem, 0] = new BasisInfoItem(k, BasisFunctionType.ByGeometricNode, nodes[0]);
            basisInfo[ielem, 1] = new BasisInfoItem(k + 1, BasisFunctionType.ByEdgeNode, 0);
            basisInfo[ielem, 2] = new BasisInfoItem(k + 2, BasisFunctionType.ByGeometricNode, nodes[1]);
            basisInfo[ielem, 3] = new BasisInfoItem(k + 2 * nx - 1, BasisFunctionType.ByEdgeNode, 1);
            basisInfo[ielem, 4] = new BasisInfoItem(k + 2 * nx, BasisFunctionType.ByInnerNode, -1);
            basisInfo[ielem, 5] = new BasisInfoItem(k + 2 * nx + 1, BasisFunctionType.ByEdgeNode, 2);
            basisInfo[ielem, 6] = new BasisInfoItem(k + 4 * nx - 2, BasisFunctionType.ByGeometricNode, nodes[2]);
            basisInfo[ielem, 7] = new BasisInfoItem(k + 4 * nx - 1, BasisFunctionType.ByEdgeNode, 3);
            basisInfo[ielem, 8] = new BasisInfoItem(k + 4 * nx, BasisFunctionType.ByGeometricNode, nodes[3]);
        }

        return basisInfo;
    }
}