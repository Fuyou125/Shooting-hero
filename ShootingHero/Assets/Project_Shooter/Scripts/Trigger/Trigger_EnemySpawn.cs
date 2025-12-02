using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shooter.Gameplay
{
    // 触发敌人生成的脚本
    public class Trigger_EnemySpawn : MonoBehaviour
    {
        public EnemySpawnPoint[] m_SpawnPoints; // 存储敌人生成点的数组
        
        // 当触发器与其他物体碰撞时调用
        void OnTriggerEnter(Collider coll)
        {
            // 如果碰撞的物体是玩家
            if (coll.gameObject.tag == "Player")
            {
                // 遍历所有敌人生成点，并生成敌人
                foreach (EnemySpawnPoint sp in m_SpawnPoints)
                {
                    sp.SpawnEnemy();  // 调用每个生成点的 SpawnEnemy 方法生成敌人
                }

                // 禁用当前的碰撞器，防止重复触发
                GetComponent<Collider>().enabled = false;
            }
        }
    }
}