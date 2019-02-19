using System.Collections.Generic;
using UnityEngine;

public class CombatGrid : MonoBehaviour {

	private Transform gridTransform;
	public GameObject gridTilePrefab;
	public GameObject warriorPrefab;
	
	public int cols;
	public int rows;
	private GridTile[,] gridTiles;
	private bool characterSelected;
	
	public GridTile[,] GridTiles { get => gridTiles; }
	public bool CharacterSelected { get => characterSelected; set => characterSelected = value; }

	public void Initialize(SaveData playerSave) {
		gridTiles = new GridTile[cols, rows];
		characterSelected = false;

		generateGrid();
		populatePlayerClan(playerSave);
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
				tile.name = "(" + x + ", " + y + ") Grid Tile";

				gridTiles[x, y] = tile.GetComponent<GridTile>();
				gridTiles[x, y].Initialize(new Vector2Int(x, y), this);
			}
		}
	}

	private void populatePlayerClan(SaveData playerSave) {
		int xPos = 0;
		for (int i = 0; i < playerSave.characterAmount; i++) {
			GameObject characterObject;
			// if (characterData.CharacterClass.Equals(Class.WARRIOR)) use once we add more classes
			if (playerSave.characterPositions[i] != null) {
				characterObject = Instantiate(warriorPrefab, new Vector3(playerSave.characterPositions[i][0], 
					playerSave.characterPositions[i][1], 0f), Quaternion.identity) as GameObject;


				Character character = characterObject.GetComponent<Character>();
				character.GridTile = gridTiles[playerSave.characterPositions[i][0], playerSave.characterPositions[i][1]];

				gridTiles[playerSave.characterPositions[i][0], playerSave.characterPositions[i][1]].Character = character;
			}
			else {
				characterObject = Instantiate(warriorPrefab, new Vector3(xPos, i, 0f), Quaternion.identity) as GameObject;

				Character character = characterObject.GetComponent<Character>();
				character.GridTile = gridTiles[xPos, i];
				character.CharacterName = playerSave.characterNames[i];
				gridTiles[xPos, i].Character = character;
			}
		}
	}
}
