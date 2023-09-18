using System.Globalization;

namespace MeshBuilding;

public static class Utilities
{
    public static void SaveMesh(Mesh.Mesh mesh, string folder)
    {
        var sw = new StreamWriter($"{folder}/points");
        foreach (var p in mesh.Points)
        {
            sw.WriteLine($"{p.X} {p.Y}", CultureInfo.CurrentCulture);
        }
        sw.Close();

        sw = new StreamWriter($"{folder}/elements");
        foreach (var element in mesh.Elements)
        {
            var nodes = element.Nodes;
            sw.WriteLine($"{nodes[0]} {nodes[1]} {nodes[2]} {nodes[3]}");
        }
        sw.Close();
    }
}