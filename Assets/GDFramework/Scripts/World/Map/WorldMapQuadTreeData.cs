using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace GDFramework.World.Map
{
    public class WorldMapQuadTreeData
    {
        public enum EShowGOType
        {
            None = 0,
            ShowInFrustum = 1,
            ShowNotInFrustum = 2,
            All = -1,
        }

        /// <summary>
        /// 四叉树内的对象
        /// </summary>
        public GameObject[] Goes;
        
        /// <summary>
        /// 原始四叉树的根对象
        /// </summary>
       public GameObject GoesRoot;

        /// <summary>
        /// 视锥水平剔除空间扩大的 unit 单位
        /// </summary>
        public float FrustumHorPadding = 5;

        /// <summary>
        /// 视锥分段的多层AABB的级别
        /// </summary>
        public float FrustumAabbLevel = 3;

        public QuadTree<GameObject> QuadTree;

        public List<GameObject> QuadTreeSelectRetHelperList;

        public List<AABB_Check> CameraAabbList = new List<AABB_Check>();

        public void InitAabbTree()
        {
            // 计算整个场景的 bounds
            var scene_bounds = CalculateSceneAABB();

            // 创建四叉树对象
            QuadTree = new QuadTree<GameObject>(scene_bounds);
            QuadTreeSelectRetHelperList = new List<GameObject>();

            // 给 qt 插入每一个
            InsertQTObjs();
        }

        public void CaculateAabb()
        {
            SelectQT();
            UpdateGoVisible();
        }
        
        /// <summary>
        /// 计算场景AABB
        /// </summary>
        /// <returns></returns>
        private Bounds CalculateSceneAABB()
        {
            var ret = new Bounds();
            foreach (var go in Goes)
            {
                var renderer = go.GetComponent<Renderer>();
                if (renderer != null)
                {
                    ret.Encapsulate(renderer.bounds);
                }
            }

            return ret;
        }
        
        private void InsertQTObjs()
        {
            // 给 qt 插入每一个
            foreach (var go in Goes)
            {
                var renderer = go.GetComponent<Renderer>();
                if (renderer != null)
                {
                    QuadTree.Insert(go, renderer.bounds);
                }
            }
        }

        private void SelectQT()
        {
            QuadTree.Select(CameraAabbList, QuadTreeSelectRetHelperList, true);
        }

        private void UpdateGoVisible()
        {
            GoesRoot.SetActive(true);
            foreach (var go in Goes)
            {
                go.SetActive(false);
            }

            if (QuadTree != null && QuadTreeSelectRetHelperList != null)
            {
                // 概要筛选
                QuadTree.Select(CameraAabbList, QuadTreeSelectRetHelperList, true);
                foreach (var go in QuadTreeSelectRetHelperList)
                {
                    go.SetActive(true);
                }
            }
        }
    }
}