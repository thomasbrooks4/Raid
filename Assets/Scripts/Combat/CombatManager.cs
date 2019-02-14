using UnityEngine;

public class CombatManager : MonoBehaviour {

	private CombatGrid combatGridScript;
	
	void Awake() {
		combatGridScript = GetComponent<CombatGrid>();

		initializeCombatScene();
	}

	// Update is called once per frame
	void Update() {

	}

	private void initializeCombatScene() {
		combatGridScript.SetupCombatGrid();
	}
}
