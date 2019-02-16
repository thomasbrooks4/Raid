using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Pathfinder {

	private List<Node> grid = new List<Node>();
	private int gridCols;
	private int gridRows;

	private static int MOVE_COST_DIAGONAL = 14;
	private static int MOVE_COST_STRAIGHT = 10;

	public void initializeNodes(int cols, int rows) {
		gridCols = cols;
		gridRows = rows;

		for (int c = 0; c < cols; c++) {
			for (int r = 0; r < rows; r++) {
				grid.Add(new Node(c, r));
			}
		}
	}

	public List<Node> findPath(Vector3 startPos, Vector3 endPos) {
		List<Node> openNodes = new List<Node>();
		HashSet<Node> searchedNodes = new HashSet<Node>();

		Node startNode = getNode((int)startPos.x, (int)startPos.y);
		Node endNode = getNode((int)endPos.x, (int)endPos.y);

		startNode.G = 0;
		startNode.H = getH(startNode, endNode);

		while(openNodes.Count > 0) {
			Node activeNode = getLowestF();
			openNodes.Remove(activeNode);
			searchedNodes.Add(activeNode);

			List<Node> surroundingNodes = getSurroundingNodes(activeNode);

			if (surroundingNodes.Contains(endNode)) {
				endNode.Parent = activeNode;

				return createPath(startNode, endNode);
			}

			foreach (Node surroundingNode in surroundingNodes) {
				if (searchedNodes.Contains(surroundingNode)) {
					continue;
				}

				int gCost = activeNode.G + getMoveCost(activeNode, surroundingNode);
				if (gCost < surroundingNode.G || !openNodes.Contains(surroundingNode)) {
					surroundingNode.Parent = activeNode;
					surroundingNode.G = gCost;
					surroundingNode.H = getH(surroundingNode, endNode);

					if (!openNodes.Contains(surroundingNode)) {
						openNodes.Add(surroundingNode);
					}
				}
			}
		}

		return null;
	}

	private Node getNode(int x, int y) {
		foreach (Node node in grid) {
			if (node.X == x && node.Y == y) {
				return node;
			}
		}

		return null;
	}

	private int getH(Node startNode, Node endNode) {
		return Math.Abs(endNode.X - startNode.X) + Math.Abs(endNode.Y - startNode.Y);
	}

	private Node getLowestF() {
		Node minNode = grid.First();
		int minValue = int.MaxValue;

		foreach (Node node in grid)
			if (node.F < minValue) {
				minValue = node.F;
				minNode = node;
			}

		return minNode;
	}

	public List<Node> getSurroundingNodes(Node node) {
		List<Node> surrounding = new List<Node>();

		for (int x = -1; x <= 1; x++) {
			for (int y = -1; y <= 1; y++) {
				if (x == 0 && y == 0)
					continue;

				int checkX = node.X + x;
				int checkY = node.Y + y;

				if (0 <= checkX && checkX < gridCols && 0 <= checkY && checkY < gridRows)
					surrounding.Add(getNode(checkX, checkY));
			}
		}

		return surrounding;
	}

	private List<Node> createPath(Node startNode, Node endNode) {
		List<Node> path = new List<Node>();
		Node currentNode = endNode;

		while (!currentNode.Equals(startNode)) {
			path.Add(currentNode);
			currentNode = currentNode.Parent;
		}
		path.Reverse();

		return path;
	}

	private int getMoveCost(Node startNode, Node endNode) {
		int distanceX = Math.Abs(endNode.X - startNode.X);
		int distanceY = Math.Abs(endNode.Y - startNode.Y);

		return (distanceX == distanceY) ? MOVE_COST_DIAGONAL : MOVE_COST_STRAIGHT;
	}
}
