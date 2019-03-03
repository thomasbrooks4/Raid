using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class SaveData {

	public int characterAmount;
	public int[][] characterPositions;
	public string[] characterNames;
	public string[] characterClasses;

    public SaveData(List<CharacterData> characterData) {
        characterAmount = characterData.Count;
        characterPositions = new int[characterData.Count()][];
        characterNames = new string[characterData.Count()];
        characterClasses = new string[characterData.Count()];

        for (int i = 0; i < characterData.Count; i++) {
            characterNames[i] = characterData[i].CharacterName;
            characterClasses[i] = characterData[i].CharacterClass;
            characterPositions[i] = characterData[i].CharacterPosition;
        }
    }
}
