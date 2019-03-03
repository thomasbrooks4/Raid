using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class SaveManager {
	
	private static readonly string SAVE_DATA_PATH = Application.persistentDataPath + "/raid-dev.save";

	public static void SaveGame(SaveData saveData) {
		BinaryFormatter formatter = new BinaryFormatter();
		FileStream stream = new FileStream(SAVE_DATA_PATH, FileMode.Create);

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
}
