public class Node
{
    private int x;
    private int y;
    private Grid _grid;
    public Node(Grid grid,int x,int y)
    {
        _grid = grid;
        this.x = x;
        this.y = y;
    }
    
    public override string ToString()
    {
        return x + "," + y+"/n";
    }
}