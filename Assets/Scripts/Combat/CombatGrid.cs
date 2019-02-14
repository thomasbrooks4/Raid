using System.Collections.Generic;
using UnityEngine;

public class CombatGrid : MonoBehaviour {

	private Vector3 startingPosition = new Vector3(-8, -1, 0);
	private int rows = 5;
	private int cols = 16;

	private Transform combatGrid;
	private List<Vector3> gridPositions = new List<Vector3>();

	public GameObject gridTile;
	
	public void SetupCombatGrid() {
		initializeCombatGrid();
		initializeGridPositions();
	}

	private void initializeGridPositions() {
		gridPositions.Clear();

		for (int x = 0; x < cols; x++) {
			for (int y = 0; y < rows; y++) {
				gridPositions.Add(new Vector3(x, y, 0f));
			}
		}
	}

	private void initializeCombatGrid() {
		combatGrid = new GameObject("Combat Grid").transform;

		for (int x = 0; x < cols; x++) {
			for (int y = 0; y < rows; y++) {
				GameObject instance = Instantiate(gridTile, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;

				instance.transform.SetParent(combatGrid);
			}
		}
	}
}
