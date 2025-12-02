using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shooter.Gameplay
{
    public class Enemy_Canon_A : Enemy
    {
        // 子弹预制件
        public GameObject m_BulletPrefab1;
        // 开火时的粒子效果
        public GameObject m_FireParticlePrefab1;
        // 开火前的粒子效果
        public GameObject m_PreFireParticlePrefab1;

        // Start is called before the first frame update
        void Start()
        {
            // 启动攻击循环
            StartCoroutine(CoroutineAttackLoop());
        }

        // Update is called once per frame
        void Update()
        {
            // 如果敌人和玩家的距离小于等于30，则开始面朝玩家
            if (Vector3.Distance(transform.position, PlayerChar.m_Current.transform.position) <= 30f)
            {
                m_FacePlayer = true;
            }
            else
            {
                m_FacePlayer = false;
            }
            
            // 处理面朝玩家的行为
            HandleFacePlayer();
            // 处理敌人死亡
            HandleDeath();
        }

        // 攻击循环协程
        IEnumerator CoroutineAttackLoop()
        {
            while (true)
            {
                // 每0.5秒检查一次
                yield return new WaitForSeconds(0.5f);
                
                // 如果敌人和玩家的距离小于等于30
                if (Vector3.Distance(transform.position, PlayerChar.m_Current.transform.position) <= 30f)
                {
                    // 播放开火前的粒子效果
                    GameObject obj = Instantiate(m_PreFireParticlePrefab1);
                    obj.transform.SetParent(m_FirePoint);  // 将粒子效果设置为火力点的子物体
                    obj.transform.localPosition = Vector3.zero;  // 重置粒子的位置
                    Destroy(obj, 3);  // 3秒后销毁粒子效果

                    // 等待1.3秒后发射子弹
                    yield return new WaitForSeconds(1.3f);
                    ShootBullet();  // 发射子弹
                }

                // 等待1.5秒后继续攻击
                yield return new WaitForSeconds(1.5f);
            }
        }

        // 发射子弹
        public void ShootBullet()
        {
            // 实例化并发射子弹
            GameObject obj = Instantiate(m_BulletPrefab1);
            obj.transform.position = m_FirePoint.position;  // 设置子弹的发射位置
            Vector3 dir = PlayerChar.m_Current.transform.position - transform.position;  // 计算朝向玩家的方向
            dir.y = 0;  // 保证子弹的垂直方向不发生变化
            obj.transform.forward = dir;  // 设置子弹的朝向

            // 设置子弹的创建者
            obj.GetComponent<ProjectileCollision>().m_Creator = gameObject;
            Destroy(obj, 10);  // 10秒后销毁子弹

            // 播放火焰粒子效果
            obj = Instantiate(m_FireParticlePrefab1);
            obj.transform.position = m_FirePoint.position;  // 设置粒子效果的位置
            Destroy(obj, 3);  // 3秒后销毁粒子效果
        }
    }
}
