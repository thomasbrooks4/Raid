using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour {

	private static float BASE_MOVE_SPEED = 0.5f;

	private Color startColor;
	private SpriteRenderer spriteRenderer;
	private BoxCollider2D boxCollider;
	private Rigidbody2D rb2D;
	private Pathfinder pathfinder;
	private List<GridTile> path = new List<GridTile>();
	private GridTile gridTile;

	private string characterName;
	private bool isMoving;
	[SerializeField]
	private bool isSelected;
	private bool onCooldown;

	[SerializeField]
	private float speed;
	[SerializeField]
	private float cooldown;

	public GridTile GridTile { get => gridTile; set => gridTile = value; }
	public List<GridTile> Path { get => path; set => path = value; }
	public bool IsSelected { get => isSelected; set => isSelected = value; }

	public void Initialize(GridTile gridTile) {
		this.gridTile = gridTile;
		isSelected = false;
	}

	void Start() {
		spriteRenderer = GetComponent<SpriteRenderer>();
		boxCollider = GetComponent<BoxCollider2D>();
		rb2D = GetComponent<Rigidbody2D>();
		pathfinder = GetComponent<Pathfinder>();

		startColor = spriteRenderer.material.color;
	}
	
	void Update() {
		if (isMoving || onCooldown)
			return;

		if (isSelected) {
			spriteRenderer.material.color = Color.yellow;

			// Move to tile
			// Get tile selected
			// Check for character or empty
			// if (character)
			// get path
			// set target character
			// begin movement
			// else
			// get path
			// begin movement
		}
		else {
			spriteRenderer.material.color = startColor;
		}

		// if (!path.IsEmpty())
		// MoveOnPath()

		int horizontal = 0;
		int vertical = 0;

		horizontal = (int)(Input.GetAxisRaw("Horizontal"));
		vertical = (int)(Input.GetAxisRaw("Vertical"));
		
		if (horizontal != 0 || vertical != 0) {
			StartCoroutine(actionCooldown(cooldown));
			move(horizontal, vertical);
		}
	}

	public void SetTargetTile(GridTile targetTile) {
		if (targetTile.Character != null) {
			// Move and attack
		}
		else {
			// Move
			path = pathfinder.findPath(gridTile, targetTile);
		}
	}

	private void moveOnPath() {
		if (!isMoving) {
			// update path
			bool moved; // = Move(path.First());
			// Remove tile from path
		}
	}

	private bool move(int xDir, int yDir) {
		Vector2 startTile = transform.position;
		Vector2 endTile = startTile + new Vector2(xDir, yDir);

		boxCollider.enabled = false;
		RaycastHit2D hit = Physics2D.Linecast(startTile, endTile, LayerMask.GetMask("Blocking Layer"));
		boxCollider.enabled = true;

		if (hit.transform == null) {
			StartCoroutine(smoothMovement(endTile));
			return true;
		}

		return false;
	}

	private IEnumerator smoothMovement(Vector3 endPos) {
		isMoving = true;

		float sqrRemainingDistance = (transform.position - endPos).sqrMagnitude;
		float inverseMoveTime = (1f / BASE_MOVE_SPEED) * speed;

		while (sqrRemainingDistance > float.Epsilon) {
			Vector3 newPosition = Vector3.MoveTowards(transform.position, endPos, inverseMoveTime * Time.deltaTime);
			transform.position = newPosition;
			sqrRemainingDistance = (transform.position - endPos).sqrMagnitude;

			yield return null;
		}

		isMoving = false;
	}

	private IEnumerator actionCooldown(float cooldown) {
		onCooldown = true;
		
		while (cooldown > 0f) {
			cooldown -= Time.deltaTime;
			yield return null;
		}

		onCooldown = false;
	}

	void OnMouseOver() {
		spriteRenderer.material.color = Color.yellow;
	}

	void OnMouseExit() {
		spriteRenderer.material.color = startColor;
	}
}
