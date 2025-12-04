using UnityEngine;
using System.Collections;


namespace Shooting.Gameplay
{
    public class EnemyShooterA : EnemyBase
    {
        // 子弹预制件
        public GameObject m_BulletPrefab1;
        // 发射时的粒子效果
        public GameObject m_FireParticlePrefab1;
        
        // 标记敌人是否已生成
        bool m_Spawned = false;

        void Start()
        {
            m_FacePlayer = false;
        }

        void Update()
        {
            // 检查敌人是否已经生成
            if (!m_Spawned)
            {
                // 如果敌人的位置在相机视野范围内，则标记为已生成，并开始进入场景的行为
                if (CameraController.m_Current.m_CameraTopPosition.z > transform.position.z && transform.position.z >= CameraController.m_Current.m_CameraBottomPosition.z)
                {
                    m_Spawned = true;
                    StartCoroutine(CoroutineEnterLevel());
                }
            }
            
            // 每帧检查是否面朝玩家
            HandleFacePlayer();
            // 每帧检查敌人死亡
            HandleDeath();
        }
        
        // 敌人进入场景的动作
        IEnumerator CoroutineEnterLevel()
        {
            // 获取敌人运动组件
            EnemyMove movement = GetComponent<EnemyMove>();
            // 设置敌人移动到指定位置
            movement.SetMoveTargetPosition(transform.position + m_StartWalkDistance * transform.forward);
            m_FacePlayer = false;

            // 等待敌人到达目标位置
            while (!movement.m_ReachedTargetPosition)
            {
                yield return null;
            }

            // 当敌人到达目标位置后，开始攻击循环
            StartCoroutine(CoroutineAttackLoop());
        }
        
        // 攻击循环协程
        IEnumerator CoroutineAttackLoop()
        {
            m_FacePlayer = true;  // 开始面朝玩家
            while (true)
            {
                // 每0.1秒检查一次
                yield return new WaitForSeconds(0.1f);

                // 如果敌人和玩家的距离小于等于30个单位，开始发射子弹
                if (Vector3.Distance(transform.position, PlayerCharacter.m_Current.transform.position) <= 30f)
                {
                    ShootBullet();
                }

                // 每次攻击间隔1.5秒
                yield return new WaitForSeconds(1.5f);
            }
        }
        
        public void ShootBullet()
        {
            // 实例化并发射子弹
            GameObject obj = Instantiate(m_BulletPrefab1);
            obj.transform.position = m_FirePoint.position;  // 设置子弹的发射位置
            Vector3 dir = PlayerCharacter.m_Current.transform.position - transform.position;  // 计算朝向玩家的方向
            dir.y = 0;  // 保证子弹的垂直方向不发生变化
            obj.transform.forward = dir;  // 设置子弹的朝向

            // 设置子弹的创建者
            obj.GetComponent<ProjectileCollisions>().m_Creator = gameObject;
            Destroy(obj, 10);  // 10秒后销毁子弹

            // 播放发射时的粒子效果
            obj = Instantiate(m_FireParticlePrefab1);
            obj.transform.position = m_FirePoint.position;  // 设置粒子效果的位置
            Destroy(obj, 3);  // 3秒后销毁粒子效果
        }
        
        // 用于编辑器中显示敌人移动的路径
        void OnDrawGizmos()
        {
            // 在场景视图中绘制敌人可能的行走路径
            Gizmos.DrawLine(transform.position, transform.position + m_StartWalkDistance * transform.forward);
        }
    }
}

