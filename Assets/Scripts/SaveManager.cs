using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class SaveManager {
	
	private static readonly string SAVE_DATA_PATH = Application.persistentDataPath + "/raid-dev.save";

	public static void SaveGame() {
		BinaryFormatter formatter = new BinaryFormatter();
		FileStream stream = new FileStream(SAVE_DATA_PATH, FileMode.Create);

        // SaveData playerSaveData = GameManager.Instance.playerSave;
        SaveData saveData = CreateSaveData();

		formatter.Serialize(stream, saveData);
		stream.Close();
	}

	public static SaveData LoadSave() {
		if (File.Exists(SAVE_DATA_PATH)) {
			BinaryFormatter formatter = new BinaryFormatter();
			FileStream stream = new FileStream(SAVE_DATA_PATH, FileMode.Open);

			SaveData saveData = formatter.Deserialize(stream) as SaveData;
			stream.Close();

			return saveData;
		}
		else {
			Debug.LogError("Save file not found in " + SAVE_DATA_PATH);
			return null;
		}
	}

    // TEST PURPOSES
    private static SaveData CreateSaveData() {
        const int characterAmount = 2;
        int[][] characterPositions = new int[characterAmount][];
        characterPositions[0] = new int[] { 0, 0 };
        characterPositions[1] = new int[] { 1, 4 };
        string[] characterNames = { "thomas", "brianna" };
        string[] characterClasses = { "WARRIOR", "WARRIOR" };

        return new SaveData(characterAmount, characterPositions, characterNames, characterClasses);
    }
}
