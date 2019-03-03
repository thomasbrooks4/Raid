using UnityEngine;

public class GameManager : MonoBehaviour {

	public static GameManager Instance { get; private set; }

	public SaveData PlayerSave { get; set; }
    public int EnemyAmount { get; set; }

    private void Awake() {
		if (Instance == null) {
			Instance = this;
			DontDestroyOnLoad(gameObject);
            PlayerSave = SaveManager.LoadSave();
		}
		else {
			Destroy(gameObject);
		}
	}
}
