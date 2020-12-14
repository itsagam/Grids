using UnityEngine;

namespace Grids
{
	public class SquareGrid: Grid
	{
		public override Vector2Int WorldToGridLocal(float x, float y, float z)
		{
			return new Vector2Int(Mathf.RoundToInt(x / TileSize.x + GridSize.x / 2.0f),
								  Mathf.RoundToInt(y / TileSize.y + GridSize.y / 2.0f));
		}

		public override Vector3 GridToWorldLocal(int x, int y)
		{
			return new Vector3((x - GridSize.x / 2.0f) * TileSize.x , (y - GridSize.y / 2.0f) * TileSize.y);
		}

		public override int GetDistance(Vector2Int a, Vector2Int b)
		{
			return GetDistance8Way(a, b);
		}
	}
}