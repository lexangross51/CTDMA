using MeshBuilding.Geometry;

namespace MeshBuilding.MeshContext.Interfaces;

public interface IMeshBuilder
{
    IEnumerable<Point> Points { get; set; }
    void CreatePoints();
    void CreateElements();
    void CreateBoundaries();
    Mesh GetMesh();
}