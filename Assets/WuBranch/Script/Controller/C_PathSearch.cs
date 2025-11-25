using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class C_PathSearch
{
    public static List<M_MapPosition> GetPath(List<List<int>> path, M_MapPosition start, M_MapPosition end)
    {
        // A*で道を探す
        // スタート地点とゴールはちゃんとマップに入っているかを確認する
        if (!CheckInMap<int>(path, start) || !CheckInMap<int>(path, end))
            return null;

        // 全体マップ
        List<List<Node>> searchMap = InitMap(path);
        // これから探すノードと決めたノードを保存するリストを用意する
        List<Node> nextNodes = new();
        List<Node> pathNodes = new();

        // スタート地点
        Node startPoint = searchMap[start.Y][start.X];
        // 目標地点
        Node goalPoint = searchMap[end.Y][end.X];

        // スタート地点のコストを0にする
        startPoint.ActualCost = 0;

        // スタート地点を探すリストに入れる
        nextNodes.Add(startPoint);

        while (nextNodes.Count > 0)
        {
            // 探すリストから一番コストが低いノードを探す
            Node current = GetSmallestCostNode(nextNodes);

            // 確定なので探すリストから削除、決めたノードに保存
            nextNodes.Remove(current);
            pathNodes.Add(current);

            // ゴールに着いた
            if (current == goalPoint)
                return RetracePath(ref startPoint, ref goalPoint);

            // 次の探すべきノードを用意
            List<Node> nextNodesTmp = PrepareNextNodes(searchMap, nextNodes, pathNodes, current, goalPoint);
            nextNodes.AddRange(nextNodesTmp);
        }
        return new();
    }

    /// <summary>
    /// 場所がマップに入っているか
    /// </summary>
    /// <param name="map">マップ</param>
    /// <param name="pos">場所</param>
    /// <returns>true: はい、false: いいえ</returns>
    private static bool CheckInMap<T>(List<List<T>> map, M_MapPosition pos)
    {
        if (pos.Y < 0 || pos.Y >= map.Count || pos.X < 0 || pos.X >= map[pos.Y].Count)
            return false;
        return true;
    }

    /// <summary>
    /// マップの初期化
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    private static List<List<Node>> InitMap(List<List<int>> path)
    {
        List<List<Node>> map = new();
        for (int rowIndex = 0; rowIndex < path.Count; rowIndex++)
        {
            List<Node> rowNode = new();
            for (int colIndex = 0; colIndex < path[rowIndex].Count; colIndex++)
            {
                M_MapPosition pos = new(colIndex, rowIndex);
                Node node = new(pos, path[rowIndex][colIndex], path[rowIndex][colIndex] != (int)PathStructure.Blocked);
                rowNode.Add(node);
            }
            map.Add(rowNode);
        }
        return map;
    }

    /// <summary>
    /// リストから一番コストが低いノードを探す
    /// </summary>
    /// <param name="nextNode">リスト</param>
    /// <returns></returns>
    private static Node GetSmallestCostNode(List<Node> nodes)
    {
        if (nodes.Count == 0)
            return null;

        Node target = nodes.FirstOrDefault();
        for (int index = 1; index < nodes.Count; index++)
        {
            if (nodes[index].FinalCost < target.FinalCost ||
                (nodes[index].FinalCost == target.FinalCost && nodes[index].HeuristicCost < target.HeuristicCost))
                target = nodes[index];
        }
        return target;
    }

    /// <summary>
    /// スタートからゴールまで通った道を準備
    /// </summary>
    /// <param name="startPoint">スタート地点</param>
    /// <param name="goalPoint">ゴール</param>
    /// <returns>通った道</returns>
    private static List<M_MapPosition> RetracePath(ref Node startPoint, ref Node goalPoint)
    {
        List<M_MapPosition> path = new();
        Node current = goalPoint;

        // ゴールから通った地点をリストに追加
        while (current != startPoint)
        {
            path.Add(current.Pos);
            current = current.Parent;
        }

        // スタート地点も追加
        path.Add(startPoint.Pos);
        // 反転
        path.Reverse();

        return path;
    }

    private static List<Node> PrepareNextNodes(List<List<Node>> searchMap, List<Node> pathNodes, List<Node> nextNodes, Node current, Node goal)
    {
        // 上下左右の位置
        M_MapPosition[] dirs = {
            new(1, 0),
            new(-1, 0),
            new(0, 1),
            new(0, -1)
        };
        List<Node> nodes = new();
        foreach (var dir in dirs)
        {
            // 次の場所
            M_MapPosition neighborPoint = new(current.Pos.X + dir.X, current.Pos.Y + dir.Y);

            // マップに入っているか
            if (!CheckInMap<Node>(searchMap, neighborPoint))
                continue;

            // 実際のノード
            Node neighbor = searchMap[neighborPoint.Y][neighborPoint.X];

            // 通れないもしくはすでに決めたノードリストに入ったなら無視
            if (!neighbor.CanGo || pathNodes.Contains(neighbor))
                continue;
            // コストを計算
            int tentativeG = CaculateCost(current);
            // ノードのコストが計算したコストより多い、もしくは
            if (tentativeG < neighbor.ActualCost || !nextNodes.Contains(neighbor))
            {
                neighbor.ActualCost = tentativeG;
                neighbor.HeuristicCost = CaculateHeuristicCost(neighbor, goal);
                neighbor.Parent = current;

                if (!nextNodes.Contains(neighbor))
                    nodes.Add(neighbor);
            }
        }
        return nodes;
    }

    /// <summary>
    /// 今の位置までのコストを計算
    /// </summary>
    /// <param name="current">今の位置</param>
    /// <returns>コスト</returns>
    private static int CaculateCost(Node current)
    {
        return current.ActualCost + 1 + current.CurrentCost;
    }

    /// <summary>
    /// 推測のコストを計算
    /// </summary>
    /// <param name="current">今の位置</param>
    /// <param name="goal">目標地点</param>
    /// <returns></returns>
    private static int CaculateHeuristicCost(Node current, Node goal)
    {
        return Mathf.Abs(current.Pos.X - goal.Pos.X) + Mathf.Abs(current.Pos.Y - goal.Pos.Y);
    }
}

public class Node
{
    /// <summary>
    /// 今の位置
    /// </summary>
    public M_MapPosition Pos;

    /// <summary>
    /// 今の位置のコスト
    /// </summary>
    public int CurrentCost;

    /// <summary>
    /// ここまでかかるコスト
    /// </summary>
    public int ActualCost = int.MaxValue;

    /// <summary>
    /// 推測のコスト
    /// </summary>
    public int HeuristicCost;

    /// <summary>
    /// 総コスト
    /// </summary>
    public int FinalCost => ActualCost + HeuristicCost;

    /// <summary>
    /// 通れるか
    /// </summary>
    public bool CanGo;

    /// <summary>
    /// 前のノード
    /// </summary>
    public Node Parent;

    public Node(M_MapPosition pos, int cost, bool canGo)
    {
        Pos = pos;
        CanGo = canGo;
        CurrentCost = cost;
    }
}