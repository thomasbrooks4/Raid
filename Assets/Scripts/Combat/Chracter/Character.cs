﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Character : MonoBehaviour {

	private static float BASE_MOVE_SPEED = 0.5f;

	private Color startColor;
	private SpriteRenderer spriteRenderer;
	private BoxCollider2D boxCollider;
	private Rigidbody2D rb2D;
	private Pathfinder pathfinder;
	[SerializeField]
	private List<GridTile> path = new List<GridTile>();
	private GridTile gridTile;

	private string characterName;
	private CharacterClass characterClass;
	[SerializeField]
	private bool isMoving;
	[SerializeField]
	private bool isSelected;
	private bool onCooldown;
	private bool lowStance;

	protected int maxHealth;
	protected int health;
	protected float speed;
	protected float cooldown;

	public GridTile GridTile { get => gridTile; set => gridTile = value; }
	public List<GridTile> Path { get => path; set => path = value; }

	public string CharacterName { get => characterName; set => characterName = value; }
	public CharacterClass CharacterClass { get => characterClass; set => characterClass = value; }
	public bool IsSelected { get => isSelected; set => isSelected = value; }

	public virtual void Start() {
		spriteRenderer = GetComponent<SpriteRenderer>();
		boxCollider = GetComponent<BoxCollider2D>();
		rb2D = GetComponent<Rigidbody2D>();
		pathfinder = GetComponent<Pathfinder>();

		startColor = spriteRenderer.material.color;

		isSelected = false;
		lowStance = true;
	}

	public virtual void Update() {
		if (isMoving || onCooldown)
			return;

		if (isSelected)
			spriteRenderer.material.color = Color.yellow;
		else
			spriteRenderer.material.color = startColor;

		if (path.Any())
			moveOnPath();
	}

	void OnMouseOver() {
		spriteRenderer.material.color = Color.yellow;
	}

	void OnMouseExit() {
		if (!IsSelected)
			spriteRenderer.material.color = startColor;
	}

	public void SetTargetTile(GridTile targetTile) {
		// Move
		if (targetTile.Character == null)
			path = pathfinder.findPath(gridTile, targetTile);
	}

	public void changeStance() {
		lowStance = !lowStance;
	}

	private void moveOnPath() {
		if (!isMoving) {
			GridTile nextTile = path.First();
			path.Remove(path.First());

			StartCoroutine(actionCooldown(cooldown));
			move(nextTile);
		}
	}

	private bool move(GridTile targetTile) {
		boxCollider.enabled = false;
		RaycastHit2D hit = Physics2D.Linecast(gridTile.GridPos, targetTile.GridPos, LayerMask.GetMask("Blocking Layer"));
		boxCollider.enabled = true;

		if (hit.transform == null) {
			StartCoroutine(smoothMovement(targetTile));
			return true;
		}

		return false;
	}

	private IEnumerator smoothMovement(GridTile targetTile) {
		isMoving = true;

		float sqrRemainingDistance = (new Vector2(transform.position.x, transform.position.y) - targetTile.GridPos).sqrMagnitude;
		float inverseMoveTime = (1f / BASE_MOVE_SPEED) * speed;

		while (sqrRemainingDistance > float.Epsilon) {
			Vector2 newPosition = Vector2.MoveTowards(transform.position, targetTile.GridPos, inverseMoveTime * Time.deltaTime);
			transform.position = newPosition;
			sqrRemainingDistance = (new Vector2(transform.position.x, transform.position.y) - targetTile.GridPos).sqrMagnitude;
			yield return null;
		}

		isMoving = false;
		gridTile.Character = null;
		gridTile = targetTile;
		gridTile.Character = this;
	}

	private IEnumerator actionCooldown(float cooldown) {
		onCooldown = true;
		
		while (cooldown > 0f) {
			cooldown -= Time.deltaTime;
			yield return null;
		}

		onCooldown = false;
	}
}