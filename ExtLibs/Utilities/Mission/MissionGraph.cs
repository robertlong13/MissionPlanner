using System.Collections.Generic;
using static MissionPlanner.Utilities.Mission.CommandUtils;

namespace MissionPlanner.Utilities.Mission
{
    public sealed class MissionNode
    {
        public int MissionIndex { get; }
        public Locationwp Command { get; }
        public bool IsTerminal { get; internal set; } = false;
        public List<int> IncomingEdges { get; } = new List<int>();
        public List<int> OutgoingEdges { get; } = new List<int>();

        public MissionNode(int missionIndex, Locationwp command)
        {
            MissionIndex = missionIndex;
            Command = command;
        }
    }

    public sealed class MissionEdge
    {
        public int FromNode { get; }
        public int ToNode { get; }
        public bool IsJump { get; }
        public int? JumpRepeat { get; }

        public MissionEdge(int fromNode, int toNode, bool isJump, int? jumpRepeat = null)
        {
            FromNode = fromNode;
            ToNode = toNode;
            IsJump = isJump;
            JumpRepeat = jumpRepeat;
        }

        public override string ToString()
        {
            return $"{(IsJump ? "Jump" : "Seq")}: {FromNode} -> {ToNode}";
        }
    }

    public sealed class MissionGraph
    {
        public IReadOnlyList<MissionNode> Nodes { get; }
        public IReadOnlyList<MissionEdge> Edges { get; }

        public MissionGraph(List<MissionNode> nodes,
                            List<MissionEdge> edges)
        {
            Nodes = nodes;
            Edges = edges;
        }
    }

    public class MissionGraphBuilder
    {
        public static MissionGraph Build(PointLatLngAlt home, List<Locationwp> missionitems)
        {
            var nodes = new List<MissionNode>();
            var edges = new List<MissionEdge>();

            // Map from mission index (0-based) to node index. -1 means not a node.
            var missionToNode = new int[missionitems.Count];
            for (int i = 0; i < missionToNode.Length; i++)
            {
                missionToNode[i] = -1;
            }

            var jumpTags = new Dictionary<int, int>(); // key=tag, value=mission index

            if (home != PointLatLngAlt.Zero)
            {

                var homeCopy = new Locationwp
                {
                    id = 0,
                    lat = home.Lat,
                    lng = home.Lng,
                    alt = (float)home.Alt,
                    frame = (byte)MAVLink.MAV_FRAME.GLOBAL,
                };
                nodes.Add(new MissionNode(-1, homeCopy));
            }

            for (int i = 0; i < missionitems.Count; i++)
            {
                var cmd = missionitems[i];
                if (IsNode(cmd))
                {
                    var node = new MissionNode(i, cmd)
                    {
                        IsTerminal = IsTerminal(cmd)
                    };
                    nodes.Add(node);
                    missionToNode[i] = nodes.Count - 1;
                }
                if (IsJumpTag(cmd))
                {
                    jumpTags[GetJumpTag(cmd)] = i;
                }
                if (IsJumpCommand(cmd) && GetJumpCount(cmd) < 0 && nodes.Count > 0)
                {
                    nodes[nodes.Count - 1].IsTerminal = true;
                }
            }

            // Sequential edges over nodes
            for (int i = 0; i < nodes.Count - 1; i++)
            {
                if (nodes[i].IsTerminal)
                {
                    continue;
                }
                edges.Add(new MissionEdge(i, i + 1, false));
                nodes[i].OutgoingEdges.Add(edges.Count - 1);
                nodes[i + 1].IncomingEdges.Add(edges.Count - 1);
            }

            // Jump edges
            for (int i = 0; i < missionitems.Count; i++)
            {
                var item = missionitems[i];
                int jumpTargetMissionIndex;
                if (IsDoJump(item))
                {
                    jumpTargetMissionIndex = GetJumpTarget(item) - 1; // p1 is 1-based seq
                }
                else if (IsDoJumpTag(item))
                {
                    var tag = GetJumpTarget(item);
                    if (!jumpTags.TryGetValue(tag, out jumpTargetMissionIndex))
                        continue; // invalid tag
                }
                else
                {
                    continue; // not a jump
                }

                if (jumpTargetMissionIndex < 0 || jumpTargetMissionIndex >= missionitems.Count)
                {
                    continue; // invalid target
                }

                int srcNodeIndex = FindLastNodeBeforeOrAt(i, missionToNode);
                if (srcNodeIndex < 0 || srcNodeIndex >= nodes.Count)
                {
                    continue; // source is not a node
                }

                int destNodeIndex = FindFirstNodeAtOrAfter(jumpTargetMissionIndex, missionToNode);
                if (destNodeIndex < 0 || destNodeIndex >= nodes.Count)
                {
                    continue; // target is not a node
                }

                int jumpRepeat = GetJumpCount(item);
                if (jumpRepeat == 0)
                {
                    continue; // no jump
                }

                edges.Add(new MissionEdge(
                    srcNodeIndex,
                    destNodeIndex,
                    true,
                    jumpRepeat));
                nodes[srcNodeIndex].OutgoingEdges.Add(edges.Count - 1);
                nodes[destNodeIndex].IncomingEdges.Add(edges.Count - 1);
            }

            return new MissionGraph(nodes, edges);
        }

        static int FindLastNodeBeforeOrAt(int missionIndex, int[] missionToNode)
        {
            if (missionToNode == null || missionToNode.Length == 0 || missionIndex < 0)
            {
                return -1;
            }

            if (missionIndex >= missionToNode.Length)
            {
                missionIndex = missionToNode.Length - 1;
            }

            for (int i = missionIndex; i >= 0; i--)
            {
                var nodeIndex = missionToNode[i];
                if (nodeIndex != -1)
                {
                    return nodeIndex;
                }
            }

            return -1;
        }

        static int FindFirstNodeAtOrAfter(int missionIndex, int[] missionToNode)
        {
            if (missionToNode == null || missionToNode.Length == 0 || missionIndex >= missionToNode.Length)
            {
                return -1;
            }

            if (missionIndex < 0)
            {
                missionIndex = 0;
            }

            for (int i = missionIndex; i < missionToNode.Length; i++)
            {
                var nodeIndex = missionToNode[i];
                if (nodeIndex != -1)
                {
                    return nodeIndex;
                }
            }

            return -1;
        }
    }
}
