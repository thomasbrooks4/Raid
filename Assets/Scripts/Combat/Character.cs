using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Character : MonoBehaviour {
	
	public LayerMask blockingLayer;
	private BoxCollider2D boxCollider;
	private Rigidbody2D rb2D;

	private string characterName;
	private bool isAlive;
	private bool isMoving;
	private bool onCooldown;

	[SerializeField]
	private float moveSpeed;
	[SerializeField]
	private float cooldown;

	private int maxHealth = 100;
	private int health;

	public Character(string characterName) {
		this.characterName = characterName;

		isAlive = true;
		isMoving = false;
		health = maxHealth;
	}

	// Start is called before the first frame update
	void Start() {
		boxCollider = GetComponent<BoxCollider2D>();
		rb2D = GetComponent<Rigidbody2D>();
	}

	// Update is called once per frame
	void Update() {
		if (isMoving || onCooldown)
			return;

		//To store move directions.
		int horizontal = 0;
		int vertical = 0;

		//To get move directions
		horizontal = (int)(Input.GetAxisRaw("Horizontal"));
		vertical = (int)(Input.GetAxisRaw("Vertical"));

		//If there's a direction, we are trying to move.
		if (horizontal != 0 || vertical != 0) {
			StartCoroutine(actionCooldown(cooldown));
			Move(horizontal, vertical);
		}
	}

	private bool Move(int xDir, int yDir) {
		Vector2 startTile = transform.position;
		Vector2 endTile = startTile + new Vector2(xDir, yDir);

		boxCollider.enabled = false;
		RaycastHit2D hit = Physics2D.Linecast(startTile, endTile, blockingLayer);
		boxCollider.enabled = true;

		if (hit.transform == null) {
			StartCoroutine(SmoothMovement(endTile));
			return true;
		}

		return false;
	}

	private IEnumerator SmoothMovement(Vector3 endPosition) {
		isMoving = true;

		float sqrRemainingDistance = (transform.position - endPosition).sqrMagnitude;
		float inverseMoveTime = 1f / moveSpeed;

		while (sqrRemainingDistance > float.Epsilon) {
			Vector3 newPosition = Vector3.MoveTowards(transform.position, endPosition, inverseMoveTime * Time.deltaTime);
			transform.position = newPosition;
			sqrRemainingDistance = (transform.position - endPosition).sqrMagnitude;

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

	private TileBase getTile(Tilemap tilemap, Vector2 tileWorldPosition) {
		return tilemap.GetTile(tilemap.WorldToCell(tileWorldPosition));
	}
}
