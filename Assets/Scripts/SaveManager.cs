using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class SaveManager {

	private static string SAVE_DATA_PATH = "C:\\Users\\thomas\\Desktop\\raid-dev.save";
	//private static string SAVE_DATA_PATH = Application.persistentDataPath + "/raid.save";

	public static void SaveGame() {
		BinaryFormatter formatter = new BinaryFormatter();
		FileStream stream = new FileStream(SAVE_DATA_PATH, FileMode.Create);

		const int characterAmount = 2;
		int[][] characterPositions = new int[characterAmount][];
		characterPositions[0] = new int[] { 0, 0 };
		characterPositions[1] = new int[] { 1, 4 };
		string[] characterNames = new string[characterAmount] { "thomas", "brianna" };
		string[] characterClasses = new string[characterAmount] { "WARRIOR", "WARRIOR" };

		SaveData saveData = new SaveData(characterAmount, characterPositions, characterNames, characterClasses);

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
