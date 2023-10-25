using MeshBuilding.MeshContext.Interfaces;

namespace MeshBuilding.MeshContext;

public class MeshManager
{
    private readonly IMeshBuilder _meshBuilder;

    public MeshManager(IMeshBuilder meshBuilder)
        => _meshBuilder = meshBuilder;

    public MeshContext.Mesh CreateMesh()
    {
        _meshBuilder.CreatePoints();
        _meshBuilder.CreateElements();
        _meshBuilder.CreateBoundaries();
        return _meshBuilder.GetMesh();
    }
}