using FieldsDrawer.Core.Graphics.Shaders.Wrappers;

namespace FieldsDrawer.Core.Graphics.Shaders;

public static class ShaderCollection
{
    public static ShaderProgram LineShader() 
        => new("Core//Graphics//Shaders//LineShader.vert", "Core//Graphics//Shaders//LineShader.frag");

    public static ShaderProgram TextShader()
        => new("Core//Graphics//Shaders//TextShader.vert", "Core//Graphics//Shaders//TextShader.frag");

    public static ShaderProgram FieldShader()
        => new("Core//Graphics//Shaders//FieldShader.vert", "Core//Graphics//Shaders//FieldShader.frag");
}