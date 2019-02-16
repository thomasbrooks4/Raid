using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
	public List<GameObject> friendlyClan = new List<GameObject>();

	public List<GameObject> FriendlyClan { get => friendlyClan; }

	public void AddToFriendlyClan(GameObject character) {
		friendlyClan.Add(character);
	}

	public void RemoveFromFriendlyClan(GameObject character) {
		friendlyClan.Remove(character);
	}
}
