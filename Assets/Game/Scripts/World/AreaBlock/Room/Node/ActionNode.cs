using System;
using System.Collections.Generic;
using GDFrameworkExtend.Data;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Game.World
{
    [Serializable,LabelText("只描述静态节点事实")]
    public class NodeDto
    {
        [LabelText("节点ID(对玩家不可见)")]
        public string nodeId;
        
        [LabelText("节点名称")]
        public string nodeName;
        
        [LabelText("子节点ID")]
        public List<string> childIds = new();  
        
        [LabelText("")]
        public List<EdgeDto> edges = new(); 
        
        [LabelText("当前节点所处的位置")]
        public Vector2 editorPos;
        
        [LabelText("当前节点触发时会发生的效果")]
        public ActionDto actionDto;
    }
    
    [Serializable,LabelText("行为触发时发生的效果")]
    public class ActionDto
    {
        [LabelText("震动屏幕")]
        public bool willShakeScreen = false;

        [ShowIf("willShakeScreen"),LabelText("震动强度")]
        public int shakeSceenStrength;

        [LabelText("触发时播放的音频")]
        public AudioClip audioClip;

        [LabelText("触发时生成的粒子特效")]
        public GameObject particleObject;
        
        [ShowIf("$particleObject"),LabelText("生成粒子特效的位置)(默认为触发节点周围)")]
        public Vector3 particlePos = Vector3.zero;
    }
    
    [Serializable,LabelText("")]
    public class EdgeDto
    {
        public string targetId;
        
        public string conditionFlag;
    }

    public class NodeRuntime
    {
        public NodeDto NodeDto;
        
        [LabelText("是否可见")]
        public bool IsVisible;
        
        [LabelText("是否触发")]
        public bool IsTriggered;
    }
    
}

