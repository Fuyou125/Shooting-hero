using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shooter.Gameplay
{
    public class Projectile_Base : MonoBehaviour
    {
        // 命中时显示的粒子效果预设
        public GameObject HitParticlePrefab1;

        // 用于记录创建该投射物的物体
        [HideInInspector]
        public GameObject Creator;

        // 投射物的飞行速度
        public float Speed = 100;

        // 投射物的伤害值
        public float Damage = 1;

        // 投射物的碰撞半径
        public float m_Radius = 1f;

        // 投射物的最大飞行距离
        public float m_Range = 10;

        // 记录投射物的起始位置
        Vector3 m_StartPosition;

        // 初始化函数
        void Start()
        {
            // 记录投射物的起始位置
            m_StartPosition = transform.position;
        }

        // 每帧更新投射物的状态
        void Update()
        {
            // 如果投射物飞行距离超过最大范围，则销毁该投射物
            if (Vector3.Distance(m_StartPosition, transform.position) >= m_Range)
            {
                Destroy(gameObject);
                return;
            }

            // 使用球形射线检测与投射物路径上的所有物体的碰撞
            RaycastHit[] hits = Physics.SphereCastAll(transform.position, m_Radius, transform.forward, Speed * Time.deltaTime);
            
            // 遍历所有碰撞的物体
            foreach (RaycastHit hit in hits)
            {
                Collider col = hit.collider;

                // 如果碰撞到的是玩家
                if (col.gameObject.tag == "Player")
                {
                    // 确保投射物不是击中自己创建的对象
                    if (col.gameObject != Creator)
                    {
                        // 获取玩家的伤害控制组件并应用伤害
                        DamageControl d = col.gameObject.GetComponent<DamageControl>();
                        if (d != null)
                        {
                            d.ApplyDamage(Damage, transform.forward, 1);
                        }

                        // 获取玩家角色并处理投射物销毁
                        PlayerChar p = col.gameObject.GetComponent<PlayerChar>();
                        Destroyed(hit.point);
                    }
                }
                // 如果碰撞到的是障碍物
                else if (col.gameObject.tag == "Block")
                {
                    // 应用伤害并销毁投射物
                    DamageControl d = col.gameObject.GetComponent<DamageControl>();
                    if (d != null)
                    {
                        d.ApplyDamage(Damage, transform.forward, 1);
                    }
                    Destroyed(hit.point);
                }
                // 如果碰撞到的是敌人
                else if (col.gameObject.tag == "Enemy")
                {
                    // 应用伤害并销毁投射物
                    DamageControl d = col.gameObject.GetComponent<DamageControl>();
                    if (d != null)
                    {
                        d.ApplyDamage(Damage, transform.forward, 1);
                    }
                    Destroyed(hit.point);
                }
            }

            // 更新投射物的位置，使其按速度沿着前方方向飞行
            transform.position += Speed * Time.deltaTime * transform.forward;
        }

        // 当投射物命中目标时触发
        public virtual void Destroyed(Vector3 pos)
        {
            // 销毁投射物
            Destroy(gameObject);

            // 创建并显示命中粒子效果
            GameObject obj = Instantiate(HitParticlePrefab1);
            obj.transform.position = pos;
            //obj.transform.localScale = 0.4f * Vector3.one; // 可选：调整粒子效果的缩放
            Destroy(obj, 6); // 6秒后销毁粒子效果
        }
    }
}
