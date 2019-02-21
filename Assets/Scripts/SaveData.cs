using System;

[Serializable]
public class SaveData {

	public int characterAmount;
	public int[][] characterPositions;
	public string[] characterNames;
	public string[] characterClasses;

	public SaveData(int characterAmount, int[][] characterPositions, string[] characterNames, string[] characterClasses) {
		this.characterAmount = characterAmount;
		this.characterPositions = characterPositions;
		this.characterNames = characterNames;
		this.characterClasses = characterClasses;
	}

	public SaveData(int characterAmount, string[] characterNames, string[] characterClasses) {
		this.characterAmount = characterAmount;
		characterPositions = new int[characterAmount][];
		this.characterNames = characterNames;
		this.characterClasses = characterClasses;
	}

	public void SetCharacterPosition(string characterName, int x, int y) {
		int index = Array.IndexOf(characterNames, characterName);

		if (index >= 0) {
			characterPositions[index] = new int[] { x, y };
		}
	}
}
