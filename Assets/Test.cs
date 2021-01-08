using Grids;
using UnityEngine;
using UnityEngine.UI;

public class Test : MonoBehaviour
{
	public Grids.Grid Grid;
	public Text Text;

	private new Camera camera;

	private void Awake()
	{
		camera = Camera.main;
	}

	private void Update()
	{
		Vector2 input = Input.mousePosition;
		Ray ray = camera.ScreenPointToRay(input);
		bool result = Physics.Raycast(ray, out RaycastHit rayHit, float.PositiveInfinity, -5);

		if (result)
		{
			Tile tile = rayHit.transform.GetComponent<Tile>();
			Text.text = tile.GridPosition + " " + Grid.WorldToGrid(rayHit.point);
		}
		else
		{
			Text.text = "Nothing";
		}
	}
}
