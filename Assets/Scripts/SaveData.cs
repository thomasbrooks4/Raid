using System;
using System.Collections.Generic;
using System.Linq;

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
	}

	public SaveData(List<Character> characters) {
		characterAmount = characters.Count();
		characterPositions = new int[characters.Count()][];
		characterNames = new string[characters.Count()];
		characterClasses = new string[characters.Count()];

		
		for (int i = 0; i < characters.Count(); i++) {
			if (characters[i].StartPosition != null)
				characterPositions[i] = new int[] { characters[i].StartPosition.x, characters[i].StartPosition.y };
			characterNames[i] = characters[i].CharacterName;
			characterClasses[i] = characters[i].CharacterClass.ToString();
		}
	}

	public void SetCharacterPosition(string characterName, int x, int y) {
		int index = Array.IndexOf(characterNames, characterName);

		if (index >= 0) {
			characterPositions[index] = new int[] { x, y };
		}
	}
}
