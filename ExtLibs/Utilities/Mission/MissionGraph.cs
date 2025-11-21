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
        public readonly PointLatLngAlt Home;

        private readonly IReadOnlyList<MissionNode> firstAtOrAfter;
        private readonly IReadOnlyList<MissionNode> lastAtOrBefore;

        public MissionGraph(List<MissionNode> nodes,
                            List<MissionEdge> edges,
                            PointLatLngAlt home,
                            List<MissionNode> firstAtOrAfter,
                            List<MissionNode> lastAtOrBefore)
        {
            Nodes = nodes;
            Edges = edges;
            Home = home;
            this.firstAtOrAfter = firstAtOrAfter;
            this.lastAtOrBefore = lastAtOrBefore;
        }

        public MissionNode FirstNodeAtOrAfter(int missionIndex)
        {
            if (missionIndex >= firstAtOrAfter.Count)
            {
                return null;
            }
            if (missionIndex < 0)
            {
                missionIndex = 0;
            }
            return firstAtOrAfter[missionIndex];
        }

        public MissionNode LastNodeAtOrBefore(int missionIndex)
        {
            if (missionIndex < 0)
            {
                return null;
            }
            if (missionIndex >= lastAtOrBefore.Count)
            {
                missionIndex = lastAtOrBefore.Count - 1;
            }
            return lastAtOrBefore[missionIndex];
        }
    }

    public class MissionGraphBuilder
    {
        public static MissionGraph Build(PointLatLngAlt home, List<Locationwp> missionitems)
        {
            var nodes = new List<MissionNode>();
            var edges = new List<MissionEdge>();

            // Map from mission index (0-based) to node indices
            var missionToNode = new Dictionary<int, MissionNode>();

            var jumpTags = new Dictionary<int, int>(); // key=tag, value=mission index
            var landNodes = new List<MissionNode>();

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

            var firstAtOrAfter = new List<MissionNode>(new MissionNode[missionitems.Count]);
            MissionNode next = null;
            for (int i = missionitems.Count - 1; i >= 0; i--)
            {
                if (missionToNode.TryGetValue(i, out var node))
                {
                    next = node;
                }
                firstAtOrAfter[i] = next;
            }

            var lastAtOrBefore = new List<MissionNode>(new MissionNode[missionitems.Count]);
            MissionNode last = null;
            for (int i = 0; i < missionitems.Count; i++)
            {
                if (missionToNode.TryGetValue(i, out var node))
                {
                    last = node;
                }
                lastAtOrBefore[i] = last;
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

                var srcNode = lastAtOrBefore[i];
                if (srcNode == null)
                {
                    continue; // source is not a node
                }

                var destNode = firstAtOrAfter[jumpTargetMissionIndex];
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

            return new MissionGraph(nodes, edges, home, firstAtOrAfter, lastAtOrBefore);
        }
    }
}
