using System.Collections.Generic;
using UnityEngine;

public class CombatGrid : MonoBehaviour {

	private Transform gridTransform;
	public GameObject gridTilePrefab;
	
	public int cols;
	public int rows;
	private GridTile[,] gridTiles;
	private bool characterSelected;
	
	public GridTile[,] GridTiles { get => gridTiles; }
	public bool CharacterSelected { get => characterSelected; set => characterSelected = value; }

	public void Initialize(List<GameObject> clan) {
		gridTiles = new GridTile[cols, rows];
		characterSelected = false;

		generateGrid();
		populateFriendlyClan(clan);
	}

	public List<GridTile> getSurroundingTiles(GridTile tile) {
		List<GridTile> surrounding = new List<GridTile>();

		for (int x = -1; x <= 1; x++) {
			for (int y = -1; y <= 1; y++) {
				if (x == 0 && y == 0)
					continue;

				int checkX = tile.GridPos.x + x;
				int checkY = tile.GridPos.y + y;

				if (0 <= checkX && checkX < cols && 0 <= checkY && checkY < rows)
					surrounding.Add(gridTiles[checkX, checkY]);
			}
		}

		return surrounding;
	}

	private void generateGrid() {
		gridTransform = new GameObject("Combat Grid").transform;

		for (int x = 0; x < cols; x++) {
			for (int y = 0; y < rows; y++) {
				GameObject tile = Instantiate(gridTilePrefab, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;

				tile.transform.SetParent(gridTransform);

				gridTiles[x, y] = tile.GetComponent<GridTile>();
				gridTiles[x, y].Initialize(new Vector2Int(x, y), this);
			}
		}
	}

	private void populateFriendlyClan(List<GameObject> clan) {
		int colPos = 0;
		int rowPos = 0;

		foreach (GameObject characterPrefab in clan) {
			if (rowPos == rows) {
				rowPos = 0;
				colPos++;
			}

			GameObject character = Instantiate(characterPrefab, new Vector3(colPos, rowPos, 0f), Quaternion.identity) as GameObject;

			gridTiles[colPos, rowPos].Character = character.GetComponent<Character>();
			gridTiles[colPos, rowPos].Character.Initialize(gridTiles[colPos, rowPos]);

			rowPos++;
		}
	}
}
