using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.PlasticSCM.Editor.WebApi;

public class PathFinding : MonoBehaviour
{
    public const int width = 20;
    public const int height = 20;

    public int Width { get; private set; }
    public int Height { get; private set; }

    public int startX = 1;
    public int startY = 1;

    public int endX = 18;
    public int endY = 17;

    private const float padding = .1f;

    [SerializeField] private float stepInterval = .1f;
    private float timer = 0;
    private bool path_finished = false;
    public bool paused = true;

    [SerializeField] private GameObject tileobject;

    GameObject[,] _tiles = new GameObject[width, height];

    // List for "unvisited" nodes.
    List<Tile> unvisited = new();
    // List for "closed" nodes.
    List<Tile> visited = new();
    Tile current;

    private void Awake()
    {
        Width = width;
        Height = height;

        for(int x = 0; x < width; x++)
        {
            for( int y = 0; y < height; y++)
            {
                _tiles[x, y] = Instantiate(tileobject, new Vector3(x * (tileobject.transform.localScale.x + padding), 0, y * (tileobject.transform.localScale.y + padding)), tileobject.transform.rotation);
                _tiles[x, y].GetComponent<Tile>().x = x;
                _tiles[x, y].GetComponent<Tile>().y = y;
            }
        }
    }

    private void Start()
    {
        _tiles[startX, startY].GetComponent<MeshRenderer>().material.color = Color.red;
        _tiles[endX, endY].GetComponent<MeshRenderer>().material.color = Color.blue;

        InitDisjkstra();

        _tiles[3, 5].GetComponent<Tile>().is_traversable = false;
        _tiles[4, 4].GetComponent<Tile>().is_traversable = false;
        _tiles[4, 5].GetComponent<Tile>().is_traversable = false;

        foreach (GameObject tile in _tiles)
        {
            if (!tile.GetComponent<Tile>().is_traversable)
            {
                tile.GetComponent<MeshRenderer>().material.color = Color.black;
                tile.GetComponent<Tile>().costField.text = "";
                tile.GetComponent<Tile>().leftField.text = "";
                tile.GetComponent<Tile>().rightField.text = "";
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            paused = !paused;
        }

        if (!paused)
        {
            timer += Time.deltaTime;
        }

        if (timer >= stepInterval)
        {
            path_finished = StepDisjkstra();
            timer = 0;
        }

        if (path_finished)
        {
            BackTrackRoute();
            this.enabled = false;
        }
    }

    private void InitDisjkstra()
    {
        // Set the start tile cost to 0.
        _tiles[startX, startY].GetComponent<Tile>().cost = 0;

        unvisited.Clear();

        // Populate the list.
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                unvisited.Add(_tiles[x, y].GetComponent<Tile>());
            }
        }

        visited.Clear();

        current = unvisited.Find( t => t.x == startX && t.y == startY);
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

                Tile neighbour = unvisited.Find(t => t.x == current.x + i && t.y == current.y + j);

                if (neighbour == null || !neighbour.is_traversable)
                    continue;
                if (!(neighbour.x == endX && neighbour.y == endY))
                    neighbour.GetComponent<MeshRenderer>().material.color = Color.gray;

                // Compute the cost
                float cost = current.cost;
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
                    neighbour.SetPrev(current.x, current.y);
                }
            }
        }

        // Remove from unvisited and add to visited
        unvisited.Remove(current);
        visited.Add(current);
        if(!(current.x == startX && current.y == startY))
        {
            current.GetComponent<MeshRenderer>().material.color = Color.magenta;
        }

        // Update neighbour cost (if lower).
        float mincost = unvisited.Min(t => t.cost);
        current = unvisited.Find(t => t.cost == mincost);
        Debug.Log("Min cost: " + mincost);

        // Check if we're at the end.
        if ((current.x == endX && current.y == endY) || unvisited.Count == 0)
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

            t = _tiles[t.GetPrevX(), t.GetPrevY()].GetComponent<Tile>();
        }
        
    }

}
