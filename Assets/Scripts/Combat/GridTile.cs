using UnityEngine;

public class GridTile : MonoBehaviour {

	private SpriteRenderer spriteRenderer;
	private Color startColor;
	private Color highlightColor;

    public CombatGrid Grid { get; private set; }
    public Vector2Int GridPos { get; private set; }
    public Character Character { get; set; }

    #region Pathfinding

    public int G { get; set; }
    public int H { get; set; }
    public int F { get => G + H; }

    public GridTile Parent { get; set; }
    #endregion

    public void Initialize(Vector2Int gridPos, CombatGrid grid) {
		this.GridPos = gridPos;
		this.Grid = grid;
	}

	void Start() {
		spriteRenderer = GetComponent<SpriteRenderer>();
		startColor = spriteRenderer.material.color;
		highlightColor = new Color(startColor.r, startColor.g, startColor.b, 1.2f);
	}

	void OnMouseOver() {
		if (Character == null && Grid.CharacterSelected)
			spriteRenderer.material.color = highlightColor;
		else
			spriteRenderer.material.color = startColor;
	}

	void OnMouseExit() {
		spriteRenderer.material.color = startColor;
	}

	#region Comparator Overrides
	public override bool Equals(object other) {
		if ((other == null || !this.GetType().Equals(other.GetType()))) {
			return false;
		}
		else {
			GridTile t = (GridTile)other;
			return (GridPos.x == t.GridPos.x) && (GridPos.y == t.GridPos.y);
		}
	}

    public override int GetHashCode() {
        return GridPos.x ^ GridPos.y;
    }
    #endregion
}
