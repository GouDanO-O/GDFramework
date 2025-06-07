using System;
using System.Collections.Generic;
using GDFrameworkExtend.Data;
using NUnit.Framework;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.World
{
    [Serializable,LabelText("节点数据")]
    public class NodeData
    {
        [ShowInInspector]
        private NodeDataPersistent _nodeDataPersistent;
        
        [ShowInInspector]
        private NodeDataTemporary _nodeDataTemporary;
        
        public NodeDataPersistent NodeDataPersistent
        {
            get
            {
                return _nodeDataPersistent;
            }
        }

        public NodeDataTemporary NodeDataTemporary
        {
            get
            {
                return _nodeDataTemporary;
            }
        }

        public NodeData(NodeData nodeData)
        {
            
        }
    }
    



}