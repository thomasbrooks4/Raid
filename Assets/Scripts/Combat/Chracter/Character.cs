﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class Character : MonoBehaviour {

    private const float BASE_MOVE_SPEED = 0.5f;
    private const float BASE_ATTACK_SPEED = 3f;

    private SpriteRenderer spriteRenderer;
    private BoxCollider2D boxCollider;
    private Rigidbody2D rb2D;
    private Pathfinder pathfinder;
    private Color startColor;

    [SerializeField]
    private List<GridTile> path = new List<GridTile>();
    [SerializeField]
    private Vector2Int currentDirection;
    [SerializeField]
    private Vector2Int targetDirection;
    [SerializeField]
    protected GridTile currentTile;
    [SerializeField]
    protected Character target;

    [SerializeField]
    protected bool friendly;
    [SerializeField]
    protected bool isAlive;
    [SerializeField]
    protected bool isSelected;
    [SerializeField]
    protected bool isMoving;
    [SerializeField]
    protected bool attackCooldown;
    [SerializeField]
    protected bool highAttack;
    [SerializeField]
    protected bool directionLocked;
    [SerializeField]
    protected bool focusLocked;

    protected string characterName;
    protected CharacterClass characterClass;
    protected int maxHealth;
    [SerializeField]
    protected int health;
    protected int damage;
    protected float speed;
    protected int attackRange;
    protected float cooldown;

    public List<GridTile> Path { get => path; set => path = value; }
    public GridTile GridTile { get => currentTile; set => currentTile = value; }
    public Character Target { get => target; set => target = value; }

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

        if (friendly)
            currentDirection = new Vector2Int(1, 0);
        else
            currentDirection = new Vector2Int(-1, 0);
        targetDirection = currentDirection;

        isAlive = true;
        isSelected = false;
        isMoving = false;
        attackCooldown = false;
        highAttack = false;
        directionLocked = false;
        focusLocked = false;
    }

    public virtual void Update() {
        if (isAlive) {
            if (isSelected)
                spriteRenderer.material.color = Color.yellow;
            else
                spriteRenderer.material.color = startColor;

            if (isMoving)
                return;

            if (currentDirection != targetDirection) {
                RotateTowardsTargetDirection();
            }
            // TODO: Keep in mind that archer's should move only until ABLE to attack (typically)
            else if (path.Any())
                Move();
            else {
                if (target != null) {
                    if (WithinAttackRange(target.GridTile)) {
                        if (!attackCooldown)
                            StartAttacking();
                    }
                    else
                        path = pathfinder.FindPathToNearestTile(currentTile, target.GridTile);
                }
            }
        }
    }

    #region Highlighting
    void OnMouseOver() {
        if (isAlive && friendly)
            spriteRenderer.material.color = Color.yellow;
    }

    void OnMouseExit() {
        if (isAlive && !IsSelected)
            spriteRenderer.material.color = startColor;
    }
    #endregion

    #region Movment

    private bool Move() {
        GridTile nextTile = path.First();

        if (!IsAdjacent(nextTile)) {
            path.InsertRange(0, pathfinder.FindPathToNearestTile(currentTile, nextTile));

            return false;
        }

        if (nextTile.Character == null) {
            path.Remove(path.First());

            if (!IsFacing(nextTile)) {
                targetDirection = nextTile.GridPos - currentTile.GridPos;

                while (currentDirection != targetDirection) {
                    RotateTowardsTargetDirection();
                }
            }

            StartCoroutine(Moving(nextTile));

            return true;
        }

        // Find new path
        path = pathfinder.FindPath(currentTile, path.Last());

        return false;
    }

    private IEnumerator Moving(GridTile targetTile) {
        isMoving = true;
        targetTile.Character = this;

        float sqrRemainingDistance = (GetTransfromPostionVector2() - targetTile.GridPos).sqrMagnitude;

        while (sqrRemainingDistance > float.Epsilon) {
            float inverseMoveTime = (1f / BASE_MOVE_SPEED) * speed;

            Vector2 newPosition = Vector2.MoveTowards(transform.position, targetTile.GridPos, inverseMoveTime * Time.deltaTime);
            transform.position = newPosition;
            sqrRemainingDistance = (GetTransfromPostionVector2() - targetTile.GridPos).sqrMagnitude;

            yield return null;
        }

        // TODO: Potentially move this to the top so characters can move into current tile earlier
        currentTile.Character = null;
        currentTile = targetTile;

        if (!path.Any() && target != null)
            targetDirection = target.GridTile.GridPos - currentTile.GridPos;

        isMoving = false;
    }
    #endregion

    #region Attacking
    private void StartAttacking() {
        if (target.IsAlive) {
            // Check if facing target
            if (IsFacing(target.GridTile)) {
                Attack();
            }
            else {
                if (!directionLocked) {
                    // TODO: Only works for adjacent targets
                    targetDirection = target.GridTile.GridPos - currentTile.GridPos;
                }
            }
        }
        else
            target = null;
    }

    private bool Attack() {
        // Possibilty to start attack
        if (AttemptAttack()) {
            StartCoroutine(AttackCooldown());
            AttemptDamage();
            return true;
        }

        return false;
    }

    protected abstract bool AttemptAttack();
    protected abstract void AttemptDamage();
    public abstract void ToggleAttackStance();

    private IEnumerator AttackCooldown() {
        attackCooldown = true;

        float cooldownTime = BASE_ATTACK_SPEED * speed;

        while (cooldownTime > 0f) {
            cooldownTime -= Time.deltaTime;
            yield return null;
        }

        attackCooldown = false;
    }
    #endregion

    public void SetTargetTile(GridTile targetTile) {
        if (targetTile.Character != null) {
            if (IsEnemy(targetTile.Character)) {
                if (targetTile.Character.IsAlive)
                    target = targetTile.Character;

                Vector2Int distance = targetTile.GridPos - currentTile.GridPos;
                if (Mathf.Abs(distance.x) > attackRange || Mathf.Abs(distance.y) > attackRange) {
                    path = pathfinder.FindPathToNearestTile(currentTile, target.GridTile);
                }
            }
        }
        else {
            if (!focusLocked)
                target = null;
            path = pathfinder.FindPath(currentTile, targetTile);
        }
    }

    public void QueueLeftRotation() {
        targetDirection = RotateLeft(targetDirection);
    }

    public void QueueRightRotation() {
        targetDirection = RotateRight(targetDirection);
    }

    public void ToggleDirectionLocked() {
        directionLocked = !directionLocked;
    }

    public void ToggleFocusLocked() {
        focusLocked = !focusLocked;
    }

    public void ResetDirection() {
        if (directionLocked)
            return;

        if (friendly)
            targetDirection = new Vector2Int(1, 0);
        else
            targetDirection = new Vector2Int(-1, 0);
    }

    public void TakeDamage(int damage) {
        health -= damage;

        if (health <= 0) {
            health = 0;
            isAlive = false;
            spriteRenderer.material.color = startColor;
        }
    }

    public bool IsEnemy(Character character) {
        return friendly ? !character.Friendly : character.Friendly;
    }

    public bool IsFacing(GridTile targetTile) {
        if (IsAdjacent(targetTile)) {
            Vector2Int direction = targetTile.GridPos - currentTile.GridPos;

            return currentDirection == direction;
        }

        return false;
    }

    #region Helper Methods
    private Vector2Int RotateLeft(Vector2Int direction) {
        int x, y;

        if (direction.x != -direction.y)
            x = direction.x + -direction.y;
        else
            x = -direction.y;

        if (direction.y != direction.x)
            y = direction.y + direction.x;
        else
            y = direction.x;

        return new Vector2Int(x, y);
    }

    private Vector2Int RotateRight(Vector2Int direction) {
        int x, y;

        if (direction.x != direction.y)
            x = direction.x + direction.y;
        else
            x = direction.y;

        if (direction.y != -direction.x)
            y = direction.y + -direction.x;
        else
            y = -direction.x;

        return new Vector2Int(x, y);
    }

    private void RotateTowardsTargetDirection() {
        // Find quickest direction to rotate
        int leftCount = 0, rightCount = 0;

        Vector2Int direction = currentDirection;
        while (direction != targetDirection) {
            direction = RotateLeft(direction);
            leftCount++;
        }

        direction = currentDirection;
        while (direction != targetDirection) {
            direction = RotateRight(direction);
            rightCount++;
        }

        // Friendly characters default rotating right
        if (friendly) {
            if (leftCount < rightCount)
                currentDirection = RotateLeft(currentDirection);
            else
                currentDirection = RotateRight(currentDirection);
        }
        else {
            if (rightCount < leftCount)
                currentDirection = RotateRight(currentDirection);
            else
                currentDirection = RotateLeft(currentDirection);
        }
    }

    private bool WithinAttackRange(GridTile tile) {
        Vector2Int distance = tile.GridPos - currentTile.GridPos;
        return Mathf.Abs(distance.x) <= attackRange && Mathf.Abs(distance.y) <= attackRange;
    }

    private bool IsAdjacent(GridTile tile) {
        return IsDirection(tile.GridPos - currentTile.GridPos);
    }

    private bool IsDirection(Vector2Int direction) {
        return (-1 <= direction.x && direction.x <= 1 && -1 <= direction.y && direction.y <= 1);
    }

    private Vector2 GetTransfromPostionVector2() {
        return new Vector2(transform.position.x, transform.position.y);
    }
    #endregion
}
