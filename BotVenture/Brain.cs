using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BotVenture
{
    public class Brain
    {
        private readonly IO io;
        // Internal map storing discovered tiles.
        private readonly Dictionary<(int x, int y), TileType> map;
        // Player’s current position in the overall map.
        private (int x, int y) currentPosition;
        // bool to track the serpant movement
        private bool scanningRight = true;


        // Mapping directions to coordinate offsets.
        private readonly Dictionary<Direction, (int dx, int dy)> directionDeltas = new Dictionary<Direction, (int dx, int dy)> {
            { Direction.Up, (0, -1) },
            { Direction.Down, (0, 1) },
            { Direction.Left, (-1, 0) },
            { Direction.Right, (1, 0) }
        };

        public Brain(IO io)
        {
            this.io = io ?? throw new ArgumentNullException(nameof(io));
            map = new Dictionary<(int, int), TileType>();
            // We always spawn in the top left corner; set starting coordinates to (0,0)
            currentPosition = (0, 0);
        }

        public async Task Run()
        {
            bool levelComplete = false;
            while (!levelComplete)
            {
                // Get a new look and update our map.
                var lookGrid = await io.PlayerLook();
                UpdateMap(lookGrid);

                // Check if we have discovered a goal in our internal map.
                var goalPos = FindGoalPosition();
                if (goalPos != null)
                {
                    // Immediately compute a path from currentPosition to the goal.
                    var path = FindPath(currentPosition, goalPos.Value);
                    if (path != null)
                    {
                        foreach (var move in path)
                        {
                            var moveResponse = await io.PlayerMove(move);
                            if (moveResponse != null && moveResponse.Success)
                            {
                                currentPosition = (
                                    currentPosition.x + directionDeltas[move].dx,
                                    currentPosition.y + directionDeltas[move].dy);
                            }
                            await Task.Delay(100); // 100ms delay between moves

                            // Refresh view and update the map—but note that if a cell is missing due to the API bug,
                            // the previous (possibly goal) value remains.
                            lookGrid = await io.PlayerLook();
                            UpdateMap(lookGrid);

                            // If the current cell now shows the goal, finish the level immediately.
                            if (map.TryGetValue(currentPosition, out TileType tile) &&
                                (tile & TileType.Goal) == TileType.Goal)
                            {
                                levelComplete = true;
                                break;
                            }
                        }
                        if (levelComplete)
                            continue; // Skip further exploration if goal reached.
                    }
                }

                // If no goal is visible in the map (or no valid path was found),
                // perform systematic scanning (serpentine/exploration).
                await SerpentineScan();
            }
        }

        private async Task SerpentineScan()
        {
            // Refresh view and update the map.
            var lookGrid = await io.PlayerLook();
            UpdateMap(lookGrid);

            if (scanningRight)
            {
                // Try moving to the right.
                var moveResponse = await io.PlayerMove(Direction.Right);
                if (moveResponse != null && moveResponse.Success)
                {
                    currentPosition = (
                        currentPosition.x + directionDeltas[Direction.Right].dx,
                        currentPosition.y + directionDeltas[Direction.Right].dy);
                }
                else
                {
                    // If moving right fails (e.g. wall reached), switch direction.
                    scanningRight = false;
                    // Move down by 5 cells to avoid overlapping scanned areas.
                    for (int i = 0; i < 5; i++)
                    {
                        var downResponse = await io.PlayerMove(Direction.Down);
                        if (downResponse != null && downResponse.Success)
                        {
                            currentPosition = (
                                currentPosition.x + directionDeltas[Direction.Down].dx,
                                currentPosition.y + directionDeltas[Direction.Down].dy);
                        }
                        else
                        {
                            break;
                        }
                        await Task.Delay(100);
                    }
                }
            }
            else // Currently scanning left
            {
                var moveResponse = await io.PlayerMove(Direction.Left);
                if (moveResponse != null && moveResponse.Success)
                {
                    currentPosition = (
                        currentPosition.x + directionDeltas[Direction.Left].dx,
                        currentPosition.y + directionDeltas[Direction.Left].dy);
                }
                else
                {
                    // If moving left fails, switch direction.
                    scanningRight = true;
                    // Move down by 5 cells.
                    for (int i = 0; i < 5; i++)
                    {
                        var downResponse = await io.PlayerMove(Direction.Down);
                        if (downResponse != null && downResponse.Success)
                        {
                            currentPosition = (
                                currentPosition.x + directionDeltas[Direction.Down].dx,
                                currentPosition.y + directionDeltas[Direction.Down].dy);
                        }
                        else
                        {
                            break;
                        }
                        await Task.Delay(100);
                    }
                }
            }
            // Allow some delay after the move.
            await Task.Delay(100);
        }




        /// <summary>
        /// Updates the internal map using the 5x5 grid returned by IO.PlayerLook.
        /// Assumes the player is at the center (index [2,2]) of the grid.
        /// </summary>
        private void UpdateMap(List<TileType?>[,] lookGrid)
        {
            if (lookGrid == null)
                throw new ArgumentNullException(nameof(lookGrid), "The look grid cannot be null.");

            int numRows = lookGrid.GetLength(0);
            int numCols = lookGrid.GetLength(1);
            int centerRow = numRows / 2;
            int centerCol = numCols / 2;

            for (int i = 0; i < numRows; i++)
            {
                for (int j = 0; j < numCols; j++)
                {
                    // Calculate world coordinates relative to the current position.
                    int worldX = currentPosition.x + (i - centerRow);
                    int worldY = currentPosition.y + (j - centerCol);

                    var cellList = lookGrid[i, j];
                    // Only update if the current look provides a valid tile.
                    // Otherwise, preserve what was discovered earlier.
                    if (cellList != null && cellList.Count > 0 && cellList[0].HasValue)
                    {
                        map[(worldX, worldY)] = cellList[0].Value;
                    }
                }
            }
        }


        /// <summary>
        /// Searches the known map for a tile that has the Goal flag.
        /// </summary>
        private (int x, int y)? FindGoalPosition()
        {
            foreach (var kv in map)
            {
                if (kv.Value == TileType.Goal)
                {
                    return kv.Key;
                }
            }
            return null;
        }

        /// <summary>
        /// Uses A* search to find a path (as a list of Directions) from start to goal.
        /// Only considers cells in the internal map that are known to be walkable.
        /// </summary>
        private List<Direction> FindPath((int x, int y) start, (int x, int y) goal)
        {
            // openList holds nodes to be evaluated.
            List<Node> openList = new List<Node>();
            // closedSet holds positions that have already been evaluated.
            HashSet<(int, int)> closedSet = new HashSet<(int, int)>();

            // Create the start node.
            Node startNode = new Node(start, 0, Manhattan(start, goal), null, null);
            openList.Add(startNode);

            while (openList.Count > 0)
            {
                // Sort the open list by the F score (G+H); pick the node with the lowest F.
                openList.Sort((a, b) => a.F.CompareTo(b.F));
                Node current = openList[0];
                openList.RemoveAt(0);

                // If the goal is reached, reconstruct the path.
                if (current.Position.Equals(goal))
                {
                    List<Direction> path = new List<Direction>();
                    while (current.Parent != null && current.DirectionFromParent.HasValue)
                    {
                        path.Insert(0, current.DirectionFromParent.Value);
                        current = current.Parent;
                    }
                    return path;
                }

                closedSet.Add(current.Position);

                // Explore all four adjacent directions.
                foreach (var kvp in directionDeltas)
                {
                    Direction dir = kvp.Key;
                    var delta = kvp.Value;
                    var neighborPos = (current.Position.x + delta.dx, current.Position.y + delta.dy);

                    // Skip if already evaluated.
                    if (closedSet.Contains(neighborPos))
                        continue;
                    // Only consider neighbor cells that are known in the internal map.
                    if (!map.ContainsKey(neighborPos))
                        continue;
                    // Skip if the cell is not walkable.
                    if (!IsWalkable(map[neighborPos]))
                        continue;

                    int tentativeG = current.G + 1;
                    Node neighborNode = new Node(neighborPos, tentativeG, Manhattan(neighborPos, goal), current, dir);

                    // If a node for this neighbor exists with a lower G cost, skip it.
                    Node existing = openList.Find(n => n.Position.Equals(neighborPos));
                    if (existing != null && tentativeG >= existing.G)
                        continue;

                    // Otherwise, remove any duplicate and add the new neighbor node.
                    openList.RemoveAll(n => n.Position.Equals(neighborPos));
                    openList.Add(neighborNode);
                }
            }
            // Return null if no path is found.
            return null;
        }


        /// <summary>
        /// Manhattan distance heuristic.
        /// </summary>
        private int Manhattan((int x, int y) a, (int x, int y) b)
        {
            return Math.Abs(a.x - b.x) + Math.Abs(a.y - b.y);
        }

        /// <summary>
        /// Determines whether a tile is walkable.
        /// We consider a tile walkable if it explicitly has the CanWalk flag or is the Goal.
        /// Adjust this logic if you need to avoid hazards like Hurt or Enemy.
        /// </summary>
        private bool IsWalkable(TileType tile)
        {
            // Allow moving onto the goal even if it lacks CanWalk.
            if ((tile & TileType.Goal) == TileType.Goal)
                return true;
            return (tile & TileType.CanWalk) == TileType.CanWalk;
        }

        /// <summary>
        /// A simple exploration routine: if the goal isn’t yet visible,
        /// try moving into any adjacent unknown cell. If all adjacent cells are known,
        /// choose any walkable neighbor.
        /// </summary>
        private async Task Explore()
        {
            // Create a list for potential moves into unknown adjacent cells.
            List<Direction> unknownMoves = new List<Direction>();
            foreach (var kvp in directionDeltas)
            {
                var newPos = (currentPosition.x + kvp.Value.dx, currentPosition.y + kvp.Value.dy);
                if (!map.ContainsKey(newPos))
                {
                    unknownMoves.Add(kvp.Key);
                }
            }

            // If there are any unknown adjacent cells, pick one randomly.
            if (unknownMoves.Count > 0)
            {
                Direction chosen = unknownMoves[new Random().Next(unknownMoves.Count)];
                var response = await io.PlayerMove(chosen);
                if (response != null && response.Success)
                {
                    currentPosition = (
                        currentPosition.x + directionDeltas[chosen].dx,
                        currentPosition.y + directionDeltas[chosen].dy);
                    // Wait a bit after the move.
                    await Task.Delay(100);
                    return;
                }
            }

            // If all adjacent cells are known, try to move to any walkable neighbor.
            List<Direction> walkableMoves = new List<Direction>();
            foreach (var kvp in directionDeltas)
            {
                var newPos = (currentPosition.x + kvp.Value.dx, currentPosition.y + kvp.Value.dy);
                if (map.ContainsKey(newPos) && IsWalkable(map[newPos]))
                {
                    walkableMoves.Add(kvp.Key);
                }
            }

            if (walkableMoves.Count > 0)
            {
                Direction chosen = walkableMoves[new Random().Next(walkableMoves.Count)];
                var response = await io.PlayerMove(chosen);
                if (response != null && response.Success)
                {
                    currentPosition = (
                        currentPosition.x + directionDeltas[chosen].dx,
                        currentPosition.y + directionDeltas[chosen].dy);
                    await Task.Delay(100);
                    return;
                }
            }

            // If no move was possible, wait briefly before retrying.
            await Task.Delay(100);
        }


        /// <summary>
        /// Internal class for A* search nodes.
        /// </summary>
        class Node
        {
            public (int x, int y) Position;
            public int G; // cost from start
            public int H; // heuristic cost to goal
            public Node Parent;
            public Direction? DirectionFromParent;

            public int F => G + H;

            public Node((int x, int y) position, int g, int h, Node parent, Direction? direction)
            {
                Position = position;
                G = g;
                H = h;
                Parent = parent;
                DirectionFromParent = direction;
            }
        }

        /// <summary>
        /// Comparer for sorting nodes in the open set by their F cost, then H.
        /// </summary>
        class NodeComparer : IComparer<Node>
        {
            public int Compare(Node a, Node b)
            {
                int comp = a.F.CompareTo(b.F);
                if (comp == 0)
                {
                    comp = a.H.CompareTo(b.H);
                }
                if (comp == 0)
                {
                    // Ensure distinct nodes aren’t treated as equal.
                    return a.Position.Equals(b.Position) ? 0 : 1;
                }
                return comp;
            }
        }
    }
}
