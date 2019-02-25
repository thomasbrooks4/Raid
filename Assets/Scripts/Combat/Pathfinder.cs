using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Pathfinder : MonoBehaviour {

	private const int MOVE_COST_DIAGONAL = 14;
	private const int MOVE_COST_STRAIGHT = 10;

	private CombatGrid grid;

	void Start() {
		grid = GameObject.FindGameObjectWithTag("Combat Manager").GetComponent<CombatGrid>();
	}

	public List<GridTile> FindPath(GridTile startTile, GridTile endTile) {
		List<GridTile> openTiles = new List<GridTile>();
		HashSet<GridTile> searchedTiles = new HashSet<GridTile>();

		startTile.G = 0;
		startTile.H = GetH(startTile, endTile);
		openTiles.Add(startTile);

		while(openTiles.Count > 0) {
			GridTile currentTile = GetLowestF(openTiles);
			openTiles.Remove(currentTile);
			searchedTiles.Add(currentTile);

			List<GridTile> surroundingTiles = grid.GetSurroundingTiles(currentTile);

			if (surroundingTiles.Contains(endTile)) {
				endTile.Parent = currentTile;

				return CreatePath(startTile, endTile);
			}

			foreach (GridTile surroundingTile in surroundingTiles) {
				if (surroundingTile.Character != null || searchedTiles.Contains(surroundingTile)) {
					continue;
				}

				int gCost = currentTile.G + GetMoveCost(currentTile, surroundingTile);
				if (gCost < surroundingTile.G || !openTiles.Contains(surroundingTile)) {
					surroundingTile.Parent = currentTile;
					surroundingTile.G = gCost;
					surroundingTile.H = GetH(surroundingTile, endTile);

					if (!openTiles.Contains(surroundingTile)) {
						openTiles.Add(surroundingTile);
					}
				}
			}
		}

		return null;
	}

    public List<GridTile> FindPathToNearestTile(GridTile startTile, GridTile endTile) {
        List<GridTile> path = FindPath(startTile, endTile);
        path.RemoveAt(path.Count - 1);

        return path;
    }

    private int GetH(GridTile startTile, GridTile endTile) {
		return Math.Abs(endTile.GridPos.x - startTile.GridPos.x) + Math.Abs(endTile.GridPos.y - startTile.GridPos.y);
	}

	private GridTile GetLowestF(List<GridTile> gridTiles) {
		GridTile minTile = gridTiles.First();

		foreach (GridTile tile in gridTiles) {
			if (tile.F < minTile.F) {
				minTile = tile;
			}
		}

		return minTile;
	}

	private List<GridTile> CreatePath(GridTile startTile, GridTile endTile) {
		List<GridTile> path = new List<GridTile>();
		GridTile currentTile = endTile;

		while (!currentTile.Equals(startTile)) {
			path.Add(currentTile);
			currentTile = currentTile.Parent;
		}
		path.Reverse();

		return path;
	}

	private int GetMoveCost(GridTile startTile, GridTile endTile) {
		int distanceX = Math.Abs(endTile.GridPos.x - startTile.GridPos.x);
		int distanceY = Math.Abs(endTile.GridPos.y - startTile.GridPos.y);

		return (distanceX == distanceY) ? MOVE_COST_DIAGONAL : MOVE_COST_STRAIGHT;
	}
}
