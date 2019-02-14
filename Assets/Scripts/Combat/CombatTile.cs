public class CombatTile {

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
}
