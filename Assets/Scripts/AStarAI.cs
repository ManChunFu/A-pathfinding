using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using Assets.Scripts;

public partial class AStarAI : MonoBehaviour, IPathFinding
{
    [SerializeField] private Transform _player;
    [SerializeField] private Transform _target;
    [SerializeField] private Vector2 _startPoint;
    [SerializeField] private Vector2 _endPoint;
    [SerializeField] private GameObject _obstacle;
    [SerializeField] List<Vector2> ObstacleList;
    private Vector2[] SearchChildByStraightLine;
    private Vector2[] SearchChildByDiagnoalLine;
    [SerializeField] private GameObject _seed;
    [SerializeField] private GameObject _node;

    const float minX = 2f, maxX = 15f, minY = 1f, maxY = 8f;

    public bool NoPathFound = false;
    public List<Vector3> Path { get; set; }
    public bool PathCompleted { get; set; }

    private void OnEnable()
    {
        Assert.IsNotNull(_player, "No reference to Player.");
        if (_player != null)
         _startPoint = GetWorldPositionToGridPosition(_player.position);

        Assert.IsNotNull(_target, "No referernce to Target.");
        if (_target != null)
         _endPoint = GetWorldPositionToGridPosition(_target.position);

        Assert.IsNotNull(_obstacle, "No reference to Obstacle.");
        if (_obstacle != null)
        {
            ObstacleList = new List<Vector2>();
            foreach (Transform o in _obstacle.transform)
                ObstacleList.Add(GetWorldPositionToGridPosition(o.position));
        }

        SearchChildByStraightLine = new Vector2[4]
        {
            new Vector2 (-1f, 0f), //left
            new Vector2 (0f, 1f), // up
            new Vector2 (1f, 0f), //right
            new Vector2 (0f, -1f) //down
        };

        SearchChildByDiagnoalLine = new Vector2[4]
        {
            new Vector2 (-1f, 1f), //upper-left corner
            new Vector2 (1f, 1f), //upper-right corner
            new Vector2 (1f, -1f), //lower-right corner
            new Vector2 (-1f, -1f) //lower-left corner
        };

        Path = new List<Vector3>();

        StartCoroutine(FindDestination());
    }

    private Vector2 GetWorldPositionToGridPosition(Vector3 worldPos)
    {
        float x = worldPos.x + 9f;
        float y = worldPos.y + 5f;
        return new Vector2(x, y);
    }

    private Vector3 GetGridPositionToWorldPosition(Vector2 gridPos)
    {
        float x = gridPos.x - 9f;
        float y = gridPos.y - 5f;
        return new Vector3(x, y, 0);
    }

    private float GetHcost(Vector2 newNodePos)
    {
        float x = Mathf.Abs(_endPoint.x - newNodePos.x);
        float y = Mathf.Abs(_endPoint.y - newNodePos.y);
        if (x < y)
            return (y - x) + (Mathf.Sqrt(Mathf.Pow(x, 2) + Mathf.Pow(x, 2)));

        return (x - y) + (Mathf.Sqrt(Mathf.Pow(y, 2) + Mathf.Pow(y, 2)));
    }

    private IEnumerator FindDestination()
    {
        List<AStarNode> examinatedNodeArea = new List<AStarNode>();
        examinatedNodeArea.Add(new AStarNode(_startPoint, GetHcost(_startPoint), null));

        List<AStarNode> workingNodes = new List<AStarNode>();
        bool reachGoal = false;

        float currentSamllestFCost = 1000;
        do
        {
            int miniFCost = (int)examinatedNodeArea.Where(n => !n.NodeCompleted).Min(n => n.FCost);
            int miniHCostOfsmallestFCost = (int)examinatedNodeArea.Where(n => (int)n.FCost == miniFCost && !n.NodeCompleted).Min(n => n.HCost);

            if (miniFCost > currentSamllestFCost)
                workingNodes = examinatedNodeArea.FindAll(n => (int)n.FCost == miniFCost && !n.NodeCompleted);
            else
            {
                currentSamllestFCost = miniFCost;
                workingNodes = examinatedNodeArea.FindAll(n => (int)n.FCost == miniFCost && (int)n.HCost == miniHCostOfsmallestFCost && !n.NodeCompleted);

                if (!workingNodes.Any())
                    yield break;
            }

            foreach (AStarNode current in workingNodes)
            {
                if (current.Position == _endPoint)
                {
                    reachGoal = true;
                    StartCoroutine(DrawPath(current));
                }

                foreach (Vector2 childPos in SearchChildByStraightLine)
                {
                    Vector2 newNodePosition = current.Position + childPos;
                    if (newNodePosition.x >= minX && newNodePosition.x <= maxX && newNodePosition.y >= minY && newNodePosition.y <= maxY)
                        if (!ObstacleList.Any(o => o == newNodePosition))
                        {
                            AStarNode existedNode = examinatedNodeArea.Find(e => e.Position == newNodePosition);
                            if (existedNode == null)
                            {
                                examinatedNodeArea.Add(new AStarNode(newNodePosition, GetHcost(newNodePosition), current));
                                if (current.Position != _endPoint)
                                {
                                    Instantiate(_node, GetGridPositionToWorldPosition(newNodePosition), Quaternion.identity);
                                    yield return new WaitForSeconds(0.1f);
                                }
                            }
                            else if (!current.Parent.Contains(existedNode))
                                current.Parent.Add(existedNode);
                        }
                }

                if (!reachGoal)
                {
                    foreach (Vector2 childPos in SearchChildByDiagnoalLine)
                    {
                        Vector2 newNodePosition = current.Position + childPos;
                        Vector2 newNodePositionX = current.Position;
                        newNodePositionX.x += childPos.x;
                        Vector2 newNodePositionY = current.Position;
                        newNodePositionY.y += childPos.y;

                        if (newNodePosition.x >= minX && newNodePosition.x <= maxX && newNodePosition.y >= minY && newNodePosition.y <= maxY)
                            if (!ObstacleList.Any(o => o == newNodePosition))
                            {
                                if (!(ObstacleList.Any(o => o == newNodePositionX) && ObstacleList.Any(o => o == newNodePositionY)))
                                {
                                    AStarNode existedNode = examinatedNodeArea.Find(e => e.Position == newNodePosition);
                                    if (existedNode == null)
                                    {
                                        examinatedNodeArea.Add(new AStarNode(newNodePosition, GetHcost(newNodePosition), current));
                                        Instantiate(_node, GetGridPositionToWorldPosition(newNodePosition), Quaternion.identity);
                                        yield return new WaitForSeconds(0.1f);
                                    }
                                    else if (!current.Parent.Contains(existedNode))
                                        current.Parent.Add(existedNode);
                                }
                            }
                    }
                }
                current.NodeCompleted = true;
            }
        } while (examinatedNodeArea.Any(e => !e.NodeCompleted) && !reachGoal);

        if (!reachGoal)
            NoPathFound = true;
    }

    private IEnumerator DrawPath(AStarNode lastNode)
    {
        AStarNode workingNode = lastNode;
        Path.Add(GetGridPositionToWorldPosition(workingNode.Position));

        while (workingNode.Parent.Any())
        {
            AStarNode closestParent = workingNode.Parent.Where(l => l.NodeCompleted).OrderBy(l => l.GCost).First();
            Path.Add(GetGridPositionToWorldPosition(workingNode.Position));
            workingNode = closestParent;
        }

        foreach (Vector3 node in Path)
        {
            Instantiate(_seed, node, Quaternion.identity);
            yield return new WaitForSeconds(0.1f);
        }
        PathCompleted = true;
    }

}

