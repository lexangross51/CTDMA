using System.Numerics;

namespace MeshBuilding.MathHelper;

public class Matrix<T> where T : INumber<T>
{
    private readonly T[][] _storage;

    public int Rows { get; }
    public int Columns { get; }

    public Matrix(int nRows, int nColumns)
    {
        Rows = nRows;
        Columns = nColumns;
        _storage = new T[Rows].Select(_ => new T[nColumns]).ToArray();
    }

    public Matrix(T[,] data)
    {
        Rows = data.GetLength(0);
        Columns = data.GetLength(1);

        _storage = new T[Rows][];

        for (int i = 0; i < Rows; i++)
        {
            _storage[i] = new T[Columns];

            for (int j = 0; j < Columns; j++)
            {
                _storage[i][j] = data[i, j];
            }
        }
    }

    public T this[int i, int j]
    {
        get => _storage[i][j];
        set => _storage[i][j] = value;
    }

    public void Fill(T value)
    {
        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j < Columns; j++)
            {
                _storage[i][j] = value;
            }
        }
    }
}