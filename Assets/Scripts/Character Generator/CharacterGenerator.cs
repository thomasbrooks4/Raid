using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterGenerator : MonoBehaviour {

    public GameObject namePrefab;
    public GameObject classPrefab;

    private GameObject[] panels;
    private GameObject[] texts;

    private List<InputField> inputs = new List<InputField>();
    private List<Dropdown> dropdowns = new List<Dropdown>();
    private List<CharacterData> characterData = new List<CharacterData>();

    public int PlayerCharacterAmount { get; set; }
    public int EnemyCharacterAmount { get; set; }

    void Start() {
        PlayerCharacterAmount = 1;
        EnemyCharacterAmount = 1;

        panels = GameObject.FindGameObjectsWithTag("Panel");
        texts = GameObject.FindGameObjectsWithTag("Text");

        texts[0].GetComponent<Text>().text = PlayerCharacterAmount.ToString();
        texts[1].GetComponent<Text>().text = EnemyCharacterAmount.ToString();

        inputs.Add(FindObjectOfType<InputField>());
        dropdowns.Add(FindObjectOfType<Dropdown>());
    }

    public void IncreasePlayerAmount() {
        if (PlayerCharacterAmount < 5) {
            PlayerCharacterAmount++;

            texts[0].GetComponent<Text>().text = PlayerCharacterAmount.ToString();

            GameObject nameObject = Instantiate(namePrefab) as GameObject;
            nameObject.transform.SetParent(panels[0].transform);
            inputs.Add(nameObject.GetComponent<InputField>());

            GameObject classObject = Instantiate(classPrefab) as GameObject;
            classObject.transform.SetParent(panels[1].transform);
            dropdowns.Add(classObject.GetComponent<Dropdown>());
        }
    }

    public void DecreasePlayerAmount() {
        if (PlayerCharacterAmount > 1) {
            PlayerCharacterAmount--;

            texts[0].GetComponent<Text>().text = PlayerCharacterAmount.ToString();

            Destroy(inputs[inputs.Count - 1].gameObject);
            inputs.RemoveAt(inputs.Count - 1);

            Destroy(dropdowns[dropdowns.Count - 1].gameObject);
            dropdowns.RemoveAt(dropdowns.Count - 1);
        }
    }

    public void IncreaseEnemyAmount() {
        if (EnemyCharacterAmount < 5)
            EnemyCharacterAmount++;

        texts[1].GetComponent<Text>().text = EnemyCharacterAmount.ToString();
    }

    public void DecreaseEnemyAmount() {
        if (EnemyCharacterAmount > 1)
            EnemyCharacterAmount--;

        texts[1].GetComponent<Text>().text = EnemyCharacterAmount.ToString();
    }

    public void SaveCharacters() {
        for (int i = 0; i < PlayerCharacterAmount; i++) {
            characterData.Add(new CharacterData(inputs[i].text, dropdowns[i].value.ToString()));
        }

        SaveData saveData = new SaveData(characterData);
        SaveManager.SaveGame(saveData);
        GameManager.Instance.EnemyAmount = EnemyCharacterAmount;
    }
}
