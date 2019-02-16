using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridTile : MonoBehaviour {

	private SpriteRenderer spriteRenderer;
	private Color startColor;

	private Vector2Int gridPos;
	private CombatGrid grid;
	private Character character;

	public Character Character { get => character; set => character = value; }

	public void Initialize(Vector2Int gridPos, CombatGrid grid) {
		this.gridPos = gridPos;
		this.grid = grid;
	}

	void Start() {
		spriteRenderer = GetComponent<SpriteRenderer>();
		startColor = spriteRenderer.material.color;
	}

	void OnMouseOver() {
		if (character == null)
			spriteRenderer.material.color = Color.gray;
	}

	void OnMouseExit() {
		spriteRenderer.material.color = startColor;
	}
}
