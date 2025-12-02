using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shooter.Gameplay
{
    // 检查点控制类，用于管理游戏中的多个检查点
    public class CheckpointControl : MonoBehaviour
    {
        // 静态引用，方便访问当前的 CheckpointControl 实例
        public static CheckpointControl m_Main;

        // 存储所有检查点的数组
        public Checkpoint[] m_Checkpoints;

        // Awake 是 Unity 中的生命周期方法，在游戏开始时被调用
        void Awake()
        {
            // 将当前实例赋值给 m_Main，使得其他类可以方便地访问该实例
            m_Main = this;
        }
    }
}