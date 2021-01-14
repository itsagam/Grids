using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Kit;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Grids
{
	public abstract class BaseGrid : MonoBehaviour, IEnumerable<Tile>
	{
		public static readonly Vector2Int[] Neighbours8Way =
		{
			new Vector2Int(-1, -1),
			new Vector2Int(-1, 0),
			new Vector2Int(-1, +1),
			new Vector2Int(0,  -1),
			new Vector2Int(0,  +1),
			new Vector2Int(+1, -1),
			new Vector2Int(+1, 0),
			new Vector2Int(+1, +1)
		};

		public static readonly Vector2Int[] Neighbours4Way =
		{
			new Vector2Int(-1, 0),
			new Vector2Int(0,  -1),
			new Vector2Int(0,  +1),
			new Vector2Int(+1, 0)
		};

		public GameObject Prefab;
		public readonly Dictionary<Vector2Int, Tile> Tiles = new Dictionary<Vector2Int, Tile>();
		//public Tile[,] Tiles;

#if UNITY_EDITOR
		[ShowIf(nameof(ShouldShowGridSize))]
#endif

		public Vector2Int GridSize = new Vector2Int(10, 10);
		public Vector2 TileSize = new Vector2(1.0f,1.0f);
		public Axis Axis;

		protected abstract Vector2Int WorldToGridLocal(float x, float y);
		protected abstract Vector3 GridToWorldLocal(int x, int y);

		protected virtual void Start()
		{
			Generate();
		}

		protected virtual void Generate()
		{
			//Tiles = new Tile[GridSize.x, GridSize.y];

			for (int column=0; column < GridSize.x; column++)
				for (int row = 0; row < GridSize.y; row++)
				{
					Vector2Int gridPosition = new Vector2Int(column, row);
					Tiles.Add(gridPosition, GenerateTile(gridPosition));
					//Tiles[column, row] = ;
				}
		}

		protected virtual Tile GenerateTile(Vector2Int gridPosition)
		{
			return GenerateTile(gridPosition, Prefab);
		}

		protected virtual Tile GenerateTile(Vector2Int gridPosition, GameObject prefab)
		{
			GameObject tileGO = Instantiate(prefab, transform);
			Transform tileTransform = tileGO.transform;
			Tile tile = tileGO.AddComponent<Tile>();
			tile.name = $"{Prefab.name} {gridPosition}";
			tile.Grid = this;
			tile.GridPosition = gridPosition;
			tileTransform.position = tile.CalculatedWorldPosition;
			return tile;
		}

		public virtual Tile this [int x, int y] => Tiles[x, y];
		public virtual Tile this [Vector2Int vector2I] => this[vector2I.x, vector2I.y];
		public virtual Tile this [float x, float y] => this[Mathf.RoundToInt(x), Mathf.RoundToInt(y)];
		public virtual  Tile this [Vector2 vector2] => this[vector2.x, vector2.y];

		public virtual bool IsValid(int x, int y)
		{
			bool inRange = x >= 0 && y >= 0 && x < GridSize.x && y < GridSize.y;
			if (inRange)
				return this[x, y] != null;
			return false;
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

		public virtual Vector2Int WorldToGrid(float x, float y, float z)
		{
			Vector3 worldPosition = WorldPosition;
			switch (Axis)
			{
				case Axis.XZ:
				{
					y = z;
					worldPosition = worldPosition.SwapYZ();
					break;
				}

				case Axis.YZ:
				{
					x = z;
					worldPosition = worldPosition.SwapXZ();
					break;
				}
			}

			return WorldToGridLocal(x - worldPosition.x, y - worldPosition.y);
		}

		public virtual Vector3 GridToWorld(int x, int y)
		{
			Vector3 position = GridToWorldLocal(x, y);
			switch (Axis)
			{
				case Axis.XZ:
					position = position.SwapYZ();
					break;

				case Axis.YZ:
					position = position.SwapXZ();
					break;
			}
			return position + WorldPosition;
		}

		public virtual IEnumerable<Vector2Int> GetNeighbours(Vector2Int gridPosition)
		{
			return GetNeighbours(gridPosition.x, gridPosition.y);
		}

		public virtual IEnumerable<Vector2Int> GetNeighbours(int x, int y)
		{
			return GetNeighbours8Way(x, y);
		}

		public virtual IEnumerable<Vector2Int> GetNeighbours4Way(Vector2Int gridPosition)
		{
			return GetNeighbours4Way(gridPosition.x, gridPosition.y);
		}

		public virtual IEnumerable<Vector2Int> GetNeighbours4Way(int x, int y)
		{
			return GetPositions(x, y, Neighbours4Way);
		}

		public virtual IEnumerable<Vector2Int> GetNeighbours8Way(Vector2Int gridPosition)
		{
			return GetNeighbours8Way(gridPosition.x, gridPosition.y);
		}

		public virtual IEnumerable<Vector2Int> GetNeighbours8Way(int x, int y)
		{
			return GetPositions(x, y, Neighbours8Way);
		}

		public virtual Path<Vector2Int> GetPath(Vector2Int a, Vector2Int b)
		{
			return AStar.FindPath(a, b, GetDistance, p => GetDistance(b, p), GetNeighbours);
		}

		public virtual Path<Vector2Int> GetPath8Way(Vector2Int a, Vector2Int b)
		{
			return AStar.FindPath(a, b, GetDistance8Way, p => GetDistance8Way(b, p), GetNeighbours8Way);
		}

		public virtual Path<Vector2Int> GetPath4Way(Vector2Int a, Vector2Int b)
		{
			return AStar.FindPath(a, b, GetDistance4Way, p => GetDistance4Way(b, p), GetNeighbours4Way);
		}

		public static Vector2Int GetPosition(Vector2Int start, Direction direction)
		{
			return start + Neighbours8Way[(int)direction];
		}

		public static Vector2Int GetPosition(Vector2Int start, Direction direction, int numberOfTiles)
		{
			return start + Neighbours8Way[(int)direction] * numberOfTiles;
		}

		public virtual int GetDistance(Vector2Int a, Vector2Int b)
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

		public virtual IEnumerable<Vector2Int> GetPositionsInRange(Vector2Int center, Vector2Int range)
		{
			for (int x = center.x - range.x; x <= center.x + range.x; x++)
				for (int y = center.y - range.y; y <= center.y + range.y; y++)
				{
					Vector2Int position = new Vector2Int(x, y);
					if (IsValid(position))
						yield return position;
				}
		}

		public virtual IEnumerable<Vector2Int> GetPositionsInRange(Vector2Int center, int radius)
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
			return Neighbours8Way[(int)direction];
		}

		public Vector3 WorldPosition => transform.position;

		public IEnumerable<Vector2Int> GetPositions(int centerX, int centerY, IEnumerable<Vector2Int> offsets)
		{
			return GetPositions(new Vector2Int(centerX, centerY), offsets);
		}

		public IEnumerable<Vector2Int> GetPositions(Vector2Int center, IEnumerable<Vector2Int> offsets)
		{
			return offsets.Select(offset => center + offset).Where(IsValid);
		}

		public IEnumerable<Tile> GetTiles(IEnumerable<Vector2Int> positions)
		{
			return positions.Select(position => this[position]);
		}

		public IEnumerator<Tile> GetEnumerator()
		{
			return Tiles.Cast<Tile>().GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

#if  UNITY_EDITOR
		public virtual bool ShouldShowGridSize => true;
#endif
	}
}

