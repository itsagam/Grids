using UnityEngine;

namespace Grids
{
	public class IsoGrid: Grid
	{
		//public const float TileRatio = 2.0f;
		public const float TileRatio = 1.732f;

		public override Vector2Int WorldToGridLocal(float x, float y, float z)
		{
			x = x / TileSize.x;
			y = y / TileSize.y;
			return new Vector2Int(Mathf.RoundToInt((TileRatio * y + x) / 2),
								  Mathf.RoundToInt((TileRatio * y - x) / 2));
		}

		public override Vector3 GridToWorldLocal(int x, int y)
		{
			float pointX = x * TileSize.x;
			float pointY = y * TileSize.y;
			return new Vector3(pointX - pointY,
							   ((pointX + pointY) / TileRatio) - (GridSize.y / 2) * TileSize.y);
		}

		public override int GetDistance(Vector2Int a, Vector2Int b)
		{
			return GetDistance8Way(a, b);
		}
	}
}