using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Warrior : Character {

	private static float BASE_SPEED = 0.9f;
	private static float GUARD_SPEED = 0.5f;

	private bool onGuard;

	// Start is called before the first frame update
	public override void Start() {
		base.Start();

		maxHealth = 125;
		health = maxHealth;
		speed = BASE_SPEED;
		cooldown = 0.2f;

		onGuard = false;
	}

	// Update is called once per frame
	public override void Update() {
		base.Update();

		if (Input.GetKeyDown(KeyCode.G))
			changeGuard();
	}
	
	private void changeGuard() {
		onGuard = !onGuard;

		if (onGuard)
			speed = GUARD_SPEED;
		else
			speed = BASE_SPEED;
	}
}
