using UnityEngine;

namespace Grids
{
	public class HexGrid: Grid
	{
		public HexShape Shape;
		public HexLayout Layout;
		public HexOrientation Orientation;

		public override Vector2Int WorldToGrid(float x, float y, float z)
		{
			throw new System.NotImplementedException();
		}

		public override Vector3 GridToWorld(int x, int y)
		{
			throw new System.NotImplementedException();
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