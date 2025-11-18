using System.Collections.Generic;
using static MissionPlanner.Utilities.Mission.CommandUtils;

namespace MissionPlanner.Utilities.Mission
{
    public sealed class MissionNode
    {
        public int MissionIndex { get; }
        public Locationwp Command { get; }
        public bool IsTerminal { get; internal set; } = false;
        public List<MissionEdge> IncomingEdges { get; } = new List<MissionEdge>();
        public List<MissionEdge> OutgoingEdges { get; } = new List<MissionEdge>();

        public MissionNode(int missionIndex, Locationwp command)
        {
            MissionIndex = missionIndex;
            Command = command;
        }
    }

    public sealed class MissionEdge
    {
        public MissionNode FromNode { get; }
        public MissionNode ToNode { get; }
        public bool IsJump { get; }
        public int? JumpRepeat { get; }

        public MissionEdge(MissionNode fromNode, MissionNode toNode, bool isJump, int? jumpRepeat = null)
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

            // Map from mission index (0-based) to node
            var missionToNode = new MissionNode[missionitems.Count];

            var jumpTags = new Dictionary<int, int>(); // key=tag, value=mission index
            var landNodes = new List<MissionNode>();

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
                    missionToNode[i] = node;
                    if (IsLand(cmd))
                    {
                        landNodes.Add(node);
                    }
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
                var node1 = nodes[i];
                var node2 = nodes[i + 1];
                if (node1.IsTerminal)
                {
                    continue;
                }
                if (IsLand(node1.Command) && !IsTakeoff(node2.Command))
                {
                    continue;
                }
                var edge = new MissionEdge(node1, nodes[i + 1], false);
                edges.Add(edge);
                node1.OutgoingEdges.Add(edge);
                node2.IncomingEdges.Add(edge);
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
                    {
                        continue; // invalid tag
                    }
                }
                else
                {
                    continue; // not a jump
                }

                if (jumpTargetMissionIndex < 0 || jumpTargetMissionIndex >= missionitems.Count)
                {
                    continue; // invalid target
                }

                var srcNode = FindLastNodeBeforeOrAt(i, missionToNode);
                if (srcNode == null)
                {
                    continue; // source is not a node
                }

                var destNode = FindFirstNodeAtOrAfter(jumpTargetMissionIndex, missionToNode);
                if (destNode == null)
                {
                    continue; // target is not a node
                }

                if (IsLand(srcNode.Command) && !IsTakeoff(destNode.Command))
                {
                    continue;
                }

                int jumpRepeat = GetJumpCount(item);
                if (jumpRepeat == 0)
                {
                    continue; // no jump
                }

                var edge = new MissionEdge(srcNode, destNode, true, jumpRepeat);
                edges.Add(edge);
                srcNode.OutgoingEdges.Add(edge);
                destNode.IncomingEdges.Add(edge);
            }

            // Mark all land nodes without a takeoff (no outgoing edges) as terminal
            foreach (var landNode in landNodes)
            {
                if (landNode.OutgoingEdges.Count == 0)
                {
                    landNode.IsTerminal = true;
                }
            }

            return new MissionGraph(nodes, edges);
        }

        static MissionNode FindLastNodeBeforeOrAt(int missionIndex, MissionNode[] missionToNode)
        {
            if (missionToNode == null || missionToNode.Length == 0 || missionIndex < 0)
            {
                return null;
            }

            if (missionIndex >= missionToNode.Length)
            {
                missionIndex = missionToNode.Length - 1;
            }

            for (int i = missionIndex; i >= 0; i--)
            {
                var node = missionToNode[i];
                if (node != null)
                {
                    return node;
                }
            }

            return null;
        }

        static MissionNode FindFirstNodeAtOrAfter(int missionIndex, MissionNode[] missionToNode)
        {
            if (missionToNode == null || missionToNode.Length == 0 || missionIndex >= missionToNode.Length)
            {
                return null;
            }

            if (missionIndex < 0)
            {
                missionIndex = 0;
            }

            for (int i = missionIndex; i < missionToNode.Length; i++)
            {
                var node = missionToNode[i];
                if (node != null)
                {
                    return node;
                }
            }

            return null;
        }
    }
}
