public class Node
{
    private int x;
    private int y;
    private GridSystem<Node> _gridSystem;
    public Node(GridSystem<Node>gridSystem,int x,int y)
    {
        this._gridSystem = gridSystem;
        this.x = x;
        this.y = y;
    }
    
    public override string ToString()
    {
        return x + "," + y+"/n";
    }
}