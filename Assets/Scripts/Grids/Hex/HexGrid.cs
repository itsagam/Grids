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
					Tiles[x, y] = GenerateTile(new Vector2Int(x, y));
		}

		public void MakeTriMap(int size)
		{
			for (int y=0; y <=size; y++)
				for (int x=-y; x <=0; x++)
					GenerateTile(new Vector2Int(x, y));
		}


		public override Vector2Int WorldToGridLocal(float x, float y, float z)
		{
			return Vector2Int.zero;
			//throw new System.NotImplementedException();
		}

		public override Vector3 GridToWorldLocal(int x, int y)
		{
			switch (Shape)
			{
				case HexShape.Rectangle:

					Vector2Int transformed = OffsetToAxial(x, y);
					x = transformed.x;
					y = transformed.y;
					break;
			}

			Vector3 position = Vector3.zero;
			switch (Orientation)
			{
				case HexOrientation.Pointed:
					position = new Vector3(x * TileSize.x + (y / 2.0f * TileSize.x), y * TileSize.y * 0.75f);
					break;

				case HexOrientation.Flat:
					position = new Vector3(x * TileSize.x * 0.75f, y * TileSize.y + (x / 2.0f * TileSize.y));
					break;
			}


			//Vector2Int size = GridSize - Vector2Int.one; //OffsetToAxial(GridSize);
			//position -= (Vector3) (size * TileSize / 2.0f);
			return position;
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