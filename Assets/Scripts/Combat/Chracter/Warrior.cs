using UnityEngine;

public class Warrior : Character {

    private const int DEFAULT_HEALTH = 125;
    private const int DEFAULT_DAMAGE = 35;
    private const float DEFAULT_SPEED = 0.9f;
    private const int DEFAULT_ATTACK_RANGE = 1;
    private const float GUARD_SPEED = 0.5f;
    private const float COOLDOWN = 0.2f;

    private bool onGuard;
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
        speed = DEFAULT_SPEED;
        attackRange = DEFAULT_ATTACK_RANGE;
        cooldown = COOLDOWN;

        onGuard = false;
        highGuard = false;
    }

    // Update is called once per frame
    public override void Update() {
        base.Update();

        if (attackCooldown || actionCooldown)
            return;

        if (Input.GetKeyDown(KeyCode.G))
            ToggleGuard();
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

            // TODO: Cleanup for if enemy is facing
            if (enemy.OnGuard) {
                if (highAttack)
                    if (!enemy.HighGuard)
                        enemy.TakeDamage(damage);
                    else
                    if (enemy.HighGuard)
                        enemy.TakeDamage(damage);
            }
            else {
                enemy.TakeDamage(damage);
            }
        }
    }

    public override void ToggleAttackStance() {
        highAttack = !highAttack;

        if (onGuard) {
            if (highAttack && highGuard)
                highGuard = false;

            if (!highAttack && !highGuard)
                highGuard = true;
        }
    }

    private void ToggleGuard() {
        onGuard = !onGuard;

        if (onGuard)
            speed = GUARD_SPEED;
        else
            speed = DEFAULT_SPEED;
    }
}
