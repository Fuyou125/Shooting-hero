using UnityEngine;

namespace Shooting.Gameplay
{
    // 敌人生成点类
    public class EnemySpawn : MonoBehaviour
    {
        // 敌人的预制体
        public GameObject m_EnemyPrefab;
        
        // 该生成点生成的敌人数量
        public int m_SpawnCount;
        
        // 记录该生成点是否已经生成敌人
        [HideInInspector]
        public bool m_Spawned = false;
        
        // 生成点的旋转角度，用于偏移生成位置
        public float m_PositionsAngle = 0;

        // 存储生成点的偏移位置
        Vector3[] m_Points;

        // 敌人开始行走的距离
        public float m_StartWalkDistance = 10;

        // 是否自动生成敌人
        public bool m_AutoSpawn = true;

        void Start()
        {
            // 初始化生成状态为未生成
            m_Spawned = false;
        }

        void Update()
        {
            // 如果敌人尚未生成，并且允许自动生成开启
            if (!m_Spawned && m_AutoSpawn)
            {
                // 判断敌人生成点是否在相机视野范围内，如果在，则生成敌人
                if (CameraController.m_Current.m_CameraTopPosition.z > transform.position.z && transform.position.z >= CameraController.m_Current.m_CameraBottomPosition.z)
                {
                    SpawnEnemy();  // 生成敌人
                    m_Spawned = true;  // 标记为已生成
                }
            }
        }
        
        // 生成敌人
        public void SpawnEnemy()
        {
            // 根据生成数量生成敌人
            for (int i = 0; i < m_SpawnCount; i++)
            {
                // 创建一个新的敌人对象
                GameObject obj = Instantiate(m_EnemyPrefab);

                // 设置敌人生成的位置（基于旋转的偏移位置）
                obj.transform.position = transform.position + transform.rotation * m_Points[i];

                // 设置敌人朝向（朝着生成点的前方）
                obj.transform.forward = transform.forward;

                // 获取敌人脚本并设置开始行走的距离
                EnemyBase e = obj.GetComponent<EnemyBase>();
                e.m_StartWalkDistance = m_StartWalkDistance;
            }
        }
        
        // 在编辑器中对生成点位置进行预计算和更新
        void OnValidate()
        {
            // 如果生成数量大于0
            if (m_SpawnCount > 0)
            {
                m_Points = new Vector3[m_SpawnCount];

                // 如果生成一个敌人
                if (m_SpawnCount == 1)
                {
                    m_Points[0] = Vector3.zero;  // 生成在当前位置
                }
                else
                {
                    // 计算敌人生成的角度间隔和生成半径
                    float dAngle = 360f / (float)m_SpawnCount;
                    float lenght = Mathf.Clamp(2f + .3f * (m_SpawnCount - 2), .6f, 7);  // 控制半径大小

                    // 根据间隔角度和半径计算每个敌人的位置
                    for (int i = 0; i < m_SpawnCount; i++)
                    {
                        m_Points[i] = Helpers.RotatedLength(i * dAngle + m_PositionsAngle, lenght);
                    }
                }
            }
        }

        // 在场景视图中绘制敌人生成点的调试信息
        void OnDrawGizmos()
        {
            // 如果有生成敌人数量
            if (m_SpawnCount > 0)
            {
                // 绘制生成点的前方线条
                Gizmos.DrawLine(transform.position, transform.position + 2 * transform.forward);

                // 绘制每个敌人的生成位置（红色球体）
                for (int i = 0; i < m_SpawnCount; i++)
                {
                    Gizmos.color = Color.red;
                    // 绘制敌人生成点的位置
                    Gizmos.DrawWireSphere(transform.position + transform.rotation * m_Points[i], .5f);
                    // 绘制敌人移动的路径（从生成点开始的前进线）
                    Gizmos.DrawLine(transform.position + transform.rotation * m_Points[i], transform.position + transform.rotation * m_Points[i] + m_StartWalkDistance * transform.forward);
                }
            }
        }
    }
}