namespace MeshBuilding.Mesh;

public interface IMeshBuilder
{
    void CreatePoints();
    void CreateElements();
    Mesh GetMesh();
}