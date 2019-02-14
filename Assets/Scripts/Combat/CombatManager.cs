using UnityEngine;

public class CombatManager : MonoBehaviour {

	private GameManager gameManagerScript;
	private CombatGrid combatGridScript;
	
	void Awake() {
		gameManagerScript = GetComponent<GameManager>();
		combatGridScript = GetComponent<CombatGrid>();

		initializeCombatScene();
	}

	// Update is called once per frame
	void Update() {

	}

	private void initializeCombatScene() {
		combatGridScript.SetupCombatGrid(gameManagerScript.Clan);
	}
}
