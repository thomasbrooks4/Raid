using System;

public class Node {

	private int x;
	private int y;

	private int g;
	private int h;

	private Node parent;

	public Node(int x, int y) {
		this.x = x;
		this.y = y;
	}

	public int X { get => x; }
	public int Y { get => y; }

	public int G { get => g; set => g = value; }
	public int H { get => h; set => h = value; }
	public int F { get => g + h; }

	public Node Parent { get => parent; set => parent = value; }

	public override bool Equals(Object obj) {
		if ((obj == null || !this.GetType().Equals(obj.GetType()))) {
			return false;
		}
		else {
			Node n = (Node)obj;
			return (x == n.X) && (y == n.Y);
		}
	}
}
