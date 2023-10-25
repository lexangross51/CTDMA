namespace MeshBuilding.MeshContext.Interfaces;

public interface IMeshBuilder
{
    void CreatePoints();
    void CreateElements();
    void CreateBoundaries();
    Mesh GetMesh();
}