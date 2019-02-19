using System;
using System.Collections.Generic;

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
		this.characterPositions = new int[characterAmount][];
		this.characterNames = characterNames;
		this.characterClasses = characterClasses;

		/*
		characterAmount = playerClan.Count;
		characterPositions = new int[characterAmount][];
		characterNames = new string[characterAmount];
		characterClasses = new string[characterAmount];

		for (int i = 0; i < characterAmount; i++) {
			characterNames[i] = playerClan[i].CharacterName;
			characterClasses[i] = playerClan[i].CharacterClass.ToString();
		}
		*/
	}

	public void setCharacterPosition(string characterName, int x, int y) {
		int index = Array.IndexOf(characterNames, characterName);

		if (index >= 0) {
			characterPositions[index] = new int[2] { x, y };
		}
	}
}
