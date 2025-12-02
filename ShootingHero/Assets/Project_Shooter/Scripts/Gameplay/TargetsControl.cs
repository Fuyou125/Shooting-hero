using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shooter.Gameplay
{
    // 该类用于管理游戏中的所有目标物体
    public class TargetsControl : MonoBehaviour
    {
        // 静态变量，保存当前的 TargetsControl 实例（单例模式）
        public static TargetsControl m_Main;

        // 用于存储所有目标物体的列表
        [HideInInspector]
        public List<TargetObject> m_Targets;

        // 在 Awake 方法中初始化 m_Main 和 m_Targets
        void Awake()
        {
            // 设置 m_Main 为当前 TargetsControl 实例
            m_Main = this;

            // 初始化目标列表
            m_Targets = new List<TargetObject>();
        }

        // 向目标列表中添加目标物体
        public void AddTarget(TargetObject obj)
        {
            // 将目标物体添加到 m_Targets 列表中
            m_Targets.Add(obj);
        }

        // 从目标列表中移除目标物体
        public void RemoveTarget(TargetObject obj)
        {
            // 从 m_Targets 列表中移除指定的目标物体
            m_Targets.Remove(obj);
        }
    }
}