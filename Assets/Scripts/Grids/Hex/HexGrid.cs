using Sirenix.OdinInspector;
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
					MakeHexMap(GridSize.x - 1);
					break;

				case HexShape.Rectangle:
					MakeRectMap(GridSize.x, GridSize.y);
					break;

				case HexShape.Triangle:
					MakeTriMap(GridSize.x);
					break;
			}
		}

		public void MakeHexMap(int size)
		{
			for (int x=-size; x <=size; x++)
				for (int y=-size; y <=size; y++)
					if (Mathf.Abs(-x - y) <= size)
						GenerateTile(new Vector2Int(x, y));
		}

		public void MakeRectMap(int width, int height)
		{
			for (int x=0; x <width; x++)
				for (int y=0; y<height; y++)
					Tiles[x, y] = GenerateTile(OffsetToAxial(new Vector2Int(x, y)));
		}

		public void MakeTriMap(int size)
		{
			for (int y=0; y <=size; y++)
				for (int x=-y; x <=0; x++)
					GenerateTile(new Vector2Int(x, y));
		}


		public override Vector2Int WorldToGridLocal(float x, float y, float z)
		{
			throw new System.NotImplementedException();
		}

		public override Vector3 GridToWorldLocal(int x, int y)
		{
			switch (Orientation)
			{
				case HexOrientation.Pointed:
					return new Vector3(x * TileSize.x + (y / 2.0f * TileSize.x),
									   y * TileSize.y * 0.75f);

				case HexOrientation.Flat:
					return new Vector3(x * TileSize.x * 0.75f,
									   y * TileSize.y + (x /2.0f * TileSize.y));
			}

			return Vector3.zero;
		}

		public override int GetDistance(Vector2Int a, Vector2Int b)
		{
			return (Mathf.Abs(a.x - b.x) +
					Mathf.Abs(a.y - b.y) +
					Mathf.Abs(a.x + a.y - b.x - b.y)) / 2;
		}

		public Vector2Int OffsetToAxial(Vector2Int offset)
		{
			switch (Layout)
			{
				case HexLayout.Odd:
					switch (Orientation)
					{
						case HexOrientation.Pointed:
							return OddRToAxial(offset);

						case HexOrientation.Flat:
							return OddQToAxial(offset);
					}

					break;
				case HexLayout.Even:
					switch (Orientation)
					{
						case HexOrientation.Pointed:
							return EvenRToAxial(offset);

						case HexOrientation.Flat:
							return EvenQToAxial(offset);
					}

					break;
			}

			return default;
		}

		public static Vector2Int OddQToAxial(Vector2Int offset)
		{
			return new Vector2Int(offset.x, offset.y - ((offset.x - (offset.x & 1)) >> 1));
		}

		public static Vector2Int OddRToAxial(Vector2Int offset)
		{
			return new Vector2Int(offset.x - ((offset.y + (offset.y & 1)) >> 1), offset.y);
		}

		public static Vector2Int EvenQToAxial(Vector2Int offset)
		{
			return new Vector2Int(offset.x, offset.y - ((offset.x + (offset.x & 1)) >> 1));
		}

		public static Vector2Int EvenRToAxial(Vector2Int offset)
		{
			return new Vector2Int(offset.x - ((offset.y - (offset.y & 1)) >> 1), offset.y);
		}

		public Vector2Int AxialToOffset(Vector2Int offset)
		{
			switch (Layout)
			{
				case HexLayout.Odd:
					switch (Orientation)
					{
						case HexOrientation.Pointed:
							return AxialToOddR(offset);

						case HexOrientation.Flat:
							return AxialToOddQ(offset);
					}

					break;
				case HexLayout.Even:
					switch (Orientation)
					{
						case HexOrientation.Pointed:
							return AxialToEvenR(offset);

						case HexOrientation.Flat:
							return AxialToEvenQ(offset);
					}

					break;
			}

			return default;
		}

		public static Vector2Int AxialToOddQ(Vector2Int offset)
		{
			return new Vector2Int(offset.x, offset.y + (offset.x - (offset.x & 1)) / 2);
		}

		public static Vector2Int AxialToOddR(Vector2Int offset)
		{
			return new Vector2Int(offset.x + (offset.y - (offset.y & 1)) / 2, offset.y);
		}

		public static Vector2Int AxialToEvenQ(Vector2Int offset)
		{
			return new Vector2Int(offset.x, offset.y + (offset.x + (offset.x & 1)) / 2);
		}

		public static Vector2Int AxialToEvenR(Vector2Int offset)
		{
			return new Vector2Int(offset.x + (offset.y + (offset.y & 1)) / 2, offset.y);
		}
	}
}