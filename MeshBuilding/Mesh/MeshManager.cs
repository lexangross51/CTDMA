namespace MeshBuilding.Mesh;

public class MeshManager
{
    public IMeshBuilder MeshBuilder { get; set; }

    public MeshManager(IMeshBuilder meshBuilder)
    {
        MeshBuilder = meshBuilder;
    }

    public Mesh CreateMesh()
    {
        MeshBuilder.CreatePoints();
        MeshBuilder.CreateElements();
        return MeshBuilder.GetMesh();
    }
}