using UnityEngine;

namespace Shooting
{
    public class Checkpoints : MonoBehaviour
    {
        // 检查点的编号，用于区分不同的检查点
        public int m_CheckpointNumber = 0;
        
        // 检查点是否已经被激活
        [HideInInspector]
        public bool m_IsActivated = false;

        // 玩家复活的位置
        public Transform m_SpawnPoint;

        // 检查点的基础物体数组，用于显示不同的状态
        public GameObject[] m_Bases;

        // 激活检查点时显示的粒子效果
        public GameObject m_ActivateParticle;
    }
}