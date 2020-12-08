using Grids;
using UnityEngine;
using UnityEngine.UI;

public class Test : MonoBehaviour
{
	public Grids.Grid Grid;
	public Text Text;

	void Update()
	{
		Vector2 input = Input.mousePosition;
		Ray ray = Camera.main.ScreenPointToRay(input);
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
