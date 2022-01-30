using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathFindingInput
{
    public struct CellSate
    {
        public bool Calculated;
        public Vector2Int MapPos;
        public List<Vector2Int> BestWay;
    }

    public readonly CellSate[,] Map;
    public Vector2Int PlayerPos;
    public readonly Vector2Int[] FoesPos;
    public Vector2Int RequestedPosition;

    public PathFindingInput(MapCell[,] map, Player player, IEnumerable<Foe> foes, Vector2Int requestedPosition)
    {
        RequestedPosition = requestedPosition;
        PlayerPos = player.mapPos;
        FoesPos = foes.Select(foe => foe.mapPos).ToArray();
        var height = map.GetLength(0);
        var width = map.GetLength(1);
        
        Map = new CellSate[height, width];

        for (var y = 0; y < height; ++y)
        {
            for (var x = 0; x < width; ++x)
            {
                Map[y, x] = new CellSate
                {
                    Calculated = false,
                    MapPos = new Vector2Int(x, y),
                    BestWay = null
                };
            }
        }
    }
}