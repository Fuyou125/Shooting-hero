using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shooter.Gameplay
{
    public class Boss_A : Enemy
    {
        // 是否可以发射子弹
        bool CanFire = true;

        // 发射点位置
        public Transform ShotPoint;
        // 子弹预制件
        public GameObject m_BulletPrefab1;

        // 多个发射点
        public Transform[] m_FirePoints;
        // 死亡时的粒子效果发射点
        public Transform[] m_DeathParticlePoints;
        // 死亡粒子预制件
        [SerializeField]
        protected GameObject m_DeathParticlePrefab2;

        // 攻击等级
        [HideInInspector]
        public int m_AttackLevel = 0;

        // 初始化：设置初始位置、无伤害状态
        void Start()
        {
            m_DamageControl = GetComponent<DamageControl>();  // 获取 DamageControl 组件
            InitPosition = transform.position;  // 初始位置
            m_DamageControl.m_NoDamage = true;  // 设置为无伤害状态
            transform.position = InitPosition + new Vector3(0, 0, 20);  // 将 Boss 放置在屏幕外面

            m_AttackLevel = 0;  // 初始攻击等级
        }

        // 每帧更新
        void Update()
        {
            float damage = m_DamageControl.Damage / m_DamageControl.MaxDamage;
            // 根据当前损伤百分比设置攻击等级
            if (damage > .6f)
            {
                m_AttackLevel = 0;
            }
            else if (damage > .3f)
            {
                m_AttackLevel = 1;
            }
            else
            {
                m_AttackLevel = 2;
            }

            HandleFacePlayer();  // 处理面向玩家
            HandleDeath();  // 处理死亡
        }

        // 攻击循环协程
        IEnumerator CoroutineAttackLoop()
        {
            while (true)
            {
                // 根据攻击等级选择不同的攻击模式
                switch (m_AttackLevel)
                {
                    case 0:
                        yield return new WaitForSeconds(1f);
                        ShootRingBullet(2);  // 发射环形子弹
                        yield return new WaitForSeconds(2f);
                        for (int i = 0; i < 5; i++)
                        {
                            ShootBullet();  // 发射普通子弹
                            yield return new WaitForSeconds(.3f);
                        }
                        yield return new WaitForSeconds(2f);
                        break;

                    case 1:
                        yield return new WaitForSeconds(1f);
                        ShootRingBullet(3);  // 发射环形子弹
                        yield return new WaitForSeconds(.7f);
                        ShootRingBullet(3);
                        yield return new WaitForSeconds(2f);
                        for (int i = 0; i < 10; i++)
                        {
                            ShootBullet();  // 发射普通子弹
                            yield return new WaitForSeconds(.2f);
                        }
                        yield return new WaitForSeconds(1f);
                        break;

                    case 2:
                        yield return new WaitForSeconds(1f);
                        ShootRingBullet(4);  // 发射环形子弹
                        yield return new WaitForSeconds(.6f);
                        ShootRingBullet(4);
                        yield return new WaitForSeconds(.6f);
                        ShootRingBullet(4);
                        yield return new WaitForSeconds(2f);
                        for (int i = 0; i < 15; i++)
                        {
                            ShootBullet();  // 发射普通子弹
                            yield return new WaitForSeconds(.3f);
                        }
                        yield return new WaitForSeconds(1f);
                        break;
                }
            }
        }

        // 移动循环协程
        IEnumerator CoroutineMoveLoop()
        {
            EnemyMovement movement = GetComponent<EnemyMovement>();  // 获取敌人移动组件

            movement.m_FaceMoveDirection = false;
            m_FacePlayer = true;

            // 定义四个移动点
            Vector3[] points = new Vector3[4];
            points[0] = InitPosition + new Vector3(-3, 0, 0);
            points[1] = InitPosition + new Vector3(3, 0, 0);
            points[2] = InitPosition + new Vector3(-3, 0, 2);
            points[3] = InitPosition + new Vector3(3, 0, 2);
            int pointNum = 0;

            while (true)
            {
                movement.SetMoveTargetPosition(points[pointNum]);
                while (!movement.m_ReachedTargetPosition)
                {
                    yield return null;
                }

                pointNum = Random.Range(0, 4);  // 随机选择下一个目标点

                yield return new WaitForSeconds(1);  // 每次移动后等待1秒
            }
        }

        // 启用敌人：启动协程
        public override void EnableEnemy()
        {
            base.EnableEnemy();
            StartCoroutine(CoroutineEnableEnemy());
        }

        // 启用敌人协程
        IEnumerator CoroutineEnableEnemy()
        {
            float lerp = 0;
            while (lerp <= 1)
            {
                lerp += 0.2f * Time.deltaTime;
                transform.position = Vector3.Lerp(InitPosition + new Vector3(0, 0, 20), InitPosition , lerp);  // 从初始位置平滑移动到目标位置
                yield return null;
            }

            transform.position = InitPosition;

            yield return new WaitForSeconds(1f);
            m_DamageControl.m_NoDamage = false;  // 恢复伤害状态
            m_FacePlayer = true;
            StartCoroutine(CoroutineMoveLoop());  // 开始移动
            yield return new WaitForSeconds(1f);
            StartCoroutine(CoroutineAttackLoop());  // 开始攻击循环
        }

        // 发射普通子弹
        public void ShootBullet()
        {
            Vector3 dir;
            GameObject obj;

            obj = Instantiate(m_BulletPrefab1);
            obj.transform.position = m_FirePoints[1].position;
            dir = PlayerChar.m_Current.transform.position - m_FirePoints[1].position;
            dir.y = 0;
            obj.transform.forward = Quaternion.Euler(0, -00, 0) * dir;  // 朝向玩家
            obj.GetComponent<ProjectileCollision>().m_Creator = gameObject;
            obj.GetComponent<ProjectileMovement>().m_TurnSpeed = 00;
            Destroy(obj, 10);  // 10秒后销毁子弹

            obj = Instantiate(m_BulletPrefab1);
            obj.transform.position = m_FirePoints[2].position;
            dir = PlayerChar.m_Current.transform.position - m_FirePoints[2].position;
            dir.y = 0;
            obj.transform.forward = Quaternion.Euler(0, 00, 0) * dir;  // 朝向玩家
            obj.GetComponent<ProjectileCollision>().m_Creator = gameObject;
            obj.GetComponent<ProjectileMovement>().m_TurnSpeed = -00;
            Destroy(obj, 10);  // 10秒后销毁子弹
        }

        // 发射环形子弹
        public void ShootRingBullet(int halfCount)
        {
            for (int i = -halfCount; i <= halfCount; i++)
            {
                GameObject obj = Instantiate(m_BulletPrefab1);
                obj.transform.position = m_FirePoints[0].position;
                obj.transform.forward = Quaternion.Euler(0, i * 20, 0) * m_RotationBase.forward;  // 环形发射
                obj.GetComponent<ProjectileCollision>().m_Creator = gameObject;
                Destroy(obj, 10);  // 10秒后销毁子弹
            }
        }

        // 处理死亡
        public override void HandleDeath()
        {
            if (!m_IsDead)
            {
                if (m_DamageControl.Damage <= 0)
                {
                    StopAllCoroutines();  // 停止所有协程
                    StartCoroutine(CoroutineHandleDeath());  // 启动死亡协程
                    m_IsDead = true;  // 标记为死亡
                }
            }
        }

        // 死亡协程
        IEnumerator CoroutineHandleDeath()
        {
            float delay = .2f;
            for (int i = 0; i < 10; i++)
            {
                GameObject obj = Instantiate(m_DeathParticlePrefab);
                obj.transform.position = m_DeathParticlePoints[i % 5].position;
                Destroy(obj, 10);  // 10秒后销毁死亡粒子
                yield return new WaitForSeconds(delay);
                delay -= .01f;  // 每次减少延迟时间
            }

            yield return new WaitForSeconds(.5f);

            GameObject obj1 = Instantiate(m_DeathParticlePrefab2);
            obj1.transform.position = transform.position;  // 生成第二个死亡粒子
            Destroy(obj1, 6);
            CameraControl.m_Current.StartShake(.6f, .2f);  // 相机震动
            DropItem(20);  // 掉落物品

            Destroy(gameObject);  // 销毁 Boss
        }

        // 掉落物品
        public override void DropItem(int count)
        {
            // 掉落物品
            for (int i = 0; i < 20; i++)
            {
                GameObject obj1 = Instantiate(m_ItemPrefabs[0]);
                obj1.transform.position = transform.position;
                Vector3 v = Helper.RotatedLenght(i * 18, 10) + new Vector3(0, 15, 0);  // 随机位置
                obj1.GetComponent<Rigidbody>().velocity = v;  // 设置物理速度
            }

            for (int i = 0; i < 20; i++)
            {
                GameObject obj1 = Instantiate(m_ItemPrefabs[0]);
                obj1.transform.position = transform.position;
                Vector3 v = Helper.RotatedLenght(i * 18, 15) + new Vector3(0, 20, 0);  // 随机位置
                obj1.GetComponent<Rigidbody>().velocity = v;  // 设置物理速度
            }
        }
    }
}
