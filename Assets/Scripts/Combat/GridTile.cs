using UnityEngine;

public class GridTile : MonoBehaviour {

	private SpriteRenderer spriteRenderer;
	private Color startColor;

	private Vector2Int gridPos;
	private CombatGrid grid;
	private Character character;

	public Vector2Int GridPos { get => gridPos; }
	public Character Character { get => character; set => character = value; }

	#region Pathfinding
	private int g;
	private int h;

	private GridTile parent;

	public int G { get => g; set => g = value; }
	public int H { get => h; set => h = value; }
	public int F { get => g + h; }

	public GridTile Parent { get => parent; set => parent = value; }
	#endregion

	public void Initialize(Vector2Int gridPos, CombatGrid grid) {
		this.gridPos = gridPos;
		this.grid = grid;
	}

	void Start() {
		spriteRenderer = GetComponent<SpriteRenderer>();
		startColor = spriteRenderer.material.color;
	}

	void OnMouseOver() {
		if (character == null && grid.CharacterSelected)
			spriteRenderer.material.color = Color.gray;
		else
			spriteRenderer.material.color = startColor;
	}

	void OnMouseExit() {
		spriteRenderer.material.color = startColor;
	}

	#region Comparator Overrides
	public override bool Equals(System.Object obj) {
		if ((obj == null || !this.GetType().Equals(obj.GetType()))) {
			return false;
		}
		else {
			GridTile t = (GridTile)obj;
			return (gridPos.x == t.GridPos.x) && (gridPos.y == t.GridPos.y);
		}
	}

	public override int GetHashCode() {
		return gridPos.x ^ gridPos.y;
	}
	#endregion
}
