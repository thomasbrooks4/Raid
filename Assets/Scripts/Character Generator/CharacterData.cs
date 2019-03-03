public class CharacterData {

    public string CharacterName { get; set; }
    public string CharacterClass { get; set; }
    public int[] CharacterPosition { get; set; }

    public CharacterData(string characterName, string characterClass) {
        CharacterName = characterName;
        CharacterClass = characterClass;
        CharacterPosition = new int[] { 0, 0 };
    }
}
