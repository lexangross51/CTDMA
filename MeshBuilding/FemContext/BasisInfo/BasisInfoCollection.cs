using System.Collections;

namespace MeshBuilding.FemContext.BasisInfo;

public struct BasisInfoItem
{
    public int FunctionNumber { get; }
    public BasisFunctionType Type { get; }
    public int Index { get; }

    public BasisInfoItem(int functionNumber, BasisFunctionType type, int index)
    {
        FunctionNumber = functionNumber;
        Type = type;
        Index = index;
    }
}

public class BasisInfoCollection : IEnumerable<KeyValuePair<int, BasisInfoItem[]>>
{
    private readonly Dictionary<int, BasisInfoItem[]> _basisByElements;

    public BasisInfoCollection(int elementCount, int functionsPerElement)
    {
        _basisByElements = new Dictionary<int, BasisInfoItem[]>(elementCount * functionsPerElement);

        for (int i = 0; i < elementCount; i++)
        {
            _basisByElements.Add(i, new BasisInfoItem[functionsPerElement]);
        }
    }

    public BasisInfoItem this[int elementIndex, int functionIndex]
    {
        get => _basisByElements[elementIndex][functionIndex];
        set => _basisByElements[elementIndex][functionIndex] = value;
    }

    public IEnumerator<KeyValuePair<int, BasisInfoItem[]>> GetEnumerator()
        => _basisByElements.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}