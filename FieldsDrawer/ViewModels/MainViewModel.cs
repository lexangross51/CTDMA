using System.Collections.Generic;
using System.Linq;
using FieldsDrawer.Core;
using FieldsDrawer.Core.Graphics.Objects;
using FieldsDrawer.Core.Graphics.Objects.Colormap;
using FieldsDrawer.Core.Graphics.Objects.Contour;
using FieldsDrawer.Core.Graphics.Objects.Mesh;
using FieldsDrawer.Core.Graphics.Palette;
using FieldsDrawer.MVVMTools;
using FieldsDrawer.MVVMTools.Services;
using MeshBuilding.FemContext;
using MeshBuilding.FemContext.BasisInfo;
using MeshBuilding.MeshContext;
using TriangleNet;
using Mesh = MeshBuilding.MeshContext.Mesh;

namespace FieldsDrawer.ViewModels;

public class MainViewModel : NotifyObject
{
    private readonly MeshManager _meshManager;
    private Mesh? _femMesh;
    private Core.Graphics.Objects.Mesh.Mesh? _mesh;
    private Contour? _contour;
    private FemSolver? _femSolver;

    private List<Point>? _points;
    private List<double>? _values;
    private double _xMin, _xMax, _yMin, _yMax;
    private int _nx, _ny;
    private bool _isAbleToDrawMesh;
    private int _meshRefinement;
    private int _isolinesCount;

    public int MeshRefinement
    {
        get => _meshRefinement;
        set => RaiseAndSetIfChanged(ref _meshRefinement, value);
    }

    public int IsolinesCount
    {
        get => _isolinesCount;
        set => RaiseAndSetIfChanged(ref _isolinesCount, value);
    } 
    
    public bool IsAbleToDrawMesh
    {
        get => _isAbleToDrawMesh;
        set => RaiseAndSetIfChanged(ref _isAbleToDrawMesh, value);
    }
    public RelayCommand BuildMeshCommand { get; } = null!;
    public RelayCommand RunFemSolverCommand { get; } = null!;
    public RelayCommand DrawFemMeshCommand { get; } = null!;
    public RelayCommand DrawIsolinesCommand { get; } = null!;
    public RelayCommand DrawTriMeshCommand { get; } = null!;

    public MainViewModel()
    {
        _meshManager = new MeshManager();
    }
    
    public MainViewModel(IUserDialogService userDialog) : this()
    {
        BuildMeshCommand = RelayCommand.Create(_ =>
        {
            var filename = userDialog.OpenSelectFileWindow();
            
            if (filename is null) return;

            var meshParameters = MeshParameters.ReadJson(filename);

            _nx = 2 * meshParameters.AbscissaSplits.Sum() - 1;
            _ny = 2 * meshParameters.OrdinateSplits.Sum() - 1;
            _xMin = meshParameters.ControlPoints.Min(p => p.X);
            _yMin = meshParameters.ControlPoints.Min(p => p.Y);
            _xMax = meshParameters.ControlPoints.Max(p => p.X);
            _yMax = meshParameters.ControlPoints.Max(p => p.Y);
            
            _meshManager.MeshBuilder = new MeshBuilder(meshParameters);
            _femMesh = _meshManager.CreateMesh();

            FromFemToDrawerMesh();

            IsAbleToDrawMesh = true;
            userDialog.SendObjectToView(_mesh!);
            
            GeneratePoints();
        });

        DrawFemMeshCommand = RelayCommand.Create(p =>
        {
            if (p is not bool toDraw) return;

            if (!toDraw)
            {
                userDialog.DeleteObjectFromView(_mesh!);
                IsAbleToDrawMesh = false;
            }
            else
            {
                userDialog.SendObjectToView(_mesh!);
                IsAbleToDrawMesh = true;
            }
        }, _ => _mesh is not null);
        
        RunFemSolverCommand = RelayCommand.Create(_ =>
        {
            _femSolver = new FemSolver(_femMesh!, new BiQuadraticBasis());
            _femSolver.Solve();
            
            CalculateValues();
            
            var delaunay = new TriangleNet.Meshing.Algorithm.Dwyer();
            var triMesh = delaunay.Triangulate(MathHelper.ToTriangleNetVertices(_points!), new Configuration());

            IsolinesCount = 10;
            _contour = new Contour(triMesh, _values!, IsolinesCount);
            var colorMap = new ColorMap(_contour.Mesh, _values!, Palette.Rainbow);
            
            userDialog.SendObjectToView(colorMap);

        }, _ => _femMesh is not null);

        DrawIsolinesCommand = RelayCommand.Create(p =>
        {
            if (p is not bool toDraw) return;

            if (!toDraw)
            {
                userDialog.DeleteObjectFromView(_contour!);
            }
            else
            {
                userDialog.SendObjectToView(_contour!);
            }
        });

        DrawTriMeshCommand = RelayCommand.Create(p =>
        {
            if (p is not bool toDraw) return;
            
            if (!toDraw)
            {
                userDialog.DeleteObjectFromView(_contour!.Mesh);
            }
            else
            {
                userDialog.SendObjectToView(_contour!.Mesh);
            }
        });
    }

    private void GeneratePoints()
    {
        _points ??= new List<Point>();
        _points.Clear();
        
        double hx = (_xMax - _xMin) / (_nx - 1);
        double hy = (_yMax - _yMin) / (_ny - 1);

        for (int i = 0; i < _ny; i++)
        {
            for (int j = 0; j < _nx; j++)
            {
                _points.Add(new Point(_xMin + j * hx, _yMin + i * hy));
            }
        }
    }

    private void CalculateValues()
    {
        if (_femSolver is null) return;
        if (_points is null) return;
        
        _values ??= new List<double>();
        _values.Clear();
        
        foreach (var p in _points)
        {
            var value = _femSolver.ValueAtPoint(p.X, p.Y);
            _values.Add(value);
        }
    }
    
    private void FromFemToDrawerMesh()
    {
        if (_femMesh is null) return;
        
        var points = new Point[_femMesh.Points.Length];

        for (int i = 0; i < points.Length; i++)
        {
            points[i] = new Point(_femMesh.Points[i].X, _femMesh.Points[i].Y);
        }

        var elements = new Element[_femMesh.Elements.Length];

        for (int i = 0; i < elements.Length; i++)
        {
            elements[i] = new Element(_femMesh.Elements[i].Nodes.ToArray());
        }

        _mesh = new Core.Graphics.Objects.Mesh.Mesh(points, elements);
    }
}