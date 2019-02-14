using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
	public List<Character> clan = new List<Character>();

	public List<Character> Clan { get => clan; }

	public void AddToClan(Character character) {
		clan.Add(character);
	}

	public void RemoveFromClan(Character character) {
		clan.Remove(character);
	}
}
