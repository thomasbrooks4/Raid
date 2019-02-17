using UnityEngine;

public class CombatManager : MonoBehaviour {

	private static string SPACE_BAR = "space";
	private static int LEFT_MOUSE_BUTTON = 0;
	private static int RIGHT_MOUSE_BUTTON = 1;

	private GameManager gameManager;
	private CombatGrid combatGrid;

	[SerializeField]
	private Character selectedCharacter;
	private bool isPaused;

	void Awake() {
		gameManager = GetComponent<GameManager>();
		combatGrid = GetComponent<CombatGrid>();

		initializeCombatScene();
	}

	// Update is called once per frame
	void Update() {
		if (Input.GetKeyDown(SPACE_BAR)) {
			isPaused = !isPaused;

			if (isPaused)
				Time.timeScale = 0f;
			else
				Time.timeScale = 1f;
		}

		if (Input.GetMouseButtonDown(LEFT_MOUSE_BUTTON)) {
			Vector3Int mousePos = Vector3Int.RoundToInt(Camera.main.ScreenToWorldPoint(Input.mousePosition));
			GridTile tile = combatGrid.GridTiles[mousePos.x, mousePos.y];

			if (tile.Character != null) {
				if (selectedCharacter != null)
					selectedCharacter.IsSelected = false;
				
				tile.Character.IsSelected = true;
				combatGrid.CharacterSelected = true;
				selectedCharacter = tile.Character;
			}
			else {
				if (selectedCharacter != null)
					selectedCharacter.IsSelected = false;

				selectedCharacter = null;
				combatGrid.CharacterSelected = false;
			}
		}

		if (Input.GetMouseButtonDown(RIGHT_MOUSE_BUTTON)) {
			if (selectedCharacter != null) {
				Vector3Int mousePos = Vector3Int.RoundToInt(Camera.main.ScreenToWorldPoint(Input.mousePosition));
				GridTile tile = combatGrid.GridTiles[mousePos.x, mousePos.y];

				selectedCharacter.SetTargetTile(tile);
			}
		}
	}

	private void initializeCombatScene() {
		combatGrid.Initialize(gameManager.FriendlyClan);

		selectedCharacter = null;
		isPaused = false;
	}
}
