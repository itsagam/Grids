using UnityEngine;

namespace Grids
{
	public class IsoGrid: Grid
	{
		public const float TileRatio = 1.732f;

		public override Vector2Int WorldToGrid(float x, float y, float z)
		{
			return new Vector2Int
				   {
					   x = Mathf.RoundToInt(x - y),
					   y = Mathf.RoundToInt((x + y) / TileRatio)
				   };
		}

		public override Vector3 GridToWorld(int x, int y)
		{
			return new Vector2
				   {
					   x = (TileRatio * y + x) / 2,
					   y = (TileRatio * y - x) / 2
				   };
		}

		public override int GetDistance(Vector2Int a, Vector2Int b)
		{
			return GetDistance8Way(a, b);
		}
	}
}