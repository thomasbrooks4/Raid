using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CombatManager : MonoBehaviour {

    private const int LEFT_MOUSE_BUTTON = 0;
    private const int RIGHT_MOUSE_BUTTON = 1;

	private List<Character> selectedCharacters = new List<Character>();

	public CombatGrid CombatGrid { get; set; }
	public bool SetupPhase { get; set; }
	public bool IsPaused { get; set; }

	void Start() {
        CombatGrid = GetComponent<CombatGrid>();
        GameManager.Instance.PlayerSave = SaveManager.LoadSave();

		SetupPhase = true;
		IsPaused = false;

		CombatGrid.Initialize();
	}

    // Update is called once per frame
    void Update() {
		// Selection
		if (Input.GetMouseButtonDown(LEFT_MOUSE_BUTTON)) {
			Vector3Int mousePos = Vector3Int.RoundToInt(Camera.main.ScreenToWorldPoint(Input.mousePosition));

			if (WithinGrid(mousePos)) {
				GridTile tile = CombatGrid.GridTiles[mousePos.x, mousePos.y];

				if (tile.Character != null && tile.Character.Friendly && tile.Character.IsAlive) {
					if (!Input.GetKey(KeyCode.LeftShift))
						ClearSelectedCharacters();

					selectedCharacters.Add(tile.Character);
					tile.Character.Select();
					CombatGrid.CharacterSelected = true;
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
					GridTile tile = CombatGrid.GridTiles[mousePos.x, mousePos.y];

					foreach (Character selectedCharacter in selectedCharacters) {
						selectedCharacter.SetTargetTile(tile);
					}
				}
			}
		}

		if (SetupPhase) {
			// Start combat
			if (Input.GetKeyDown(KeyCode.Return)) {
				// TODO: Start combat
				SetupPhase = false;
			}

			if (Input.GetKeyDown(KeyCode.Slash)) {
				// Save character positions
			}
		}
		else {
			RemoveDeadSelectedCharacters();

			// Pause combat
			if (Input.GetKeyDown(KeyCode.Space)) {
				TogglePause();
			}

			// Create paths
			if (Input.GetMouseButton(RIGHT_MOUSE_BUTTON) && Input.GetKey(KeyCode.LeftShift)) {
				if (selectedCharacters.Any()) {
					Vector3Int mousePos = Vector3Int.RoundToInt(Camera.main.ScreenToWorldPoint(Input.mousePosition));

					if (WithinGrid(mousePos)) {
						GridTile tile = CombatGrid.GridTiles[mousePos.x, mousePos.y];

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
					character.QueueLeftRotation();
				}
			}

			// Rotate right
			if (Input.GetKeyDown(KeyCode.R)) {
				foreach (Character character in selectedCharacters) {
					character.QueueRightRotation();
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

			// Change guard stance if warrior
			if (Input.GetKeyDown(KeyCode.S)) {
				foreach (Character character in selectedCharacters) {
					if (character.CharacterClass.Equals(CharacterClass.WARRIOR)) {
						Warrior warrior = (Warrior)character;
						warrior.ToggleGuardStance();
					}
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

			// Toggle guard if warrior
			if (Input.GetKeyDown(KeyCode.G)) {
				foreach (Character character in selectedCharacters) {
					if (character.CharacterClass.Equals(CharacterClass.WARRIOR)) {
						Warrior warrior = (Warrior)character;
						warrior.ToggleGuard();
					}
				}
			}

			// Clear path
			if (Input.GetKeyDown(KeyCode.C)) {
				foreach (Character character in selectedCharacters) {
					character.Path.Clear();
				}
			}
		}
    }

    private void TogglePause() {
        IsPaused = !IsPaused;

        if (IsPaused)
            Time.timeScale = 0f;
        else
            Time.timeScale = 1f;
    }

    private void ClearSelectedCharacters() {
        foreach (Character character in selectedCharacters)
            character.Deselect();

        CombatGrid.CharacterSelected = false;
        selectedCharacters.Clear();
    }

    private void RemoveDeadSelectedCharacters() {
        selectedCharacters.RemoveAll(c => !c.IsAlive);
    }

    private bool WithinGrid(Vector3Int position) {
        return 0 <= position.x && position.x <= (CombatGrid.cols - 1)
                && 0 <= position.y && position.y <= (CombatGrid.rows - 1);
    }

}
