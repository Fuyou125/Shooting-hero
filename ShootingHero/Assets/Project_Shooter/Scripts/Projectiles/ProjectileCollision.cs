using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Shooter.Gameplay
{
    public class ProjectileCollision : MonoBehaviour
    {
        public GameObject m_Creator;            // 投射物的创建者（通常是发射者）
        public GameObject m_HitParticle;        // 碰撞时的粒子特效
        public float m_Damage = 1;              // 投射物造成的伤害
        public bool m_IsEnemyTeam = true;       // 判断投射物是否来自敌方队伍

        // Update 是每帧调用一次
        void Update()
        {
            // 获取当前投射物周围半径为 0.5 的所有碰撞体
            Collider[] hits = Physics.OverlapSphere(transform.position, .5f);

            // 遍历所有碰撞体
            foreach (Collider col in hits)
            {
                // 如果碰撞体是投射物的创建者，跳过当前循环
                if (col.gameObject == m_Creator)
                    continue;

                // 如果碰撞体是玩家且投射物来自敌方队伍
                if (col.gameObject.tag == "Player" && m_IsEnemyTeam)
                {
                    // 获取目标物体的伤害控制组件，并应用伤害
                    DamageControl d = col.gameObject.GetComponent<DamageControl>();
                    if (d != null)
                    {
                        d.ApplyDamage(m_Damage, transform.forward, 1);
                    }
                    // 创建粒子效果
                    CreateHitParticle();
                    // 销毁当前投射物
                    Destroy(gameObject);
                }
                // 如果碰撞体是方块
                else if (col.gameObject.tag == "Block")
                {
                    // 获取方块的伤害控制组件，并应用伤害
                    DamageControl d = col.gameObject.GetComponent<DamageControl>();
                    if (d != null)
                    {
                        d.ApplyDamage(m_Damage, transform.forward, 1);
                    }
                    // 创建粒子效果
                    CreateHitParticle();
                    // 销毁当前投射物
                    Destroy(gameObject);
                }
                // 如果碰撞体是敌人且投射物来自己方队伍
                else if (col.gameObject.tag == "Enemy" && !m_IsEnemyTeam)
                {
                    // 获取目标物体的伤害控制组件，并应用伤害
                    DamageControl d = col.gameObject.GetComponent<DamageControl>();
                    if (d != null)
                    {
                        d.ApplyDamage(m_Damage, transform.forward, 1);
                    }
                    // 创建粒子效果
                    CreateHitParticle();
                    // 销毁当前投射物
                    Destroy(gameObject);
                }

            }
        }

        // 创建碰撞时的粒子特效
        public void CreateHitParticle()
        {
            // 实例化粒子效果
            GameObject obj = Instantiate(m_HitParticle);
            obj.transform.position = transform.position;  // 设置粒子效果的位置为当前投射物的位置
            Destroy(obj, 3);  // 3秒后销毁粒子效果
        }
    }
}
