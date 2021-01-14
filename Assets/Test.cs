using System.Linq;
using Grids;
using UnityEngine;
using UnityEngine.UI;

public class Test : MonoBehaviour
{
	public BaseGrid Grid;
	public Text Text;

	private new Camera camera;

	private void Awake()
	{
		camera = Camera.main;
	}

	private void Update()
	{
		if (!Input.GetMouseButtonDown(0))
			return;

		Vector2 input = Input.mousePosition;
		Ray ray = camera.ScreenPointToRay(input);
		bool result = Physics.Raycast(ray, out RaycastHit rayHit, float.PositiveInfinity, -5);

		if (result)
		{
			Tile tile = rayHit.transform.GetComponent<Tile>();

			HexGrid grid = (HexGrid) Grid;

			Debug.Log(tile.GridPosition);
			Debug.Log(grid.GetNeighbours(tile.GridPosition).Count());

			foreach (var pos in grid.GetNeighbours(tile.GridPosition))
			{
				Debug.Log(pos + " " + grid.IsValid(pos));
				//pos.gameObject.SetActive(false);
			}

			//Text.text = tile.GridPosition + " " + Grid.WorldToGrid(rayHit.point);
		}
		else
		{
			Text.text = "Nothing";
		}
	}
}
