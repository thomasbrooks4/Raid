using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Pathfinder : MonoBehaviour {

	private static int MOVE_COST_DIAGONAL = 14;
	private static int MOVE_COST_STRAIGHT = 10;

	private CombatGrid grid;

	void Start() {
		grid = GameObject.FindGameObjectWithTag("Combat Manager").GetComponent<CombatGrid>();
	}

	public List<GridTile> findPath(GridTile startTile, GridTile endTile) {
		List<GridTile> openTiles = new List<GridTile>();
		HashSet<GridTile> searchedTiles = new HashSet<GridTile>();

		startTile.G = 0;
		startTile.H = getH(startTile, endTile);
		openTiles.Add(startTile);

		while(openTiles.Count > 0) {
			GridTile currentTile = getLowestF(openTiles);
			openTiles.Remove(currentTile);
			searchedTiles.Add(currentTile);

			List<GridTile> surroundingTiles = grid.getSurroundingTiles(currentTile);

			if (surroundingTiles.Contains(endTile)) {
				endTile.Parent = currentTile;

				return createPath(startTile, endTile);
			}

			foreach (GridTile surroundingTile in surroundingTiles) {
				if (searchedTiles.Contains(surroundingTile)) {
					continue;
				}

				int gCost = currentTile.G + getMoveCost(currentTile, surroundingTile);
				if (gCost < surroundingTile.G || !openTiles.Contains(surroundingTile)) {
					surroundingTile.Parent = currentTile;
					surroundingTile.G = gCost;
					surroundingTile.H = getH(surroundingTile, endTile);

					if (!openTiles.Contains(surroundingTile)) {
						openTiles.Add(surroundingTile);
					}
				}
			}
		}

		return null;
	}

	private int getH(GridTile startTile, GridTile endTile) {
		return Math.Abs(endTile.GridPos.x - startTile.GridPos.x) + Math.Abs(endTile.GridPos.y - startTile.GridPos.y);
	}

	private GridTile getLowestF(List<GridTile> gridTiles) {
		GridTile minTile = gridTiles.First();

		foreach (GridTile tile in gridTiles) {
			if (tile.F < minTile.F) {
				minTile = tile;
			}
		}

		return minTile;
	}

	private List<GridTile> createPath(GridTile startTile, GridTile endTile) {
		List<GridTile> path = new List<GridTile>();
		GridTile currentTile = endTile;

		while (!currentTile.Equals(startTile)) {
			path.Add(currentTile);
			currentTile = currentTile.Parent;
		}
		path.Reverse();

		return path;
	}

	private int getMoveCost(GridTile startTile, GridTile endTile) {
		int distanceX = Math.Abs(endTile.GridPos.x - startTile.GridPos.x);
		int distanceY = Math.Abs(endTile.GridPos.y - startTile.GridPos.y);

		return (distanceX == distanceY) ? MOVE_COST_DIAGONAL : MOVE_COST_STRAIGHT;
	}
}
