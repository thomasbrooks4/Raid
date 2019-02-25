using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CombatManager : MonoBehaviour {

	private const int LEFT_MOUSE_BUTTON = 0;
	private const int RIGHT_MOUSE_BUTTON = 1;

	private CombatGrid combatGrid;

	private List<Character> selectedCharacters = new List<Character>();
	private bool isPaused;

	void Start() {
		combatGrid = GetComponent<CombatGrid>();

		InitializeCombatScene();
	}

	// Update is called once per frame
	void Update() {
		// Pause combat
		if (Input.GetKeyDown(KeyCode.Space)) {
			TogglePause();
		}

		// Selection
		if (Input.GetMouseButtonDown(LEFT_MOUSE_BUTTON)) {
			Vector3Int mousePos = Vector3Int.RoundToInt(Camera.main.ScreenToWorldPoint(Input.mousePosition));

			if (WithinGrid(mousePos)) {
				GridTile tile = combatGrid.GridTiles[mousePos.x, mousePos.y];

				if (tile.Character != null && tile.Character.Friendly) {
					if (!Input.GetKey(KeyCode.LeftShift))
						ClearSelectedCharacters();

					selectedCharacters.Add(tile.Character);
					tile.Character.IsSelected = true;
					combatGrid.CharacterSelected = true;
				}
				else {
					if (selectedCharacters.Any())
						ClearSelectedCharacters();
				}
			}
		}

		// Movement
		if (Input.GetMouseButtonDown(RIGHT_MOUSE_BUTTON) && !Input.GetKey(KeyCode.LeftShift)) {
			if (selectedCharacters.Any()) {
				Vector3Int mousePos = Vector3Int.RoundToInt(Camera.main.ScreenToWorldPoint(Input.mousePosition));

				if (WithinGrid(mousePos)) {
					GridTile tile = combatGrid.GridTiles[mousePos.x, mousePos.y];

					foreach (Character selectedCharacter in selectedCharacters) {
						selectedCharacter.SetTargetTile(tile);
					}
				}
			}
		}

        // Create paths
        if (Input.GetMouseButton(RIGHT_MOUSE_BUTTON) && Input.GetKey(KeyCode.LeftShift)) {
            if (selectedCharacters.Any()) {
                Vector3Int mousePos = Vector3Int.RoundToInt(Camera.main.ScreenToWorldPoint(Input.mousePosition));

                if (WithinGrid(mousePos)) {
                    GridTile tile = combatGrid.GridTiles[mousePos.x, mousePos.y];

                    foreach (Character selectedCharacter in selectedCharacters) {
                        if (!selectedCharacter.Path.Contains(tile)) {
                            selectedCharacter.Path.Add(tile);
                        }
                    }
                }
            }
        }

        // Rotate left
        if (Input.GetKeyDown(KeyCode.E)) {
			foreach (Character character in selectedCharacters) {
				character.RotateLeft();
			}
		}

		// Rotate right
		if (Input.GetKeyDown(KeyCode.R)) {
			foreach (Character character in selectedCharacters) {
				character.RotateRight();
			}
		}

		// Reset direction
		if (Input.GetKeyDown(KeyCode.T)) {
			foreach (Character character in selectedCharacters) {
				character.ResetDirection();
			}
		}

		// Toggle attack stance
		if (Input.GetKeyDown(KeyCode.A)) {
			foreach (Character character in selectedCharacters) {
				character.ToggleAttackStance();
			}
		}

		// Change stance
		if (Input.GetKeyDown(KeyCode.S)) {
			foreach (Character character in selectedCharacters) {
				character.ToggleAttackStance();
			}
		}

		// Toggle direction lock
		if (Input.GetKeyDown(KeyCode.D)) {
			foreach (Character character in selectedCharacters) {
				character.ToggleDirectionLocked();
			}
		}

		// Toggle focus lock
		if (Input.GetKeyDown(KeyCode.F)) {
			foreach (Character character in selectedCharacters) {
				character.ToggleFocusLocked();
			}
		}
	}

	private void InitializeCombatScene() {
		combatGrid.Initialize(GameManager.Instance.playerSave);

		isPaused = false;
	}

	private void TogglePause() {
		isPaused = !isPaused;

		if (isPaused)
			Time.timeScale = 0f;
		else
			Time.timeScale = 1f;
	}

	private void ClearSelectedCharacters() {
		foreach (Character character in selectedCharacters)
			character.IsSelected = false;

		combatGrid.CharacterSelected = false;
		selectedCharacters.Clear();
	}

    private bool WithinGrid(Vector3Int position) {
        return 0 <= position.x && position.x <= (combatGrid.cols - 1)
                && 0 <= position.y && position.y <= (combatGrid.rows - 1);
    }

}
