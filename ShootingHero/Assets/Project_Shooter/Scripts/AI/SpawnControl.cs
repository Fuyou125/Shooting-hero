using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shooter.Gameplay
{
    // 敌人生成控制类
    public class SpawnControl : MonoBehaviour
    {
        // 用于控制是否延迟生成敌人
        private bool m_DelayingSpawn = true;

        // 敌人预制体数组，用于存储可以生成的敌人类型
        public GameObject[] m_EnemyPrefabs;

        // 生成点的数组
        [HideInInspector]
        public GameObject[] m_SpawnPoints;

        // 当前场景中所有敌人
        [HideInInspector]
        public List<GameObject> m_Enemies;

        // 生成计数器
        [HideInInspector]
        public int SpawnCounter = 0;

        // 总共生成的敌人数量
        [HideInInspector]
        public int TotalSpawnCount = 0;

        // 当前敌人数量
        [HideInInspector]
        public int CurrentEnemyCount = 0;

        // 是否继续生成敌人
        [HideInInspector]
        public bool KeepSpawnning = true;

        // 总杀敌数量
        [HideInInspector]
        public int TotalKillCount = 0;

        // 当前 SpawnControl 实例（单例模式）
        public static SpawnControl Current;

        // 当前生成点的索引
        int m_SpawnPointNumber = 0;

        // 初始化 SpawnControl 实例
        void Awake()
        {
            Current = this;
            m_Enemies = new List<GameObject>();

            // 查找所有标签为 "SpawnPoint" 的生成点，并存储它们
            GameObject[] objs = GameObject.FindGameObjectsWithTag("SpawnPoint");
            m_SpawnPoints = objs;
        }

        // Start 是游戏开始时调用的
        void Start()
        {
            m_DelayingSpawn = false;
            SpawnCounter = 0;
            CurrentEnemyCount = 0;
            KeepSpawnning = true;
            TotalSpawnCount = 20; // 设置总共需要生成的敌人数量
        }

        // Update 是每帧调用的，用来控制敌人的生成
        void Update()
        {
            if (KeepSpawnning)
            {
                // 如果当前敌人数量小于 100，继续生成敌人
                if (CurrentEnemyCount < 100)
                {
                    // 如果没有延迟生成，开始生成敌人
                    if (!m_DelayingSpawn)
                    {
                        // 获取当前生成点的位置
                        Transform CurrentSpawnPoint = m_SpawnPoints[m_SpawnPointNumber].transform;

                        // 如果生成点离玩家的距离小于等于 30，则切换到下一个生成点
                        if (Vector3.Distance(CurrentSpawnPoint.position, PlayerChar.m_Current.transform.position) <= 30)
                        {
                            m_SpawnPointNumber++;
                        }
                        else
                        {
                            int enemyNumber = 0;  // 当前生成的敌人类型（默认是第一个类型）

                            // 实例化敌人
                            GameObject obj = Instantiate(m_EnemyPrefabs[enemyNumber]);
                            Vector3 pos = m_SpawnPoints[m_SpawnPointNumber].transform.position;
                            pos.y = 1; // 设置生成位置的 Y 坐标为 1
                            obj.transform.position = pos;

                            // 增加当前敌人数量
                            AddEnemy();

                            // 延迟生成
                            m_DelayingSpawn = true;

                            // 如果已生成敌人的数量达到了总生成数量，停止生成敌人
                            if (SpawnCounter >= TotalSpawnCount)
                            {
                                KeepSpawnning = false;
                                // 触发事件（例如，关卡完成、生成结束等）
                            }
                            else
                            {
                                // 延时 0.05 秒后允许生成下一个敌人
                                Invoke("EnableCanSpawnEnemy", .05f);
                            }

                            // 切换到下一个生成点
                            m_SpawnPointNumber++;
                        }
                        
                        // 如果生成点数组已经用完，重新从第一个生成点开始
                        if (m_SpawnPointNumber > m_SpawnPoints.Length - 1)
                        {
                            m_SpawnPointNumber = 0;
                        }
                    }
                }
            }
        }

        // 允许生成敌人（解除延迟）
        private void EnableCanSpawnEnemy()
        {
            m_DelayingSpawn = false;
        }

        // 增加敌人数量
        public void AddEnemy()
        {
            CurrentEnemyCount++;
        }
    }
}
