using System.Collections;
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

	protected List<GridTile> path = new List<GridTile>();
	[SerializeField]
	private Vector2Int facingDirection;
	private GridTile currentTile;
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

	public GridTile GridTile { get => currentTile; set => currentTile = value; }

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
		isSelected = false;
		isMoving = false;
		attackCooldown = false;
		highAttack = false;
		directionLocked = false;
		focusLocked = false;
	}

	public virtual void Update() {
		if (isSelected)
			spriteRenderer.material.color = Color.yellow;
		else
			spriteRenderer.material.color = startColor;

		if (isMoving || attackCooldown)
			return;

		if (path.Any())
			Move();
		else if (target != null)
			StartAttacking();
	}

	#region Highlighting
	void OnMouseOver() {
		if (friendly)
			spriteRenderer.material.color = Color.yellow;
	}

	void OnMouseExit() {
		if (!IsSelected)
			spriteRenderer.material.color = startColor;
	}
	#endregion

	#region Movment

	private bool Move() {
		GridTile nextTile = path.First();

		if (nextTile.Character == null) {
			path.Remove(path.First());

			if (!directionLocked)
				facingDirection = nextTile.GridPos - currentTile.GridPos;

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

		if (target != null)
			FaceTarget();

		isMoving = false;
	}
	#endregion

	#region Attacking
	private void StartAttacking() {
		if (target.IsAlive) {
			// Check if range to target
			Vector2Int distance = target.GridTile.GridPos - currentTile.GridPos;
			if (Mathf.Abs(distance.x) > attackRange || Mathf.Abs(distance.y) > attackRange) {
				SetPathToNearestEmptyTile();

				return;
			}

			// Check if facing target
			if (currentTile.GridPos + facingDirection == target.GridTile.GridPos) {
				if (!directionLocked)
					FaceTarget();
				Attack();
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
					SetPathToNearestEmptyTile();
				}
			}
		}
		else {
			if (!focusLocked)
				target = null;
			path = pathfinder.FindPath(currentTile, targetTile);
		}
	}

	public void RotateLeft() {
		int x, y;

		if (facingDirection.x != -facingDirection.y)
			x = facingDirection.x + -facingDirection.y;
		else
			x = -facingDirection.y;

		if (facingDirection.y != facingDirection.x)
			y = facingDirection.y + facingDirection.x;
		else
			y = facingDirection.x;

		facingDirection = new Vector2Int(x, y);
	}

	public void RotateRight() {
		int x, y;

		if (facingDirection.x != facingDirection.y)
			x = facingDirection.x + facingDirection.y;
		else
			x = facingDirection.y;

		if (facingDirection.y != -facingDirection.x)
			y = facingDirection.y + -facingDirection.x;
		else
			y = -facingDirection.x;

		facingDirection = new Vector2Int(x, y);
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

	#region Helper Methods
	private bool IsEnemy(Character character) {
		if (friendly)
			return !character.Friendly;
		else
			return character.Friendly;
	}

	private void SetPathToNearestEmptyTile() {
		path = pathfinder.FindPath(currentTile, target.GridTile);
		path.RemoveAt(path.Count - 1);
	}

	// TODO: Get rounded direction based on target location
	private void FaceTarget() {
		if (!path.Any() && target != null) {
			Vector2Int newDirection = target.GridTile.GridPos - currentTile.GridPos;

			if (!directionLocked && IsDirection(newDirection)) {
				facingDirection = newDirection;
			}
		}
	}

	private bool IsDirection(Vector2Int direction) {
		return (-1 <= direction.x && direction.x <= 1 && -1 <= direction.y && direction.y <= 1);
	}

	private Vector2 GetTransfromPostionVector2() {
		return new Vector2(transform.position.x, transform.position.y);
	}
	#endregion
}
