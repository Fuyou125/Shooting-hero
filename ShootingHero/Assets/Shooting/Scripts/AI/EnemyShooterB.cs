using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace Shooting.Gameplay
{
    public class EnemyShooterB : EnemyBase
    {
        // 子弹预制件
        public GameObject m_BulletPrefab1;
        // 发射时的粒子效果预制件
        public GameObject m_FireParticlePrefab1;

        // 移动的距离
        public float m_WalkDistance = 3;
        
        // Start is called before the first frame update
        void Start()
        {
            // 记录敌人初始位置
            InitPosition = transform.position;
        }

        // Update is called once per frame
        void Update()
        {
            // 每帧检查敌人是否被激活（是否进入视野）
            CheckAlert();
            
            // 每帧检查是否面朝玩家
            HandleFacePlayer();

            // 每帧检查敌人死亡状态
            HandleDeath();
        }
        
        // 重写开始警觉的方法
        public override void StartAlert()
        {
            // 调用父类的警觉方法
            base.StartAlert();
            // 启动进入场景的协程
            StartCoroutine(CoroutineEnterLevel());
        }
        
        // 敌人进入场景的行为
        IEnumerator CoroutineEnterLevel()
        {
            // 获取敌人运动组件
            EnemyMove movement = GetComponent<EnemyMove>();
            
            // 设置敌人移动目标位置
            movement.SetMoveTargetPosition(transform.position + m_StartWalkDistance * transform.forward);
            m_FacePlayer = false; // 开始时不面朝玩家

            // 等待敌人到达目标位置
            while (!movement.m_ReachedTargetPosition)
            {
                yield return null;
            }

            // 移动完成后稍等1秒，然后开始攻击循环
            yield return new WaitForSeconds(1f);
            StartCoroutine(CoroutineAttackLoop());
        }
        
        // 攻击循环协程
        IEnumerator CoroutineAttackLoop()
        {
            // 获取敌人运动组件和状态系统
            EnemyMove movement = GetComponent<EnemyMove>();

            // 设置敌人的攻击路径
            Vector3[] points = new Vector3[2];
            points[0] = transform.position + new Vector3(-m_WalkDistance, 0, 0); // 左侧目标点
            points[1] = transform.position + new Vector3(m_WalkDistance, 0, 0);  // 右侧目标点
            int pointNum = 0; // 初始时移动到左侧目标点

            while (true)
            {
                // 开始朝目标方向移动
                movement.m_FaceMoveDirection = true;
                m_FacePlayer = false;  // 暂时不面朝玩家
                movement.SetMoveTargetPosition(points[pointNum]);

                // 等待敌人到达目标位置
                while (!movement.m_ReachedTargetPosition)
                {
                    yield return null;
                }

                // 到达目标点后切换目标点
                pointNum = (pointNum == 0) ? 1 : 0;

                // 反转敌人面朝方向
                movement.m_FaceMoveDirection = false;
                m_FacePlayer = true;

                // 等待一段时间
                yield return new WaitForSeconds(1f);

                // 播放开火动画并执行射击
                m_Animator.SetTrigger("Shoot");
                ShootBullet();

                // 等待一段时间后继续循环
                yield return new WaitForSeconds(1f);
            }
        }
        
        // 射击子弹
        public void ShootBullet()
        {
            // 发射两颗子弹，并分别调整它们的发射角度
            for (int i = 0; i < 2; i++)
            {
                // 实例化子弹
                GameObject obj = Instantiate(m_BulletPrefab1);
                obj.transform.position = m_FirePoint.position;  // 设置子弹发射位置
                Vector3 dir = PlayerCharacter.m_Current.transform.position - transform.position;  // 计算朝向玩家的方向
                dir.y = 0;  // 保证子弹的垂直方向不发生变化
                obj.transform.forward = Quaternion.Euler(0, -5f + i * 10, 0) * dir;  // 设置子弹的朝向（略微调整角度）

                // 设置子弹的创建者
                obj.GetComponent<ProjectileCollisions>().m_Creator = gameObject;
                // 销毁子弹（10秒后）
                Destroy(obj, 10);

                // 播放发射时的粒子效果
                obj = Instantiate(m_FireParticlePrefab1);
                obj.transform.position = m_FirePoint.position;  // 设置粒子效果位置
                Destroy(obj, 3);  // 3秒后销毁粒子效果
            }
        }
        
        // 在场景视图中绘制敌人的移动路径
        void OnDrawGizmos()
        {
            // 绘制敌人可能的移动路径
            Gizmos.DrawLine(transform.position, transform.position + m_StartWalkDistance * transform.forward);
            Gizmos.DrawLine(transform.position + m_WalkDistance * transform.right, transform.position - m_WalkDistance * transform.right);
        }
    }
}
