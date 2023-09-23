namespace MeshBuilding.Mesh;

public interface IMeshBuilder
{
    void CreatePoints();
    void CreateElements();
    void CreateBoundaries();
    Mesh GetMesh();
}