using UnityEngine;

namespace Shooting.Gameplay
{
    // 检查点类，用于控制玩家经过检查点时的行为
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

        // Update 每帧调用，用于检测玩家是否靠近检查点
        void Update()
        {
            // 如果检查点没有被激活
            if (!m_IsActivated)
            {
                // 检查玩家与当前检查点的距离，如果小于等于6个单位，则激活检查点
                if (Vector3.Distance(PlayerCharacter.m_Current.transform.position, transform.position) <= 6)
                {
                    Activate();
                }
            }
        }
        
        // 激活检查点的方法
        public void Activate()
        {
            // 如果检查点未激活
            if (!m_IsActivated)
            {
                // 将检查点状态设置为已激活
                m_IsActivated = true;

                // 切换检查点的基础物体的显示状态（隐藏第一个，显示第二个）
                m_Bases[0].SetActive(false);
                m_Bases[1].SetActive(true);

                // 通知游戏控制器，处理当前检查点
                GameController.m_Current.HandleCheckpoint(m_CheckpointNumber);

                // 创建激活检查点时的粒子效果
                GameObject obj = Instantiate(m_ActivateParticle);
                obj.transform.position = transform.position + new Vector3(0, .3f, 0);

                // 粒子效果显示3秒后销毁
                Destroy(obj, 3);
            }
        }
    }
}