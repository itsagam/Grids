using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Grids
{
	public class SquareGrid: Grid
	{
		public static readonly Vector2Int[] Neighbours =
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
			return new Vector2Int(Mathf.RoundToInt(x / TileSize.x), Mathf.RoundToInt(y / TileSize.y));
		}

		public override Vector3 GridToWorld(int x, int y)
		{
			return new Vector3(x * TileSize.x, y * TileSize.y);
		}

		public override int GetDistance(Vector2Int a, Vector2Int b)
		{
			return GetDistance8Way(a, b);
		}

		public override IEnumerable<Vector2Int> GetNeighbours(int x, int y)
		{
			return Neighbours.Select(offset => new Vector2Int(x, y) + offset).Where(IsValid);
		}

		public static int GetDistance4Way(Vector2Int a, Vector2Int b)
		{
			return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
		}

		public static int GetDistance8Way(Vector2Int a, Vector2Int b)
		{
			return Mathf.Max(Mathf.Abs(a.x - b.x), Mathf.Abs(a.y - b.y));
		}

		public IEnumerable<Vector2Int> GetPositionsInRange(Vector2Int center, Vector2Int range)
		{
			for (int x = center.x - range.x; x <= center.x + range.x; x++)
				for (int y = center.y - range.y; y <= center.y + range.y; y++)
				{
					Vector2Int position = new Vector2Int(x, y);
					if (IsValid(position))
						yield return position;
				}
		}

		public IEnumerable<Vector2Int> GetPositionsInRange(Vector2Int center, int radius)
		{
			var squareRange = GetPositionsInRange(center, new Vector2Int(radius, radius));
			int radiusSqr = radius * radius;
			foreach (Vector2Int position in squareRange)
			{
				Vector2Int difference = position - center;
				if ((difference.x * difference.x) + (difference.y * difference.y) <= radiusSqr)
					yield return position;
			}
		}

		public static Vector2Int GetPosition(Vector2Int start, Direction direction)
		{
			return start + Neighbours[(int)direction];
		}

		public static Vector2Int GetPosition(Vector2Int start, Direction direction, int numberOfTiles)
		{
			return start + Neighbours[(int)direction] * numberOfTiles;
		}

		public virtual Direction GetDirection(Vector2Int displacement)
		{
			if (displacement.x == 0)
			{
				if (displacement.y > 1)
					return Direction.North;

				if (displacement.y < 1)
					return Direction.South;
			}

			if (displacement.x > 1)
			{
				if (displacement.y > 1)
					return Direction.NorthEast;

				if (displacement.y == 0)
					return Direction.East;

				if (displacement.y < 1)
					return Direction.SouthEast;
			}

			if (displacement.x < 1)
			{
				if (displacement.y > 1)
					return Direction.NorthWest;

				if (displacement.y == 0)
					return Direction.West;

				if (displacement.y < 1)
					return Direction.SouthWest;
			}

			return Direction.None;
		}

		public static Vector2Int GetOffset(Direction direction)
		{
			return Neighbours[(int)direction];
		}
	}
}