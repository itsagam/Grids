using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Grids
{
	public class HexGrid: BaseGrid
	{
		// public static readonly Vector2Int[] Neighbours =
		// {
		// 	new Vector2Int(0, 1),
		// 	new Vector2Int(0, -1),
		// 	new Vector2Int(1, 0),
		// 	new Vector2Int(-1, 0),
		// 	new Vector2Int(1, -1),
		// 	new Vector2Int(-1, -1),
		// };


		public static readonly Vector2Int[] Neighbours =
		{
			new Vector2Int(1, 0),
			new Vector2Int(1, -1),
			new Vector2Int(0, -1),
			new Vector2Int(-1, 0),
			new Vector2Int(-1, 1),
			new Vector2Int(0, 1)
		};

		public HexShape Shape = HexShape.Rectangle;

#if  UNITY_EDITOR
		[ShowIf(nameof(ShouldShowGridRadius))]
#endif
		[LabelText("Radius")]
		public int GridRadius = 5;

		[ShowIf("Shape", HexShape.Rectangle)]
		public HexLayout Layout = HexLayout.Even;

		public HexOrientation Orientation = HexOrientation.Flat;

		protected override void Generate()
		{
			switch (Shape)
			{
				case HexShape.Hex:
					MakeHexMap(GridRadius - 1);
					break;

				case HexShape.Rectangle:
					MakeRectMap(GridSize.x, GridSize.y);
					break;
			}
		}

		public void MakeHexMap(int size)
		{
			int size2X = size * 2;
			int size3X = size * 3;
			GridSize = new Vector2Int(size2X + 1, size2X + 1);
			Tiles = new Tile[GridSize.x, GridSize.y];

			for (int x = 0; x <= size2X; x++)
				for (int y = 0; y <= size2X; y++)
				{
					int sum = x + y;
					if (sum >= size && sum <= size3X)
						Tiles[x, y] = GenerateTile(new Vector2Int(x, y));
				}
		}

		public void MakeRectMap(int width, int height)
		{
			Tiles = new Tile[width, height];
			for (int x = 0; x < width; x++)
				for (int y = 0; y < height; y++)
					Tiles[x, y] = GenerateTile(new Vector2Int(x, y));
		}

		protected override Vector2Int WorldToGridLocal(float x, float y)
		{
			throw new NotImplementedException();
		}

		protected override Vector3 GridToWorldLocal(int x, int y)
		{
			if (Shape == HexShape.Rectangle)
			{
				Vector2Int transformed = OffsetToAxial(x, y);
				x = transformed.x;
				y = transformed.y;
			}

			Vector3 position = Vector3.zero;
			int extraFactorX = 1, extraFactorY = 1;

			Vector2Int size = GridSize;
			if (Shape == HexShape.Hex)
			{
				extraFactorX = 3;
				extraFactorY = 2;
				size = new Vector2Int(GridRadius, GridRadius);
			}

			switch (Orientation)
			{
				case HexOrientation.Pointed:
					position.x = x + y / 2.0f;
					position.y = y * 0.75f;

					// Center
					position.x -= (size.x - 1) * extraFactorX / 2.0f;
					position.y -= (size.y - 1) * extraFactorY / 2.0f * 0.75f;
					break;

				case HexOrientation.Flat:
					position.x = x * 0.75f;
					position.y = y + x / 2.0f;

					// Center
					position.x -= (size.x - 1) * extraFactorY / 2.0f * 0.75f;
					position.y -= (size.y - 1) * extraFactorX / 2.0f;
					break;
			}

			return position * TileSize;
		}

		public static Vector2Int Round(float x, float y)
		{
			float z = -x - y;
			int rx = Mathf.RoundToInt(x);
			int ry = Mathf.RoundToInt(y);
			int rz = Mathf.RoundToInt(z);
			float xDiff = Mathf.Abs(rx - x);
			float yDiff = Mathf.Abs(ry - y);
			float zDiff = Mathf.Abs(rz - z);
			if (xDiff > yDiff && xDiff > zDiff)
				rx = -ry - rz;
			else if (yDiff > zDiff)
				ry = -rx - rz;
			//else
			//	rz = -rx - ry;
			return new Vector2Int(rx, ry);
		}

		public override int GetDistance(Vector2Int a, Vector2Int b)
		{
			return (Mathf.Abs(a.x - b.x) +
					Mathf.Abs(a.y - b.y) +
					Mathf.Abs(a.x + a.y - b.x - b.y)) / 2;
		}

		public override IEnumerable<Vector2Int> GetNeighbours(int x, int y)
		{
			return GetPositions(x, y, Neighbours);
		}

		public override IEnumerable<Vector2Int> GetPositionsInRange(Vector2Int center, int radius)
		{
			for (int x = -radius; x <= radius; x++)
			{
				int start = Mathf.Max(-radius, -x - radius);
				int end = Mathf.Min(radius, -x + radius);
				for (int y = start; y <= end; y++)
				{
					Vector2Int position = new Vector2Int(x, y);
					position += center;
					if (IsValid(position))
						yield return position;
				}
			}
		}

		public IEnumerable<Vector2Int> GetLine(Vector2Int a, Vector2Int b)
		{
			int distance = GetDistance(a, b);
			for (int i = 0; i <= distance; i++)
			{
				float ratio = (float) i / distance;
				float inverseRatio = 1 - ratio;
				Vector2 aVector = a;
				Vector2 bVector = b;
				Vector2 position = aVector * ratio + bVector * inverseRatio;
				Vector2Int rounded = Round(position.x, position.y);
				if (IsValid(rounded))
					yield return rounded;
			}
		}

		public IEnumerable<Vector2Int> GetSuperline(Vector2Int a, Vector2Int b)
		{
			float cutoff = Mathf.Tan(30 * Mathf.Deg2Rad);
			foreach (Vector2Int position in GetLine(a, b))
			{
				yield return position;

				foreach (Vector2Int neighbour in GetNeighbours(position))
				{
					int distance = GetDistance(position, neighbour);
					if (distance <= cutoff)
					{
						yield return neighbour;
						break;
					}
				}
			}
		}

		public IEnumerable<Vector2Int> GetRing(Vector2Int center, int radius)
		{
			radius--;
			Vector2Int current = center + Neighbours[4] * radius;
			foreach (Vector2Int offset in Neighbours)
				for (int j = 0; j < radius; j++)
				{
					if (IsValid(current))
						yield return current;
					current += offset;
				}
		}

		public Vector2Int OffsetToAxial(int x, int y)
		{
			switch (Layout)
			{
				case HexLayout.Odd:
					switch (Orientation)
					{
						case HexOrientation.Pointed:
							return OddRToAxial(x, y);

						case HexOrientation.Flat:
							return OddQToAxial(x, y);
					}

					break;

				case HexLayout.Even:
					switch (Orientation)
					{
						case HexOrientation.Pointed:
							return EvenRToAxial(x, y);

						case HexOrientation.Flat:
							return EvenQToAxial(x, y);
					}

					break;
			}

			return default;
		}

		public static Vector2Int OddQToAxial(int x, int y)
		{
			return new Vector2Int(x, y - ((x - (x & 1)) >> 1));
		}

		public static Vector2Int OddRToAxial(int x, int y)
		{
			return new Vector2Int(x - ((y + (y & 1)) >> 1), y);
		}

		public static Vector2Int EvenQToAxial(int x, int y)
		{
			return new Vector2Int(x, y - ((x + (x & 1)) >> 1));
		}

		public static Vector2Int EvenRToAxial(int x, int y)
		{
			return new Vector2Int(x - ((y - (y & 1)) >> 1), y);
		}

		public Vector2Int AxialToOffset(int x, int y)
		{
			switch (Layout)
			{
				case HexLayout.Odd:
					switch (Orientation)
					{
						case HexOrientation.Pointed:
							return AxialToOddR(x, y);

						case HexOrientation.Flat:
							return AxialToOddQ(x, y);
					}

					break;
				case HexLayout.Even:
					switch (Orientation)
					{
						case HexOrientation.Pointed:
							return AxialToEvenR(x, y);

						case HexOrientation.Flat:
							return AxialToEvenQ(x, y);
					}

					break;
			}

			return default;
		}

		public static Vector2Int AxialToOddQ(int x, int y)
		{
			return new Vector2Int(x, y + (x - (x & 1)) / 2);
		}

		public static Vector2Int AxialToOddR(int x, int y)
		{
			return new Vector2Int(x + (y - (y & 1)) / 2, y);
		}

		public static Vector2Int AxialToEvenQ(int x, int y)
		{
			return new Vector2Int(x, y + (x + (x & 1)) / 2);
		}

		public static Vector2Int AxialToEvenR(int x, int y)
		{
			return new Vector2Int(x + (y + (y & 1)) / 2, y);
		}

		public override Tile this[float x, float y]
		{
			get
			{
				Vector2Int rounded = Round(x, y);
				return IsValid(rounded) ? this[rounded] : null;
			}
		}

#if UNITY_EDITOR
		public override bool ShouldShowGridSize => Shape == HexShape.Rectangle;
		public virtual bool ShouldShowGridRadius => Shape == HexShape.Hex;
#endif
	}
}