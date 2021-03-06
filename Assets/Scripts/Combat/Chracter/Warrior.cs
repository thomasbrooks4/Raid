﻿using System.Collections.Generic;
using UnityEngine;

public class Warrior : Character {

    private const int DEFAULT_HEALTH = 125;
    private const int DEFAULT_DAMAGE = 35;
    private const float DEFAULT_SPEED = 0.9f;
    private const int DEFAULT_ATTACK_RANGE = 1;

    private const float GUARD_SPEED = 0.5f;
    private const float COOLDOWN = 0.2f;

	[SerializeField]
    private bool onGuard;
	[SerializeField]
    private bool highGuard;

    public bool OnGuard { get => onGuard; set => onGuard = value; }
    public bool HighGuard { get => highGuard; set => highGuard = value; }

    // Start is called before the first frame update
    public override void Start() {
        base.Start();

        characterClass = CharacterClass.WARRIOR;
        maxHealth = DEFAULT_HEALTH;
        health = maxHealth;
        damage = DEFAULT_DAMAGE;
        Speed = DEFAULT_SPEED;
        attackRange = DEFAULT_ATTACK_RANGE;
        cooldown = COOLDOWN;

        onGuard = false;
        highGuard = false;
    }

    // Update is called once per frame
    public override void Update() {
        base.Update();
    }

    protected override bool AttemptAttack() {
        // Can't attack while guarding low
        if (onGuard && !highGuard)
            return false;

        return true;
    }

    protected override void AttemptDamage() {
        if (target.CharacterClass.Equals(CharacterClass.WARRIOR)) {
            Warrior enemy = (Warrior)target;

            if (enemy.IsFacing(currentTile)) {
                if (enemy.OnGuard) {
                    if (highAttack) {
                        if (!enemy.HighGuard)
                            enemy.TakeDamage(damage);
                    }
                    else {
                        if (enemy.HighGuard)
                            enemy.TakeDamage(damage);
                    }
                }
                else {
                    enemy.TakeDamage(damage);
                }
            }
            else {
                // TODO: Add supporting methods to make this not as gross
                List<GridTile> surroudingEnemyTiles = enemy.GridTile.Grid.GetSurroundingTiles(enemy.GridTile);

                // if adjacent enemy is 
                enemy.TakeDamage(damage);
            }
        }
    }

    public override void ToggleAttackStance() {
        if (actionCooldown)
            return;

        StartCoroutine(ActionCooldown());

        if (!onGuard)
            highAttack = !highAttack;
    }

    public void ToggleGuard() {
        if (actionCooldown)
            return;

        StartCoroutine(ActionCooldown());

        onGuard = !onGuard;
        highAttack = false;

        if (onGuard)
            Speed = GUARD_SPEED;
        else
            Speed = DEFAULT_SPEED;
    }

    public void ToggleGuardStance() {
        if (actionCooldown)
            return;

        StartCoroutine(ActionCooldown());
        if (onGuard)
            highGuard = !highGuard;
    }
}
