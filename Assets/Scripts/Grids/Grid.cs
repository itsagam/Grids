using System.Collections.Generic;
using UnityEngine;

namespace Grids
{
	public abstract class Grid
	{
		public Tile[,] Tiles;
		public Vector2Int Size;

		public abstract Vector2Int WorldToGrid(float x, float y, float z);
		public abstract Vector3 GridToWorld(int x, int y);
		public abstract int GetDistance(Vector2Int a, Vector2Int b);
		public abstract IEnumerable<Vector2Int> GetNeighbours(int x, int y);

		protected virtual void Initialize()
		{
			Tiles = new Tile[Size.x, Size.y];
		}

		public Tile this [int x, int y] => Tiles[x, y];
		public Tile this [Vector2Int vector2I] => this[vector2I.x, vector2I.y];
		public Tile this [float x, float y] => this[Mathf.RoundToInt(x), Mathf.RoundToInt(y)];
		public Tile this [Vector2 vector2] => this[vector2.x, vector2.y];

		public bool IsValid(int x, int y)
		{
			return x >= 0 && y >= 0 && x < Size.x && y < Size.y;
		}

		public bool IsValid(Vector2Int vector2I)
		{
			return IsValid(vector2I.x, vector2I.y);
		}

		public Vector2Int WorldToGrid(Vector3 worldPosition)
		{
			return WorldToGrid(worldPosition.x, worldPosition.y, worldPosition.z);
		}

		public Vector3 GridToWorld(Vector2Int gridPosition)
		{
			return GridToWorld(gridPosition.x, gridPosition.y);
		}

		public IEnumerable<Vector2Int> GetNeighbours(Vector2Int vector2I)
		{
			return GetNeighbours(vector2I.x, vector2I.y);
		}
	}
}

