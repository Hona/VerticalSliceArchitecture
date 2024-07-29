using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace VerticalSliceArchitectureTemplate.Common.EfCore.Configuration;

public class TileArrayComparer()
    : ValueComparer<Tile[][]>(
        (c1, c2) => CompareArrays(c1, c2),
        c =>
            c.Aggregate(
                0,
                (a, v) =>
                    HashCode.Combine(
                        a,
                        v.Aggregate(0, (a2, v2) => HashCode.Combine(a2, v2.GetHashCode()))
                    )
            ),
        c => c.Select(row => row.ToArray()).ToArray()
    )
{
    private static bool CompareArrays(Tile[][]? array1, Tile[][]? array2)
    {
        if (ReferenceEquals(array1, array2))
        {
            return true;
        }

        if (array1 == null || array2 == null)
        {
            return false;
        }

        if (array1.Length != array2.Length)
        {
            return false;
        }

        return !array1.Where((t, i) => !t.SequenceEqual(array2[i])).Any();
    }
}
