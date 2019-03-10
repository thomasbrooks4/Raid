using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatGrid : MonoBehaviour {

	private Transform gridTransform;
	public GameObject gridTilePrefab;
	public GameObject warriorPrefab;
	
	public int cols;
	public int rows;

	public GridTile[,] GridTiles { get; private set; }
	public bool CharacterSelected { get; set; }

	public void Initialize() {
		GridTiles = new GridTile[cols, rows];
		CharacterSelected = false;

		GenerateGrid();
		SetupPlayerParty();
		SetupEnemyParty();

		MoveCharactersToStartingPositions();
		// TODO: Consider wait until all characters reached thier start positions
	}

	public List<GridTile> GetSurroundingTiles(GridTile tile) {
		List<GridTile> surrounding = new List<GridTile>();

		for (int x = -1; x <= 1; x++) {
			for (int y = -1; y <= 1; y++) {
				if (x == 0 && y == 0)
					continue;

				int checkX = tile.GridPos.x + x;
				int checkY = tile.GridPos.y + y;

				if (0 <= checkX && checkX < cols && 0 <= checkY && checkY < rows)
					surrounding.Add(GridTiles[checkX, checkY]);
			}
		}

		return surrounding;
	}

	private void GenerateGrid() {
		gridTransform = new GameObject("Combat Grid").transform;

		for (int x = 0; x < cols; x++) {
			for (int y = 0; y < rows; y++) {
				GameObject tile = Instantiate(gridTilePrefab, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;

				tile.transform.SetParent(gridTransform);
				tile.name = "(" + x + ", " + y + ") Grid Tile";

				GridTiles[x, y] = tile.GetComponent<GridTile>();
				GridTiles[x, y].Initialize(new Vector2Int(x, y), this);
			}
		}
	}

	private void SetupPlayerParty() {
		System.Random random = new System.Random();
		SaveData playerSave = GameManager.Instance.PlayerSave;
		int countX = 0, countY = 0;
		
		for (int i = 0; i < playerSave.characterAmount; i++) {
			GameObject characterObject;
			int startX, startY;

			if (!(playerSave.characterPositions[i][0] == 0 && playerSave.characterPositions[i][1] == 0)) {
				startX = playerSave.characterPositions[i][0];
				startY = playerSave.characterPositions[i][1];
			}
			else {
				startX = countX;
				startY = countY++;

				if (countY == rows) {
					countX++;
					countY = 0;
				}
			}

			// if (playerSave.characterClasses[i].Equals(WARRIOR)) use once we add more classes
			characterObject = Instantiate(warriorPrefab, 
				new Vector3(random.Next(-4, -1), startY, 0f), Quaternion.identity) as GameObject;
			characterObject.name = playerSave.characterNames[i];

			Character character = characterObject.GetComponent<Character>();
			character.StartPosition = new Vector2Int(startX, startY);
			character.GridTile = GridTiles[startX, startY];
			character.CharacterName = playerSave.characterNames[i];
			character.CharacterClass = CharacterClass.WARRIOR;
			character.Friendly = true;

			GridTiles[startX, startY].Character = character;
		}
	}

	private void SetupEnemyParty() {
        // TODO: Pass in information to generate characters
        int enemyAmount = GameManager.Instance.EnemyAmount;

        System.Random random = new System.Random();
		int countX = (cols - 1), countY = (rows - 1);

		for (int i = 0; i < enemyAmount; i++) {
			GameObject characterObject;
			int startX, startY;

			startX = countX;
			startY = countY--;

			if (countY < 0) {
				countX--;
				countY = (rows - 1);
			}

			// if (playerSave.characterClasses[i].Equals(WARRIOR)) use once we add more classes
			characterObject = Instantiate(warriorPrefab,
				new Vector3(random.Next(17, 20), startY, 0f), Quaternion.identity) as GameObject;
			// TODO: Name generator
			characterObject.name = "Enemy " + i;
            characterObject.AddComponent<WarriorAI>();

			Character enemy = characterObject.GetComponent<Character>();
			enemy.StartPosition = new Vector2Int(startX, startY);
			enemy.GridTile = GridTiles[startX, startY];
			enemy.CharacterName = "Enemy "  + i;
			enemy.CharacterClass = CharacterClass.WARRIOR;
			enemy.Friendly = false;

			GridTiles[startX, startY].Character = enemy;
		}
	}

	#region Move Characters
	private void MoveCharactersToStartingPositions() {
		GameObject[] characterObjects = GameObject.FindGameObjectsWithTag("Character");

		foreach (GameObject characterObject in characterObjects) {
			Character character = characterObject.GetComponent<Character>();
			StartCoroutine(MoveCharacter(character));
		}
	}

	private IEnumerator MoveCharacter(Character character) {
		const float BASE_MOVE_SPEED = 0.5f;
		character.IsMoving = true;

		float sqrRemainingDistance = (character.GetTransfromPostionVector2() - character.StartPosition).sqrMagnitude;

		while (sqrRemainingDistance > float.Epsilon) {
			float inverseMoveTime = (1f / BASE_MOVE_SPEED) * character.Speed;

			Vector2 newPosition = Vector2.MoveTowards(character.transform.position, character.StartPosition, inverseMoveTime * Time.deltaTime);
			character.transform.position = newPosition;
			sqrRemainingDistance = (character.GetTransfromPostionVector2() - character.StartPosition).sqrMagnitude;

			yield return null;
		}

		character.IsMoving = false;
	}
	#endregion
}
