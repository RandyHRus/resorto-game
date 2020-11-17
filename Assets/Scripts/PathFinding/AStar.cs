using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Priority_Queue;
using System.Linq;
using System;

public static class AStar
{
    private const int MOVE_STRAIGHT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 14;

    public delegate void NodeUpdate(AStarNode node, AStarNodeUpdateType type);

    public class AStarNode: FastPriorityQueueNode {

        public AStarNode(Vector2Int position, AStarNode previousNode, Vector2Int pathEnd, int layerNum, Vector2Int? isEndPositionOfStairsAt)
        {
            this.Position = position;
            this.PreviousNode = previousNode;
            HCost = CalculateDistanceCost(Position, pathEnd);
            ReCalculateGAndFCosts();
            this.layerNum = layerNum;
            this.isEndPositionOfStairsAt = isEndPositionOfStairsAt;
        }

        public void ChangePreviousAndRecalculateCost(AStarNode newPreviousNode)
        {
            this.PreviousNode = newPreviousNode;
            ReCalculateGAndFCosts();
        }

        //H does not need to be recalculated as it will not change throughout the algorithm
        private void ReCalculateGAndFCosts()
        {
            GCost = PreviousNode == null ? 0: PreviousNode.GCost + CalculateDistanceCost(PreviousNode.Position, Position);
            FCost = GCost + HCost;
        }

        public AStarNode PreviousNode { get; private set; }
        public Vector2Int Position { get; private set; }
        public int GCost { get; private set; } // Distance from start
        public int HCost { get; private set; } // Distance to end
        public int FCost { get; private set; } // gCost + hCost
        public readonly int layerNum;
        public readonly Vector2Int? isEndPositionOfStairsAt;
    }

    private static int CalculateDistanceCost(Vector2Int distanceStart, Vector2Int distanceEnd)
    {
        int xDistance = Mathf.Abs(distanceStart.x - distanceEnd.x);
        int yDistance = Mathf.Abs(distanceStart.y - distanceEnd.y);
        int remaining = Mathf.Abs(xDistance - yDistance);
        return MOVE_DIAGONAL_COST * Mathf.Min(xDistance, yDistance) + MOVE_STRAIGHT_COST * remaining;
    }

    //Returns -1 if no path
    public static InGameTime? EstimatePathTime(Vector2Int pathStart, Vector2Int pathEnd, float moveSpeed, out LinkedList<Tuple<Vector2Int, Vector2Int?>> path)
    {
        path = GetShortestPath(pathStart, pathEnd);

        if (path == null)
            return null;

        LinkedListNode<Tuple<Vector2Int, Vector2Int?>> node = path.First;

        float length = 0;

        while (node.Next != null)
        {
            length += Vector2.Distance(node.Value.Item1, node.Next.Value.Item1);
            node = node.Next;
        }

        float timePerOneTile = TimeManager.Instance.timeSpeed * moveSpeed;

        float timeToWalkPathMinutes = timePerOneTile * length;

        return new InGameTime((int)timeToWalkPathMinutes);
    }

    /*
     * Use the A* Pathfinding algorithm to return the shortest path, Will return a list of positions from start to finish.
     */
    public static LinkedList<Tuple<Vector2Int, Vector2Int?>> GetShortestPath(Vector2Int pathStart, Vector2Int pathEnd)
    {
        AStarPathFinder pathFinder = new AStarPathFinder(pathStart, pathEnd);

        while (!pathFinder.Finished)
        {
            pathFinder.NextStep();
        }

        return pathFinder.ShortestPath;
    }

    /*
     * Call NextNode() to trigger next step
     * Once its done, Finished will be marked true
     * Then you can retrieve the path from "ShortestPath"
     * ShortestPath is null if no valid path is found.
     * 
     * The shortest path is a truncated linked list from beginning to end
     * By truncated, I mean it removes any unnecessary nodes if there is a straight line.
     * Ex. If there is a path of 5 tiles in a straight line, the list will only return the beginning and the end.
     * 
     * It is designed this way and not a direct return function so that
     * it is easier to debug visually.
     */
    public class AStarPathFinder
    {
        private FastPriorityQueue<AStarNode> openNodes;
        private HashSet<AStarNode> closedNodes;
        private Vector2Int pathStart, pathEnd;

        private NodeUpdate OnNodeUpdated;

        public bool Finished { get; private set; }

        //Vector2Int? represents stairs position.
        //Only the intermediate and end should be put for stairs, because we want the TARGET to be stairs.
        public LinkedList<Tuple<Vector2Int, Vector2Int?>> ShortestPath { get; private set; }

        public AStarPathFinder(Vector2Int pathStart, Vector2Int pathEnd, NodeUpdate OnNodeUpdated = null)
        {
            /*
             * Since we are using a priority queue implementation, if the node being examined is already in the OPEN set
             * and its F score is better than the one already in the open set, then the node needs to be removed from the
             * open set and reinserted. Otherwise, the ordering will be broken;
             */
            openNodes = new FastPriorityQueue<AStarNode>(TileInformationManager.totalTilesCount);
            closedNodes = new HashSet<AStarNode>();

            TileInformationManager.Instance.TryGetTileInformation(pathStart, out TileInformation startTileInfo);
            AStarNode startNode = new AStarNode(pathStart, null, pathEnd, startTileInfo.layerNum, null);
            openNodes.Enqueue(startNode, startNode.FCost);
            OnNodeUpdated?.Invoke(startNode, AStarNodeUpdateType.NewlyOpen);

            this.pathStart = pathStart;
            this.pathEnd = pathEnd;
            this.OnNodeUpdated = OnNodeUpdated;

            Finished = false;
        }

        public void NextStep()
        {
            if (openNodes.Count == 0)
            {
                ShortestPath = null;
                Debug.Log("No valid path found!");
                Finished = true;
                return;
            }
            else if (openNodes.First.Position != pathEnd)
            {

                // Get the node with the lowest TotalCost, the priority queue should be able to do this in log(n) time,
                // Needs to balance so not O(1). But still seems like the best implementation for now as it is not too complicated.
                AStarNode leastCostNode = openNodes.Dequeue();

                AStarNode currentNode = leastCostNode;
                closedNodes.Add(currentNode);
                OnNodeUpdated?.Invoke(currentNode, AStarNodeUpdateType.Closed);

                //Get next nodes (Successors)
                Vector2Int[] successors = new Vector2Int[]
                {
                    new Vector2Int(currentNode.Position.x - 1, currentNode.Position.y + 1),
                    new Vector2Int(currentNode.Position.x - 1, currentNode.Position.y),
                    new Vector2Int(currentNode.Position.x - 1, currentNode.Position.y - 1),
                    new Vector2Int(currentNode.Position.x,     currentNode.Position.y + 1),
                    new Vector2Int(currentNode.Position.x,     currentNode.Position.y - 1),
                    new Vector2Int(currentNode.Position.x + 1, currentNode.Position.y + 1),
                    new Vector2Int(currentNode.Position.x + 1, currentNode.Position.y),
                    new Vector2Int(currentNode.Position.x + 1, currentNode.Position.y - 1)
                };

                foreach (Vector2Int tempSuccessorPos in successors)
                {
                    //Check destination collision
                    //And also, check for stairs
                    StairsStartPosition stairsStartPosition = null;
                    if (CollisionManager.CheckForCollisionOnTile(tempSuccessorPos, currentNode.layerNum))
                    {
                        Vector2Int directionVector = tempSuccessorPos - currentNode.Position;
                        if (!(directionVector.x != 0 ^ directionVector.y != 0)) //Either x or y is not 0, exclusively
                            continue;

                        Direction stairsDirection = directionVector.x != 0 ?
                            (directionVector.x > 0 ? Direction.Right : Direction.Left) :
                            (directionVector.y > 0 ? Direction.Up : Direction.Down);

                        TileInformationManager.Instance.TryGetTileInformation(currentNode.Position, out TileInformation currentTileInfo);
                        if (!currentTileInfo.StairsStartPositionWithDirectionExists(stairsDirection, out stairsStartPosition))
                            continue;
                    }

                    //Check diagonal collision
                    if (tempSuccessorPos.x - currentNode.Position.x != 0 && tempSuccessorPos.y - currentNode.Position.y != 0)
                    {
                        if (CollisionManager.CheckForCollisionOnTile(new Vector2Int(currentNode.Position.x, tempSuccessorPos.y), currentNode.layerNum) ||
                            CollisionManager.CheckForCollisionOnTile(new Vector2Int(tempSuccessorPos.x, currentNode.Position.y), currentNode.layerNum))
                        {
                            continue;
                        }
                    }

                    Vector2Int successorPos = stairsStartPosition != null ? stairsStartPosition.endPosition : tempSuccessorPos;
                    int successorLayerNum = stairsStartPosition != null ? stairsStartPosition.endLayerNum : currentNode.layerNum;

                    //Check if successor is in open list
                    AStarNode openNode = null;
                    foreach (AStarNode node in openNodes)
                    {
                        if (node.Position == successorPos)
                        {
                            openNode = node;
                            break;
                        }
                    }

                    //Check if successor is in closed list
                    AStarNode closedNode = null;
                    foreach (AStarNode node in closedNodes)
                    {
                        if (node.Position == successorPos)
                        {
                            closedNode = node;
                            break;
                        }
                    }

                    int tentativeGCost = currentNode.GCost + CalculateDistanceCost(currentNode.Position, successorPos);

                    /*
                     * if successor in OPEN and cost less than g(successor):
                     * remove successor from OPEN, because new path is better
                    */
                    if (openNode != null)
                    {
                        if (tentativeGCost > openNode.GCost)
                            continue;

                        openNodes.Remove(openNode);
                        openNode.ChangePreviousAndRecalculateCost(currentNode);
                        openNodes.Enqueue(openNode, openNode.FCost);
                        OnNodeUpdated?.Invoke(openNode, AStarNodeUpdateType.Overwritten);
                    }
                    /*
                     * if successor in CLOSED and cost less than g(successor): ⁽²⁾
                     * remove neighbor from CLOSED
                     */
                    else if (closedNode != null)
                    {
                        if (tentativeGCost > closedNode.GCost)
                            continue;

                        closedNodes.Remove(closedNode);
                        closedNode.ChangePreviousAndRecalculateCost(currentNode);
                        openNodes.Enqueue(closedNode, closedNode.FCost);
                        OnNodeUpdated?.Invoke(closedNode, AStarNodeUpdateType.Overwritten);
                    }
                    /*
                     *   if successor not in OPEN and successor not in CLOSED, 
                     *   Need to create new AStarNode and put it in open
                     */
                    else
                    {
                        TileInformationManager.Instance.TryGetTileInformation(successorPos, out TileInformation successorTile);
                        AStarNode successorNode = new AStarNode(successorPos, currentNode, pathEnd, successorLayerNum, stairsStartPosition?.stairsPosition);
                        openNodes.Enqueue(successorNode, successorNode.FCost);
                        OnNodeUpdated?.Invoke(successorNode, AStarNodeUpdateType.NewlyOpen);
                    }
                }
            }
            /*
             * Case when openNode's first node is the end position
             * Backtrack and return answer 
             * 
             * Truncation happens here
            */
            else
            {
                AStarNode currentNode  = openNodes.Dequeue();
                AStarNode previousNode = currentNode.PreviousNode;

                Vector2Int currentDirection = new Vector2Int(0,0);
                ShortestPath = new LinkedList<Tuple<Vector2Int, Vector2Int?>>(); 

                while (currentNode != null)
                {
                    OnNodeUpdated?.Invoke(currentNode, AStarNodeUpdateType.MarkShortestPath);

                    if (previousNode == null)
                    {
                        ShortestPath.AddFirst(Tuple.Create<Vector2Int, Vector2Int?>(currentNode.Position, null));
                        OnNodeUpdated?.Invoke(currentNode, AStarNodeUpdateType.MarkPathKeyPoint);
                    }
                    else
                    {
                        if (currentNode.isEndPositionOfStairsAt != null)
                        {
                            // When on stairs,
                            // We need to readjust the path taken by AStar because
                            // Going from straight from stairs start to end on the case where stairs is direction left or right,
                            // Will not create a nice looking movement.
                            if (currentNode.Position.x != previousNode.Position.x)
                            {
                                Vector2Int intermediatePos = new Vector2Int(-(int)Mathf.Sign(currentNode.Position.x - previousNode.Position.x) + currentNode.Position.x,
                                                                            currentNode.Position.y > previousNode.Position.y ? currentNode.Position.y : previousNode.Position.y);

                                ShortestPath.AddFirst(Tuple.Create<Vector2Int, Vector2Int?>(currentNode.Position, currentNode.isEndPositionOfStairsAt));
                                OnNodeUpdated?.Invoke(currentNode, AStarNodeUpdateType.MarkPathKeyPoint);

                                ShortestPath.AddFirst(Tuple.Create<Vector2Int, Vector2Int?>(intermediatePos, currentNode.isEndPositionOfStairsAt));
                                //OnNodeUpdated?.Invoke(intermediatePos, AStarNodeUpdateType.MarkPathKeyPoint);
                            }
                            else
                            {
                                ShortestPath.AddFirst(Tuple.Create<Vector2Int, Vector2Int?>(currentNode.Position, currentNode.isEndPositionOfStairsAt));
                                OnNodeUpdated?.Invoke(currentNode, AStarNodeUpdateType.MarkPathKeyPoint);
                            }

                            currentDirection = new Vector2Int(0, 0); //reset direction
                        }
                        else
                        {
                            Vector2Int newDirection = currentNode.Position - previousNode.Position;
                            if (newDirection != currentDirection)
                            {
                                ShortestPath.AddFirst(Tuple.Create<Vector2Int, Vector2Int?>(currentNode.Position, null));
                                OnNodeUpdated?.Invoke(currentNode, AStarNodeUpdateType.MarkPathKeyPoint);
                                currentDirection = newDirection;
                            }
                        }
                    }

                    currentNode = previousNode;
                    previousNode = currentNode?.PreviousNode;
                }

                Finished = true;
            }
        }
    }

    public enum AStarNodeUpdateType
    {
        NewlyOpen,
        Closed,
        Overwritten,
        MarkShortestPath,
        MarkPathKeyPoint
    }
}
