using MeshBuilding;
using MeshBuilding.Geometry;
using MeshBuilding.Mesh;

// var meshParameters = new MeshParameters
// {
//     AbscissaPointsCount = 6,
//     OrdinatePointsCount = 3,
//     ControlPoints = new Point[]
//     {
//         new(0, 0),
//         new(2, 0),
//         new(4, 0),
//         new(5, 0),
//         new(7, 0),
//         new(9, 0),
//         new(0, 4),
//         new(2, 4),
//         new(4, 3),
//         new(5, 3),
//         new(7, 4),
//         new(9, 4),
//         new(0, 6),
//         new(2, 6),
//         new(4, 6),
//         new(5, 6),
//         new(7, 6),
//         new(9, 6)
//     },
//     Areas = new Area[]
//     {
//         new(0, 0, 1, 0, 2),
//         new(0, 1, 2, 0, 1),
//         new(0, 2, 3, 0, 2),
//         new(0, 1, 2, 1, 2),
//     },
//     AbscissaSplits = new[] { 3, 2, 1, 2, 3 },
//     AbscissaK = new[] { 1.0, 1.0, 1.0, 1.0, 1.0 },
//     OrdinateSplits = new[] { 4, 2 },
//     OrdinateK = new[] { 1.0, 1.0 }
// };

var meshParameters = new MeshParameters
{
    AbscissaPointsCount = 5,
    OrdinatePointsCount = 5,
    ControlPoints = new Point[]
    {
        new(0.0, 0.0),
        new(2.0, 0.0),
        new(4.0, 0.0),
        new(6.0, 0.0),
        new(8.0, 0.0),
        new(0.0, 5.27),
        new(2.44, 4.94),
        new(3.5, 4.5),
        new(4.56, 4.94),
        new(8.0, 5.27),
        new(0.0, 6.0),
        new(2.0, 6.0),
        new(3.5, 6.0),
        new(5.0, 6.0),
        new(8.0, 6.0),
        new(0.0, 6.73),
        new(2.44, 7.06),
        new(3.5, 7.5),
        new(4.56, 7.06),
        new(8.0, 6.73),
        new(0.0, 11.0),
        new(2.0, 11.0),
        new(4.0, 11.0),
        new(6.0, 11.0),
        new(8.0, 11.0),
    },
    Areas = new Area[]
    {
        new(0, 0, 1, 0, 2),
        new(0, 1, 3, 0, 1),
        new(0, 3, 4, 0, 2),
        new(0, 0, 1, 2, 4),
        new(0, 1, 3, 3, 4),
        new(0, 3, 4, 2, 4)
    },
    AbscissaSplits = new[] { 2, 1, 1, 3 },
    AbscissaK = new[] { 1.0, 1.0, 1.0, 1.0 },
    OrdinateSplits = new[] { 3, 1, 1, 3 },
    OrdinateK = new[] { -1.5, 1.0, 1.0, 1.5 }
};

var meshManager = new MeshManager(new MeshBuilder(meshParameters));
var mesh = meshManager.CreateMesh();
Utilities.SaveMesh(mesh, @"C:\\Users\\lexan\\source\\repos\\Python");