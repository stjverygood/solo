using Godot;
using System.Collections.Generic;

namespace Solo.Scripts.Global
{
    public static class GlobalHelper
    {
        public static Node2D GetNearestNode(Vector2 worldPos, List<Node2D> nodeList)
        {
            float minDistSq = float.MaxValue; // 先设为一个极大值
            Node2D targetNode = null;
            foreach (Node2D curNode in nodeList)
            {
                if (curNode == null)
                    continue;
                float curDistSq = worldPos.DistanceSquaredTo(curNode.GlobalPosition);
                if (curDistSq < minDistSq)
                {
                    minDistSq = curDistSq;
                    targetNode = curNode;
                }
            }
            return targetNode;
        }
    }
}
