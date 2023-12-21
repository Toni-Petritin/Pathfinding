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
        Dijkstra = 3
    } 
    
    [SerializeField] private Metric aStarMetric;
    
    private const int width = 20;
    private const int height = 20;
    
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
                _tiles[x, y] = temp.GetComponent<Tile>();
                _tiles[x, y].x = x;
                _tiles[x, y].y = y;
                if (x == 0 || y == 0 || x == width - 1 || y == height - 1)
                {
                    _tiles[x,y].SetTraversable(false);
                }
            }
        }
    }

    private void Start()
    {
        _tiles[startX, startY].GetComponent<MeshRenderer>().material.color = Color.red;
        _tiles[startX, startY].GetComponent<Tile>().unTouchable = true;
        _tiles[endX, endY].GetComponent<MeshRenderer>().material.color = Color.blue;
        _tiles[endX, endY].GetComponent<Tile>().unTouchable = true;

        InitAStar();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            paused = !paused;
        }

        if (!paused)
        {
            _timer += Time.deltaTime;
        }

        if (_pathFinished || _open.Count==0)
        {
            BackTrackRoute();
            this.enabled = false;
        }
        
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

        _open.Add(_current);
    }

    private bool StepAStar()
    {
        
        float mincost = _open.Min(t => t.SumCost);
        _current = _open.Find(t => t.SumCost == mincost);

        _open.Remove(_current);
        _closed.Add(_current);
        
        if (_current.x == endX && _current.y == endY)
        {
            return true;
        }
        
        if(!_current.unTouchable)
        {
            _current.GetComponent<MeshRenderer>().material.color = Color.magenta;
        }
        
        // Go through neighbours
        for(int i = -1; i<= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0)
                    continue;
                
                Tile neighbour = _tiles[_current.x + i, _current.y + j];
                if (neighbour.isTraversable && !_closed.Contains(neighbour))
                {
                    if (i == 0 || j == 0)
                        neighbour.SetCost(_current.Cost + 10);
                    else
                        neighbour.SetCost(_current.Cost + 14);
                    
                    neighbour.SetDistance(DistanceToEnd(neighbour.x, neighbour.y));
                    int tempSumCost = neighbour.Cost + neighbour.Distance;
                    bool inOpen = _open.Contains(neighbour);
                    
                    if (tempSumCost < neighbour.SumCost || !inOpen)
                    {
                        neighbour.SetSumCost(tempSumCost);
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
                
                // Tile neighbour = _open.Find(t => t.x == _current.x + i && t.y == _current.y + j);
                //
                // if (neighbour == null || !neighbour.isTraversable)
                //     continue;
                // if (!(neighbour.x == endX && neighbour.y == endY))
                //     neighbour.GetComponent<MeshRenderer>().material.color = Color.gray;
                //
                // // Compute the cost
                // int cost = _current.SumCost;
                // float toEndCost = DistanceToEnd(neighbour.x, neighbour.y);
                // float sumCost = cost + toEndCost;
                // if (i == 0 || j == 0)
                //     cost += 10;
                // else
                //     cost += 14;
                //
                // // Update neighbour cost
                // if (cost < neighbour.SumCost)
                // {
                //     neighbour.SetSumCost(cost);
                //     // neighbour.parentX = _current.x;
                //     // neighbour.parentY = _current.y;
                // }
            }
        }

        // Remove from unvisited and add to visited
        // _open.Remove(_current);
        // _closed.Add(_current);
        // if(!(_current.x == startX && _current.y == startY))
        // {
        //     _current.GetComponent<MeshRenderer>().material.color = Color.magenta;
        // }

        // // Update neighbour cost (if lower).
        // float mincost = _open.Min(t => t.SumCost);
        // _current = _open.Find(t => t.SumCost == mincost);

        // Check if we're at the end.
        // if ((_current.x == endX && _current.y == endY) || _open.Count == 0)
        //     return true;
        // else
        //     return false;
        return false;
    }

    private void BackTrackRoute()
    {
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
                return Mathf.Max(x, y) * 14 + Mathf.Abs(x-y) * 10;
            case Metric.Dijkstra:
                return 0;
            default:
                // Defaults to Dijkstra.
                return 0;
        }
    }
}
