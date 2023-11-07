using MeshBuilding;
using MeshBuilding.FemContext;
using MeshBuilding.FemContext.BasisInfo;
using MeshBuilding.MeshContext;

var meshParameters = MeshParameters.ReadJson("Input/Area.json");
var meshManager = new MeshManager(new MeshBuilder(meshParameters));
var mesh = meshManager.CreateMesh();
// Utilities.SaveMesh(mesh, @"C:\Users\lexan\source\repos\Python");

var femSolver = new FemSolver(mesh, new BiQuadraticBasis());
femSolver.Solve();

var xMin = meshParameters.ControlPoints.Min(p => p.X) + 0.5;
var yMin = meshParameters.ControlPoints.Min(p => p.Y) + 0.5;
var xMax = meshParameters.ControlPoints.Max(p => p.X) - 0.5;
var yMax = meshParameters.ControlPoints.Max(p => p.Y) - 0.5;
var points = Utilities.GeneratePoints(xMin, xMax, yMin, yMax, 4, 3);

double residual = femSolver.RootMeanSquare(points);
Console.WriteLine($"Residual: {residual}");
