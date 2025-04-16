using System.Collections.Generic;
using System.Linq;

namespace GDFramework.Mission
{
    public class MissionChainHandle
    {
        private readonly MissionChain chain;
        private readonly Dictionary<string, MissionNode> activeNodes = new Dictionary<string, MissionNode>();
        private readonly Queue<MissionNode> buffer = new Queue<MissionNode>();

        public bool IsCompleted => activeNodes.Count == 0;

        public MissionChainHandle(MissionChain chain)
        {
            this.chain = chain;
            
            /* execute prime node */
            if (chain.primeNode != null)
                ExecuteNode(chain.primeNode as BaseNode);
        }

        public void FlushBuffer(System.Action<MissionPrototype<object>> deployer)
        {
            if (buffer.Count == 0) return;
            while (buffer.Count > 0)
            {
                var node = buffer.Dequeue();
                var missionProto = node.MissionProto;
                activeNodes.Add(missionProto.MissionId, node);
                deployer(missionProto);
            }
        }

        public void OnMissionComplete(string missionId, bool continues)
        {
            if (!activeNodes.Remove(missionId, out var node)) return;
            
            /* execute all available output connections */
            if (continues)
            {
                foreach (var outConnection in node.outConnections.Where(c => ((BaseConnection)c).IsAvailable))
                    ExecuteNode(outConnection.targetNode as BaseNode);
            }
        }

        /// <summary>execute given node</summary>
        public void ExecuteNode(BaseNode node)
        {
            if (node is null) return;
            switch (node)
            {
                /* execute action node */
                case ActionNode actionNode:
                    actionNode.Execute();
                    break;
                
                /* execute mission node, add output prototype to buffer queue */
                case MissionNode missionNode:
                    if (activeNodes.ContainsKey(missionNode.MissionId)) return;
                    buffer.Enqueue(missionNode);
                    break;
            }
        }
    }
}