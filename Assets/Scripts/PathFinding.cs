using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PathFinding : MonoBehaviour
{
    private enum Metric
    {
        Manhattan = 0,
        Euclidean = 1,
        Diagonal = 2
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

    GameObject[,] _tiles = new GameObject[width, height];

    // List for "open" nodes.
    List<Tile> _unvisited = new();
    // List for "closed" nodes.
    List<Tile> _visited = new();
    Tile _current;

    private void Awake()
    {
        Width = width;
        Height = height;

        for(int x = 0; x < width; x++)
        {
            for( int y = 0; y < height; y++)
            {
                _tiles[x, y] = Instantiate(tileObject, new Vector3(x * (tileObject.transform.localScale.x + Padding), 0, y * (tileObject.transform.localScale.y + Padding)), tileObject.transform.rotation);
                _tiles[x, y].GetComponent<Tile>().x = x;
                _tiles[x, y].GetComponent<Tile>().y = y;
            }
        }
    }

    private void Start()
    {
        _tiles[startX, startY].GetComponent<MeshRenderer>().material.color = Color.red;
        _tiles[startX, startY].GetComponent<Tile>().unTouchable = true;
        _tiles[endX, endY].GetComponent<MeshRenderer>().material.color = Color.blue;
        _tiles[endX, endY].GetComponent<Tile>().unTouchable = true;

        InitDisjkstra();
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

        if (_timer >= stepInterval)
        {
            _pathFinished = StepDisjkstra();
            _timer = 0;
        }

        if (_pathFinished)
        {
            BackTrackRoute();
            this.enabled = false;
        }
    }

    private void InitDisjkstra()
    {
        // Set the start tile cost to 0.
        _tiles[startX, startY].GetComponent<Tile>().cost = 0;

        _unvisited.Clear();

        // Populate the list.
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                _unvisited.Add(_tiles[x, y].GetComponent<Tile>());
            }
        }

        _visited.Clear();

        _current = _unvisited.Find( t => t.x == startX && t.y == startY);
    }

    private bool StepDisjkstra()
    {
        // Go through neighbours
        for(int i = -1; i<= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0)
                    continue;

                Tile neighbour = _unvisited.Find(t => t.x == _current.x + i && t.y == _current.y + j);

                if (neighbour == null || !neighbour.isTraversable)
                    continue;
                if (!(neighbour.x == endX && neighbour.y == endY))
                    neighbour.GetComponent<MeshRenderer>().material.color = Color.gray;

                // Compute the cost
                float cost = _current.cost;
                if(i == 0 || j == 0)
                    cost += 10;
                else
                    cost += 14;
                Debug.Log(cost);

                // Update neighbour cost
                if (cost < neighbour.cost)
                {
                    neighbour.cost = cost;
                    neighbour.costField.text = cost.ToString();
                    neighbour.prevX = _current.x;
                    neighbour.prevY = _current.y;
                }
            }
        }

        // Remove from unvisited and add to visited
        _unvisited.Remove(_current);
        _visited.Add(_current);
        if(!(_current.x == startX && _current.y == startY))
        {
            _current.GetComponent<MeshRenderer>().material.color = Color.magenta;
        }

        // Update neighbour cost (if lower).
        float mincost = _unvisited.Min(t => t.cost);
        _current = _unvisited.Find(t => t.cost == mincost);

        // Check if we're at the end.
        if ((_current.x == endX && _current.y == endY) || _unvisited.Count == 0)
            return true;
        else
            return false;

    }

    private void BackTrackRoute()
    {
        Tile t = _tiles[endX, endY].GetComponent<Tile>();
        while (!(t.x == startX && t.y == startY))
        {
            if(!(t.x == endX && t.y == endY))
                t.GetComponent<MeshRenderer>().material.color = Color.yellow;
            
            t = _tiles[t.prevX, t.prevY].GetComponent<Tile>();
        }
    }
}
