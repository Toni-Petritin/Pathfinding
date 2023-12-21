using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AStar : MonoBehaviour
{
    private enum Metric
    {
        Manhattan = 0,
        Euclidean = 1,
        Diagonal = 2,
        Dijkstra = 3,
        Projection = 4,
        SoftManh = 5,
        HardEuclid = 6
    } 
    
    // Allows testing with different types of metrics.
    [SerializeField] private Metric aStarMetric;
    
    private const int width = 20;
    private const int height = 20;
    
    // These exist because CameraLocator-script needs the values and you can't pass
    // a reference to const variables.
    public int Width { get; private set; }
    public int Height { get; private set; }

    public int startX = 1;
    public int startY = 1;

    public int endX = 18;
    public int endY = 17;

    private const float Padding = .1f;

    // Time handling.
    [SerializeField] private float stepInterval = .1f;
    private float _timer;
    private bool _pathFinished;
    public bool paused = true;
    
    [SerializeField] private GameObject tileObject;

    Tile[,] _tiles = new Tile[width, height];

    // List for "unvisited" nodes.
    List<Tile> _open = new();
    // List for "visited" nodes.
    List<Tile> _closed = new();
    Tile _current;

    private void Awake()
    {
        Width = width;
        Height = height;

        for(int x = 0; x < width; x++)
        {
            for( int y = 0; y < height; y++)
            {
                GameObject temp =Instantiate(tileObject,
                    new Vector3(x * (tileObject.transform.localScale.x + Padding), 0, y * (tileObject.transform.localScale.y + Padding)),
                    tileObject.transform.rotation);
                // I didn't want to go looking for the Tile-component all the time.
                // So, I changed the tiles to be Tile type here.
                _tiles[x, y] = temp.GetComponent<Tile>();
                _tiles[x, y].x = x;
                _tiles[x, y].y = y;
                
                // I'm setting the borders non-traversable. This way we don't need to check
                // if we are going out of bounds.
                if (x == 0 || y == 0 || x == width - 1 || y == height - 1)
                {
                    _tiles[x,y].SetTraversable(false);
                    _tiles[x, y].unTouchable = true;
                }
            }
        }
    }

    private void Start()
    {
        // Start and end visuals.
        _tiles[startX, startY].GetComponent<MeshRenderer>().material.color = Color.red;
        _tiles[startX, startY].GetComponent<Tile>().unTouchable = true;
        _tiles[endX, endY].GetComponent<MeshRenderer>().material.color = Color.blue;
        _tiles[endX, endY].GetComponent<Tile>().unTouchable = true;

        InitAStar();
    }

    private void Update()
    {
        // Pause and unpause simulation.
        if (Input.GetKeyDown(KeyCode.Space))
        {
            paused = !paused;
        }

        if (!paused)
        {
            _timer += Time.deltaTime;
        }

        // Technically we only need to check if _open.Count == 0, but we had the
        // _pathFinished in class so... shrug.
        if (_pathFinished || _open.Count==0)
        {
            BackTrackRoute();
            this.enabled = false;
        }
        
        // Do single loop with each stepInterval.
        if (_timer >= stepInterval)
        {
            _pathFinished = StepAStar();
            _timer = 0;
        }
    }

    private void InitAStar()
    {
        _current = _tiles[startX, startY];
        // Set the start tile cost to 0.
        _current.SetSumCost(0);

        // Guess technically this is the only mandatory step in here anymore, but why not.
        _open.Add(_current);
    }

    private bool StepAStar()
    {
        // Find cheapest node in the open list.
        int mincost = _open.Min(t => t.SumCost);
        _current = _open.Find(t => t.SumCost == mincost);

        _open.Remove(_current);
        _closed.Add(_current);
        
        if (_current.x == endX && _current.y == endY)
        {
            return true;
        }
        
        // This is basically keeping track of the closed list. It will only leave out
        // start and end points.
        if(!_current.unTouchable)
        {
            _current.GetComponent<MeshRenderer>().material.color = Color.magenta;
        }
        
        // Go through neighbours of _current tile.
        for(int i = -1; i<= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                // Skip self.
                if (i == 0 && j == 0)
                    continue;
                
                Tile neighbour = _tiles[_current.x + i, _current.y + j];
                // Skip every tile we know already.
                if (neighbour.isTraversable && !_closed.Contains(neighbour))
                {
                    // Need temporary values as we don't yet know, if we are on the right path.
                    int tempCost = _current.Cost;
                    if (i == 0 || j == 0)
                        tempCost += 10;
                    else
                        tempCost += 14;
                    
                    // Distance never changes. We can update it directly.
                    neighbour.SetDistance(DistanceToEnd(neighbour.x, neighbour.y));
                    
                    // Same here. No permanent changes yet.
                    int tempSumCost = tempCost + neighbour.Distance;
                    bool inOpen = _open.Contains(neighbour);
                    
                    // Nested if-statements... :C
                    // Check if temp value is better than before.
                    if (tempSumCost < neighbour.SumCost || !inOpen)
                    {
                        // We can now overwrite the previous costs.
                        neighbour.SetCost(tempCost);
                        neighbour.SetSumCost(tempSumCost);
                        // We now know _current has a new Parent.
                        neighbour.Parent = _current;
                        if (!inOpen)
                        {
                            if (!neighbour.unTouchable)
                            {
                                neighbour.GetComponent<MeshRenderer>().material.color = Color.gray;
                            }
                            _open.Add(neighbour);
                        }
                    }
                }
            }
        }

        return false;
    }

    private void BackTrackRoute()
    {
        // Go through the Parent chain until we get back to start.
        Tile t = _tiles[endX, endY];
        while (t.x != startX || t.y != startY)
        {
            if(!(t.x == endX && t.y == endY))
                t.GetComponent<MeshRenderer>().material.color = Color.yellow;
            
            t = t.Parent;
        }
    }

    private int DistanceToEnd(int x, int y)
    {
        x = Mathf.Abs(endX - x);
        y = Mathf.Abs(endY - y);
        switch(aStarMetric)
        {
            case Metric.Manhattan:
                return (x + y) * 10;
            case Metric.Euclidean:
                return (int)Mathf.Sqrt(x*x + y*y) * 10;
            case Metric.Diagonal:
                return Mathf.Min(x, y) * 14 + Mathf.Abs(x-y) * 10;
            case Metric.Dijkstra:
                return 0;
            case Metric.Projection:
                return Mathf.Max(x, y) * 10;
            case Metric.SoftManh:
                return (x + y) * 8;
            case Metric.HardEuclid:
                // This exists purely as a way to create a more natural (non-optimal) path.
                return (int)Mathf.Sqrt(x*x + y*y) * 12;
            default:
                // Defaults to Dijkstra.
                return 0;
        }
    }
}
