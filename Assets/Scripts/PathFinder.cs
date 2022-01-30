using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using UnityEngine;

public static class PathFinder
{
    private static readonly Vector2Int[] AdjacentVec =
    {
        new Vector2Int(-1, 0),
        new Vector2Int(1, 0),
        new Vector2Int(0, 1),
        new Vector2Int(0, -1)
    };
        
    [return: MaybeNull]
    public static List<Vector2Int> FindPlayerBestWay(PathFindingInput input)
    {
        return FindPlayerBestWayR(input, input.PlayerPos, new List<Vector2Int>());
    }
    
    private static bool IsValid(PathFindingInput input, Vector2Int pos, ICollection<Vector2Int> path)
    {
        return !(
                pos.x < 0 ||
                pos.y < 0 ||
                pos.x >= input.Map.GetLength(1) ||
                pos.y >= input.Map.GetLength(0) ||
                path.Contains(pos) ||
                input.FoesPos.Contains(pos)
            );
    }
    
    private static List<Vector2Int> FindPlayerBestWayR(PathFindingInput input, Vector2Int currentPos, List<Vector2Int> path)
    {
        var cell = input.Map[currentPos.y, currentPos.x];
        
        if (cell.Calculated)
        {
            return cell.BestWay;
        }

        if (currentPos == input.RequestedPosition)
        {
            cell.Calculated = true;
            cell.BestWay = new List<Vector2Int>() { currentPos };
            input.Map[currentPos.y, currentPos.x] = cell;
            return cell.BestWay;
        }
        
        path.Add(currentPos);
        var adjacentPos = AdjacentVec.Where(vec => IsValid(input, vec + currentPos, path)).ToList();
        adjacentPos.Sort((i, j) =>  Vector2Int.Distance(currentPos + i, input.RequestedPosition) < Vector2Int.Distance(currentPos + j, input.RequestedPosition) ? -1 : 1);
        List<Vector2Int> bestWay = null;
        foreach (var adj in adjacentPos)
        {
            var search = FindPlayerBestWayR(input, currentPos + adj, path);
            if (search != null && (bestWay is null || search.Count < bestWay.Count))
                bestWay = search;
        }
        cell.Calculated = true;
        if (bestWay != null)
        {
            cell.BestWay = new List<Vector2Int>() { currentPos };
            cell.BestWay.AddRange(bestWay);
        }
        else
        {
            cell.BestWay = null;
        }
        path.Remove(currentPos);
        input.Map[currentPos.y, currentPos.x] = cell;
        return cell.BestWay;
    }
}
