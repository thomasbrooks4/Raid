using UnityEngine;

public class GameManager : MonoBehaviour {

	public static GameManager Instance { get; private set; }

	public SaveData playerSave;

	private void Awake() {
		if (Instance == null) {
			Instance = this;
			DontDestroyOnLoad(gameObject);
			SaveManager.SaveGame();
			playerSave = SaveManager.LoadSave();
		}
		else {
			Destroy(gameObject);
		}
	}
}
