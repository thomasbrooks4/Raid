using UnityEngine;

public class CombatTile : MonoBehaviour {

	private SpriteRenderer spriteRenderer;
	private Color startColor;

	private int x;
	private int y;

	// Pathfinding
	private int g;
	private int h;
	private CombatTile parent;

	public CombatTile(int x, int y) {
		this.x = x;
		this.y = y;
	}

	public int X { get => x; }
	public int Y { get => y; }

	public int G { get => g; set => g = value; }
	public int H { get => h; set => h = value; }
	public CombatTile Parent { get => parent; set => parent = value; }
	
	public int F { get => g + h; }

	void Start() {
		spriteRenderer = this.GetComponent<SpriteRenderer>();
		startColor = spriteRenderer.material.color;
	}

	void OnMouseOver() {
		spriteRenderer.material.color = Color.gray;
	}

	void OnMouseExit() {
		spriteRenderer.material.color = startColor;
	}
}
