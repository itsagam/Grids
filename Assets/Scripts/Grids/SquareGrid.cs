using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Grids
{
	public class SquareGrid: Grid
	{
		public static Vector2Int[] Neighbours =
		{
			new Vector2Int(-1, -1),
			new Vector2Int(-1, 0),
			new Vector2Int(-1, +1),
			new Vector2Int(0,  -1),
			new Vector2Int(0,  +1),
			new Vector2Int(+1, -1),
			new Vector2Int(+1, 0),
			new Vector2Int(+1, +1),
		};

		public override Vector2Int WorldToGrid(float x, float y, float z)
		{
			return new Vector2Int(Mathf.RoundToInt(x), Mathf.RoundToInt(y));
		}

		public override Vector3 GridToWorld(int x, int y)
		{
			return new Vector3(x, y);
		}

		public override int GetDistance(Vector2Int a, Vector2Int b)
		{
			return GetDistance8Way(a, b);
		}

		public static int GetDistance4Way(Vector2Int a, Vector2Int b)
		{
			return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
		}

		public static int GetDistance8Way(Vector2Int a, Vector2Int b)
		{
			return Mathf.Max(Mathf.Abs(a.x - b.x), Mathf.Abs(a.y - b.y));
		}

		public override IEnumerable<Vector2Int> GetNeighbours(int x, int y)
		{
			return Neighbours.Select(offset => new Vector2Int(x, y) + offset).Where(IsValid);
		}
	}
}