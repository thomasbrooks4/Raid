using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CombatManager : MonoBehaviour {
	
	private static int LEFT_MOUSE_BUTTON = 0;
	private static int RIGHT_MOUSE_BUTTON = 1;
	
	private CombatGrid combatGrid;

	[SerializeField]
	private List<Character> selectedCharacters = new List<Character>();
	private bool isPaused;

	void Start() {
		combatGrid = GetComponent<CombatGrid>();

		initializeCombatScene();
	}

	// Update is called once per frame
	void Update() {
		if (Input.GetKeyDown(KeyCode.Space)) {
			isPaused = !isPaused;

			if (isPaused)
				Time.timeScale = 0f;
			else
				Time.timeScale = 1f;
		}

		// Selection
		if (Input.GetMouseButtonDown(LEFT_MOUSE_BUTTON)) {
			Vector3Int mousePos = Vector3Int.RoundToInt(Camera.main.ScreenToWorldPoint(Input.mousePosition));

			if (0 <= mousePos.x && mousePos.x <= (combatGrid.cols - 1) && 0 <= mousePos.y && mousePos.y <= (combatGrid.rows - 1)) {
				GridTile tile = combatGrid.GridTiles[mousePos.x, mousePos.y];

				if (tile.Character != null) {
					if (!Input.GetKeyDown(KeyCode.LeftControl))
						clearSelectedCharacters();

					selectedCharacters.Add(tile.Character);
					tile.Character.IsSelected = true;
					combatGrid.CharacterSelected = true;
				}
				else {
					if (selectedCharacters.Any()) {
						clearSelectedCharacters();

						combatGrid.CharacterSelected = false;
					}
				}
			}
		}

		if (Input.GetMouseButtonDown(RIGHT_MOUSE_BUTTON)) {
			if (selectedCharacters.Any()) {
				Vector3Int mousePos = Vector3Int.RoundToInt(Camera.main.ScreenToWorldPoint(Input.mousePosition));

				if (0 <= mousePos.x && mousePos.x <= (combatGrid.cols - 1) && 0 <= mousePos.y && mousePos.y <= (combatGrid.rows - 1)) {
					GridTile tile = combatGrid.GridTiles[mousePos.x, mousePos.y];

					// TODO: Add logic for selecting single tile with multiple characters
					selectedCharacters.First().SetTargetTile(tile);
				}
			}
		}
	}

	private void initializeCombatScene() {
		combatGrid.Initialize(GameManager.Instance.playerSave);

		isPaused = false;
	}

	private void clearSelectedCharacters() {
		foreach (Character character in selectedCharacters)
			character.IsSelected = false;

		selectedCharacters.Clear();
	}
}
