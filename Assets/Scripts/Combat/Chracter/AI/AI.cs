using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class AI : MonoBehaviour {
    private Character activeCharacter;
    [SerializeField]
    private List<Character> allEnemies;

    private GameObject combatManagerObject;
    private CombatManager combatManager;
    private CombatGrid combatGrid;

    // Start is called before the first frame update
    public virtual void Start() {
        activeCharacter = GetComponent<Character>();
        allEnemies = GetEnemies();

        combatManagerObject = GameObject.FindGameObjectWithTag("Combat Manager");
        combatManager = combatManagerObject.GetComponent<CombatManager>();
        combatGrid = combatManagerObject.GetComponent<CombatGrid>();
    }

    // Update is called once per frame
    public virtual void Update() {
        if (!combatManager.SetupPhase)
            activeCharacter.Target = NearestEnemy();
    }

    private List<Character> GetEnemies() {
        Character[] allCharacters = FindObjectsOfType<Character>();
        List<Character> enemies = new List<Character>();

        foreach (Character character in allCharacters) {
            if (activeCharacter.IsEnemy(character)) {
                enemies.Add(character);
            }
        }

        return enemies;
    }

    private Character NearestEnemy() {
        Character closestEnemy = allEnemies.First();
        float closestDistance = Mathf.Infinity;

        foreach (Character enemy in allEnemies) {
            float distance = (enemy.transform.position - activeCharacter.transform.position).sqrMagnitude;
            if (distance < closestDistance) {
                closestEnemy = enemy;
                closestDistance = distance;
            }
        }

        return closestEnemy;
    }
}
