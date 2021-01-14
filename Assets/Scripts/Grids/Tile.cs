using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Grids
{
	public class Tile : MonoBehaviour
	{
		[NonSerialized]
		public BaseGrid Grid;

		[NonSerialized]
		public Vector2Int GridPosition;

		public Vector3 WorldPosition => transform.position;
		public Vector3 CalculatedWorldPosition => Grid.GridToWorld(GridPosition);
		public bool IsValid => Grid.IsValid(GridPosition);

		public IEnumerable<Tile> Neighbours => Grid.GetNeighbours8Way(GridPosition).Select(p => Grid[p]);

		public int GetDistance(int x, int y)
		{
			return Grid.GetDistance(GridPosition, new Vector2Int(x, y));
		}

		public int GetDistance(Vector2Int gridPosition)
		{
			return Grid.GetDistance(GridPosition, gridPosition);
		}

		public int GetDistance(Tile tile)
		{
			return Grid.GetDistance(GridPosition, tile.GridPosition);
		}

		public virtual Path<Vector2Int> GetPath(int x, int y)
		{
			return GetPath(new Vector2Int(x, y));
		}

		public virtual Path<Vector2Int> GetPath(Tile to)
		{
			return GetPath(to.GridPosition);
		}

		public virtual Path<Vector2Int> GetPath(Vector2Int to)
		{
			return Grid.GetPath(GridPosition, to);
		}
	}
}