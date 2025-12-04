using UnityEngine;

namespace Shooting.GamePlay
{
    public class TargetsObject : MonoBehaviour
    {
        // 目标物体的中心位置
        public Transform m_TargetCenter;

        // Start 方法在游戏开始时调用一次，通常用于初始化
        void Start()
        {
            // 将当前目标物体添加到 TargetsControl 中进行管理
            TargetController.m_Main.AddTarget(this);

            // 如果 m_TargetCenter 为空，则将目标物体的 transform 作为目标中心
            if (m_TargetCenter == null)
            {
                m_TargetCenter = transform;
            }
        }
    }
}