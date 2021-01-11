using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace Grids
{
	public class HexGrid: Grid
	{
		public HexShape Shape = HexShape.Rectangle;

		[ShowIf("Shape", HexShape.Rectangle)]
		public HexLayout Layout = HexLayout.Even;

		public HexOrientation Orientation = HexOrientation.Flat;

		protected override void Generate()
		{
			Tiles = new Tile[GridSize.x, GridSize.y];
			switch (Shape)
			{
				case HexShape.Hex:
					int size;
					if (GridSize.x >= GridSize.y)
						size = GridSize.y = GridSize.x;
					else
						size = GridSize.x = GridSize.y;
					MakeHexMap(size - 1);
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
			for (int x = 0; x <= size2X; x++)
				for (int y = 0; y <= size2X; y++)
				{
					int sum = x + y;
					if (sum >= size && sum <= size3X)
						GenerateTile(new Vector2Int(x, y));
				}
		}

		public void MakeRectMap(int width, int height)
		{
			for (int x = 0; x < width; x++)
				for (int y = 0; y < height; y++)
					Tiles[x, y] = GenerateTile(new Vector2Int(x, y));
		}

		public override Vector2Int WorldToGridLocal(float x, float y, float z)
		{
			return Vector2Int.zero;
			//throw new System.NotImplementedException();
		}

		public override Vector3 GridToWorldLocal(int x, int y)
		{
			if (Shape == HexShape.Rectangle)
			{
				Vector2Int transformed = OffsetToAxial(x, y);
				x = transformed.x;
				y = transformed.y;
			}

			Vector3 position = Vector3.zero;
			int extraFactorX = 1, extraFactorY = 1;
			if (Shape == HexShape.Hex)
			{
				extraFactorX = 3;
				extraFactorY = 2;
			}

			switch (Orientation)
			{
				case HexOrientation.Pointed:
					position.x = (x + y / 2.0f);
					position.y = y * 0.75f;

					// Center
					position.x -= (GridSize.x - 1) * extraFactorX / 2.0f;
					position.y -= (GridSize.y - 1) * extraFactorY / 2.0f * 0.75f;
					break;

				case HexOrientation.Flat:
					position.x = x * 0.75f;
					position.y = (y + x / 2.0f);

					// Center
					position.x -= (GridSize.x - 1) * extraFactorY / 2.0f * 0.75f;
					position.y -= (GridSize.y - 1) * extraFactorX / 2.0f;
					break;
			}

			return position * TileSize;
		}

		public override int GetDistance(Vector2Int a, Vector2Int b)
		{
			return (Mathf.Abs(a.x - b.x) +
					Mathf.Abs(a.y - b.y) +
					Mathf.Abs(a.x + a.y - b.x - b.y)) / 2;
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

		public Vector2Int AxialToOffset(Vector2Int offset)
		{
			switch (Layout)
			{
				case HexLayout.Odd:
					switch (Orientation)
					{
						case HexOrientation.Pointed:
							return AxialToOddR(offset.x, offset.y);

						case HexOrientation.Flat:
							return AxialToOddQ(offset.x, offset.y);
					}

					break;
				case HexLayout.Even:
					switch (Orientation)
					{
						case HexOrientation.Pointed:
							return AxialToEvenR(offset.x, offset.y);

						case HexOrientation.Flat:
							return AxialToEvenQ(offset.x, offset.y);
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
	}
}