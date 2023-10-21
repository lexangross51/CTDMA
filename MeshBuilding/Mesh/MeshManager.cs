namespace MeshBuilding.Mesh;

public class MeshManager
{
    private readonly IMeshBuilder _meshBuilder;

    public MeshManager(IMeshBuilder meshBuilder)
        => _meshBuilder = meshBuilder;

    public Mesh CreateMesh()
    {
        _meshBuilder.CreatePoints();
        _meshBuilder.CreateElements();
        _meshBuilder.CreateBoundaries();
        return _meshBuilder.GetMesh();
    }
}