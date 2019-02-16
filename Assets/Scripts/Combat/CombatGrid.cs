using System.Collections.Generic;
using UnityEngine;

public class CombatGrid : MonoBehaviour {

	private Transform gridTransform;
	public GameObject gridTilePrefab;

	private int cols;
	private int rows;
	private GridTile[,] gridTiles;

	public GridTile[,] GridTiles { get => gridTiles; }

	public void Initialize(int cols, int rows, List<GameObject> clan) {
		this.cols = cols;
		this.rows = rows;
		gridTiles = new GridTile[cols, rows];

		generateGrid();
		populateFriendlyClan(clan);
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
			gridTiles[colPos, rowPos].Character.Initialize();

			rowPos++;
		}
	}
}
