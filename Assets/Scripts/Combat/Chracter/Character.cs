using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class Character : MonoBehaviour {

    private const float BASE_MOVE_SPEED = 0.5f;
    private const float BASE_ATTACK_SPEED = 0.75f;

    private SpriteRenderer spriteRenderer;
    private BoxCollider2D boxCollider;
    private Rigidbody2D rb2D;
    private Pathfinder pathfinder;
    private Color startColor;

    private List<GridTile> path = new List<GridTile>();
    private Vector2Int facingDirection;
    private GridTile gridTile;
    protected Character target;

    private bool friendly;
    private bool isAlive;
    private bool isSelected;
    private bool isMoving;
    protected bool isAttacking;
    protected bool onCooldown;
    protected bool highAttack;
    protected bool directionLocked;

    protected string characterName;
    protected CharacterClass characterClass;
    protected int maxHealth;
    protected int health;
    protected int damage;
    protected float speed;
    protected int attackRange;
    protected float cooldown;

    public GridTile GridTile { get => gridTile; set => gridTile = value; }

    public bool Friendly { get => friendly; set => friendly = value; }
    public bool IsAlive { get => isAlive; set => isAlive = value; }
    public bool IsSelected { get => isSelected; set => isSelected = value; }
    public bool HighAttack { get => highAttack; set => highAttack = value; }

    public string CharacterName { get => characterName; set => characterName = value; }
    public CharacterClass CharacterClass { get => characterClass; set => characterClass = value; }
    protected int Health { get => health; set => health = value; }

    public virtual void Start() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();
        rb2D = GetComponent<Rigidbody2D>();
        pathfinder = GetComponent<Pathfinder>();

        startColor = spriteRenderer.material.color;

        isAlive = true;
        isMoving = false;
        isSelected = false;
        onCooldown = false;
        highAttack = false;
        directionLocked = false;
    }

    public virtual void Update() {
        if (isSelected)
            spriteRenderer.material.color = Color.yellow;
        else
            spriteRenderer.material.color = startColor;

        if (isMoving || isAttacking || onCooldown)
            return;

        if (path.Any())
            StartMoving();
        else if (target != null)
            StartAttacking();
    }

    #region Highlighting
    void OnMouseOver() {
        spriteRenderer.material.color = Color.yellow;
    }

    void OnMouseExit() {
        if (!IsSelected)
            spriteRenderer.material.color = startColor;
    }
    #endregion

    #region Movment
    private void StartMoving() {
        GridTile nextTile = path.First();
        path.Remove(path.First());

        StartCoroutine(StartActionCooldown());
        Move(nextTile);
    }

    private bool Move(GridTile targetTile) {
        // TODO: Fix bug where two sprites can travel into same tile then get stuck
        boxCollider.enabled = false;
        RaycastHit2D hit = Physics2D.Linecast(gridTile.GridPos, targetTile.GridPos, LayerMask.GetMask("Blocking Layer"));
        boxCollider.enabled = true;

        if (hit.transform == null) {
            facingDirection = targetTile.GridPos - gridTile.GridPos;

            StartCoroutine(Moving(targetTile));
            return true;
        }

        return false;
    }

    private IEnumerator Moving(GridTile targetTile) {
        // TODO: Move animation
        isMoving = true;

        float sqrRemainingDistance = (GetTransfromPostionVector2() - targetTile.GridPos).sqrMagnitude;

        while (sqrRemainingDistance > float.Epsilon) {
            float inverseMoveTime = (1f / BASE_MOVE_SPEED) * speed;

            Vector2 newPosition = Vector2.MoveTowards(transform.position, targetTile.GridPos, inverseMoveTime * Time.deltaTime);
            transform.position = newPosition;
            sqrRemainingDistance = (GetTransfromPostionVector2() - targetTile.GridPos).sqrMagnitude;

            yield return null;
        }

        isMoving = false;
        gridTile.Character = null;
        gridTile = targetTile;
        gridTile.Character = this;
    }
    #endregion

    #region Attacking
    private void StartAttacking() {
        if (target.IsAlive) {
            // Check if range to target
            Vector2Int distance = target.GridTile.GridPos - gridTile.GridPos;
            if (Mathf.Abs(distance.x) > attackRange || Mathf.Abs(distance.y) > attackRange) {
                // Set path to nearest empty tile to enemy
                return;
            }

            // TODO: Check if facing target

            StartCoroutine(StartActionCooldown());
            Attack();
        }
        else
            target = null;
    }

    private bool Attack() {
        // Possibilty to start attack
        if (AttemptAttack()) {
            StartCoroutine(Attacking());
            AttemptDamage();

            return true;
        }

        return false;
    }

    protected abstract bool AttemptAttack();
    protected abstract void AttemptDamage();
    public abstract void ToggleAttackStance();

    private IEnumerator Attacking() {
        // TODO: Attack animation
        isAttacking = true;

        float attackCooldown = BASE_ATTACK_SPEED * speed;

        while (attackCooldown > 0f) {
            attackCooldown -= Time.deltaTime;
            yield return null;
        }

        isAttacking = false;
    }
    #endregion

    public void SetTargetTile(GridTile targetTile) {
        if (targetTile.Character != null) {
            target = targetTile.Character;

            Vector2Int distance = targetTile.GridPos - gridTile.GridPos;
            if (Mathf.Abs(distance.x) > attackRange || Mathf.Abs(distance.y) > attackRange) {
                // TODO: Set path to nearest empty tile to enemy
            }
        }
        else {
            // TODO: Add ability to remain locked on?
            target = null;
            path = pathfinder.FindPath(gridTile, targetTile);
        }
    }

    public void ToggleDirectionLocked() {
        directionLocked = !directionLocked;
    }

    public void ResetDirection() {
        if (directionLocked)
            return;

        if (friendly)
            facingDirection = new Vector2Int(1, 0);
        else
            facingDirection = new Vector2Int(-1, 0);
    }

    public void TakeDamage(int damage) {
        health -= damage;

        if (health <= 0) {
            health = 0;
            isAlive = false;
        }
    }

    private IEnumerator StartActionCooldown() {
        onCooldown = true;

        float cooldownTime = cooldown;

        while (cooldownTime > 0f) {
            cooldownTime -= Time.deltaTime;
            yield return null;
        }

        onCooldown = false;
    }

    #region Helper Methods
    private Vector2 GetTransfromPostionVector2() {
        return new Vector2(transform.position.x, transform.position.y);
    }
    #endregion
}
