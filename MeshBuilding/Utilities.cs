using System.Globalization;

namespace MeshBuilding;

public static class Utilities
{
    #region Write

    public static void SaveMesh(Mesh.Mesh mesh, string folder)
    {
        // Points
        var sw = new StreamWriter($"{folder}/points");
        foreach (var p in mesh.Points)
        {
            sw.WriteLine($"{p.X} {p.Y}", CultureInfo.CurrentCulture);
        }
        sw.Close();

        // Elements
        sw = new StreamWriter($"{folder}/elements");
        for (var ielem = 0; ielem < mesh.Elements.Length; ielem++)
        {
            if (mesh.IsElementFictitious(ielem)) continue;
            
            var element = mesh.Elements[ielem];
            var nodes = element.Nodes;
            sw.WriteLine($"{nodes[0]} {nodes[1]} {nodes[3]} {nodes[2]}");
        }
        sw.Close();
        
        // Dirichlet
        sw = new StreamWriter($"{folder}/dirichlet");
        foreach (var dir in mesh.Dirichlet)
        {
            sw.WriteLine($"{dir.Node} {dir.Value}");
        }
        sw.Close();
        
        if (mesh.Neumann is null) return;
        
        // Neumann
        sw = new StreamWriter($"{folder}/neumann");
        foreach (var n in mesh.Neumann)
        {
            var p1 = mesh.Points[n.BorderStart];
            var p2 = mesh.Points[n.BorderEnd];
            
            sw.WriteLine($"{n.BorderStart} {n.BorderEnd} {n.Theta(p1.X, p1.Y)} {n.Theta(p2.X, p2.Y)}");
        }
        sw.Close();
    }

    #endregion
}