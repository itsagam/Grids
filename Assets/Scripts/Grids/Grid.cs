// Hex
// Iso

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Grids
{
	public abstract class Grid : IEnumerable<Tile>
	{
		public Tile[,] Tiles;
		public Vector2Int GridSize = new Vector2Int(10, 10);
		public Vector2 TileSize = new Vector2(1.0f,1.0f);

		public abstract Vector2Int WorldToGrid(float x, float y, float z);
		public abstract Vector3 GridToWorld(int x, int y);
		public abstract int GetDistance(Vector2Int a, Vector2Int b);
		public abstract IEnumerable<Vector2Int> GetNeighbours(int x, int y);

		protected virtual void Generate()
		{
			Tiles = new Tile[GridSize.x, GridSize.y];

			for (int column=0; column < GridSize.x; column++)
				for (int row = 0; row < GridSize.y; row++)
				{
					Tile tile = new Tile
								{
									Grid = this,
									GridPosition = new Vector2Int(column, row)
								};

					Tiles[column, row] = tile;
				}
		}

		public Tile this [int x, int y] => Tiles[x, y];
		public Tile this [Vector2Int vector2I] => this[vector2I.x, vector2I.y];
		public Tile this [float x, float y] => this[Mathf.RoundToInt(x), Mathf.RoundToInt(y)];
		public Tile this [Vector2 vector2] => this[vector2.x, vector2.y];

		public virtual bool IsValid(int x, int y)
		{
			return x >= 0 && y >= 0 && x < GridSize.x && y < GridSize.y;
		}

		public virtual bool IsValid(Vector2Int vector2I)
		{
			return IsValid(vector2I.x, vector2I.y);
		}

		public virtual Vector2Int WorldToGrid(Vector3 worldPosition)
		{
			return WorldToGrid(worldPosition.x, worldPosition.y, worldPosition.z);
		}

		public virtual Vector3 GridToWorld(Vector2Int gridPosition)
		{
			return GridToWorld(gridPosition.x, gridPosition.y);
		}

		public virtual IEnumerable<Vector2Int> GetNeighbours(Vector2Int gridPosition)
		{
			return GetNeighbours(gridPosition.x, gridPosition.y);
		}

		public virtual Path<Vector2Int> GetPath(Vector2Int a, Vector2Int b)
		{
			return AStar.FindPath(a, b, GetDistance, p => GetDistance(b, p), GetNeighbours);
		}

		public IEnumerator<Tile> GetEnumerator()
		{
			return Tiles.Cast<Tile>().GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}

