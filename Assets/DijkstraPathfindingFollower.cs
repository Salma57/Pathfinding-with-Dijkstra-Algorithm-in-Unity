using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DijkstraPathfindingFollower : MonoBehaviour
{
    public Tilemap moveTilemap;
    public Tilemap obstacleTilemap;
    public Transform player;
    public Transform goal;

    private List<Vector3Int> path;
    private Vector3Int lastGoalPosition;
    private Coroutine moveCoroutine;

    void Start()
    {
        lastGoalPosition = moveTilemap.WorldToCell(goal.position);
        path = FindPath(player.position, goal.position);
        moveCoroutine = StartCoroutine(MoveAlongPath());
    }

    void Update()
    {
        Vector3Int currentGoalPosition = moveTilemap.WorldToCell(goal.position);

        
        if (currentGoalPosition != lastGoalPosition)
        {
            lastGoalPosition = currentGoalPosition;

            
            path = FindPath(player.position, goal.position);
        }
    }

    List<Vector3Int> FindPath(Vector3 startPos, Vector3 targetPos)
    {
        Vector3Int start = moveTilemap.WorldToCell(startPos);
        Vector3Int target = moveTilemap.WorldToCell(targetPos);

        Dictionary<Vector3Int, float> distance = new Dictionary<Vector3Int, float>();
        Dictionary<Vector3Int, Vector3Int> cameFrom = new Dictionary<Vector3Int, Vector3Int>();

        List<Vector3Int> unvisited = new List<Vector3Int> { start };
        distance[start] = 0;

        while (unvisited.Count > 0)
        {
            Vector3Int current = unvisited[0];
            foreach (var node in unvisited)
                if (distance[node] < distance[current])
                    current = node;

            if (current == target)
                return ReconstructPath(cameFrom, current);

            unvisited.Remove(current);

            foreach (var neighbor in GetNeighbors(current))
            {
                if (obstacleTilemap.HasTile(neighbor) || distance.ContainsKey(neighbor))
                    continue;

                float newCost = distance[current] + 1;

                if (!distance.ContainsKey(neighbor) || newCost < distance[neighbor])
                {
                    distance[neighbor] = newCost;
                    cameFrom[neighbor] = current;
                    unvisited.Add(neighbor);
                }
            }
        }

        return new List<Vector3Int>();
    }

    List<Vector3Int> ReconstructPath(Dictionary<Vector3Int, Vector3Int> cameFrom, Vector3Int current)
    {
        var path = new List<Vector3Int> { current };
        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            path.Add(current);
        }
        path.Reverse();
        return path;
    }

    IEnumerator MoveAlongPath()
    {
        
        while (true)
        {
            if (path.Count == 0)
                yield break;

            Vector3Int currentCell = path[0];
            Vector3 targetPosition = moveTilemap.CellToWorld(currentCell) + new Vector3(0.5f, 0.5f, 0);

            
            while (Vector3.Distance(player.position, targetPosition) > 0.05f)
            {
                player.position = Vector3.MoveTowards(player.position, targetPosition, Time.deltaTime * 3f);
                yield return null;
            }

            path.RemoveAt(0);

            
            if (Vector3.Distance(player.position, goal.position) < 0.1f)
            {
                yield break;
            }

            
            if (path.Count == 0)
            {
                path = FindPath(player.position, goal.position);
            }
        }
    }

    List<Vector3Int> GetNeighbors(Vector3Int cell)
    {
        return new List<Vector3Int>
        {
            cell + Vector3Int.up,
            cell + Vector3Int.down,
            cell + Vector3Int.left,
            cell + Vector3Int.right
        };
    }
}
