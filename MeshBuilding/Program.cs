using MeshBuilding;
using MeshBuilding.Algorithms;
using MeshBuilding.FemContext;
using MeshBuilding.FemContext.BasisInfo;
using MeshBuilding.Geometry;
using MeshBuilding.MeshContext;

var meshParameters = new MeshParameters
{
    AbscissaPointsCount = 6,
    OrdinatePointsCount = 3,
    ControlPoints = new Point[]
    {
        new(0, 0),
        new(2, 0),
        new(4, 0),
        new(5, 0),
        new(7, 0),
        new(9, 0),
        new(0, 4),
        new(2, 4),
        new(4, 3),
        new(5, 3),
        new(7, 4),
        new(9, 4),
        new(0, 6),
        new(2, 6),
        new(4, 6),
        new(5, 6),
        new(7, 6),
        new(9, 6)
    },
    Areas = new Area[]
    {
        new(0, 1, 0, 2, 0),
        new(1, 4, 0, 1, 0),
        new(4, 5, 0, 2, 0),
        new(1, 4, 1, 2, 0),
    },
    Borders = new Border[]
    {
        new(new[] { 0, 5 }, BoundaryType.Dirichlet, 1),
        new(new[] { 0, 12 }, BoundaryType.Dirichlet, 0),
        new(new[] { 5, 17 }, BoundaryType.Dirichlet, 0),
        new(new[] { 12, 17 }, BoundaryType.Dirichlet, 0),
        new(new[] { 13, 7, 8, 9, 10, 16 }, BoundaryType.Neumann, 1)
    },
    BoundaryFormulas = new Func<double, double, double>[]
    {
        (x, y) => x + y,
        (_, y) => y
    },
    AreaProperties = new AreaProperty[]
    {
        new(1.0, 0.0, (_, _) => 1.0 )
    },
    AbscissaSplits = new[] { 3, 2, 1, 2, 3 },
    AbscissaK = new[] { -1.3, 1.0, 1.0, 1.0, 1.3 },
    OrdinateSplits = new[] { 4, 2 },
    OrdinateK = new[] { -1.5, 1.5 },
    Refinement = 0
};

var meshManager = new MeshManager(new MeshBuilder(meshParameters));
var mesh = meshManager.CreateMesh();
Utilities.SaveMesh(mesh, @"C:\Users\lexan\source\repos\Python");

var femSolver = new FemSolver(mesh, new BiQuadraticBasis());
femSolver.Solve();