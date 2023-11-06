using MeshBuilding;
using MeshBuilding.FemContext;
using MeshBuilding.FemContext.BasisInfo;
using MeshBuilding.MeshContext;

var meshParameters = MeshParameters.ReadJson("Input/Area.json");
var meshManager = new MeshManager(new MeshBuilder(meshParameters));
var mesh = meshManager.CreateMesh();
Utilities.SaveMesh(mesh, @"C:\Users\lexan\source\repos\Python");

var femSolver = new FemSolver(mesh, new BiQuadraticBasis());
var residual = femSolver.Solve();

Console.WriteLine($"Residual: {residual}");