using UnityEngine;

namespace Grids
{
	public class SquareGrid: Grid
	{
		public override Vector2Int WorldToGrid(float x, float y, float z)
		{
			Vector2 halfGrid = GridSize / 2;
			return new Vector2Int(Mathf.RoundToInt(x / TileSize.x + halfGrid.x - WorldPosition.x) - 1,
								  Mathf.RoundToInt(y / TileSize.y + halfGrid.y - WorldPosition.y) - 1);
		}

		public override Vector3 GridToWorld(int x, int y)
		{
			Vector2 halfGrid = GridSize / 2;
			Vector3 tilePosition = new Vector3(x * TileSize.x - halfGrid.x, y * TileSize.y - halfGrid.y);
			return tilePosition + WorldPosition;
		}

		public override int GetDistance(Vector2Int a, Vector2Int b)
		{
			return GetDistance8Way(a, b);
		}
	}
}