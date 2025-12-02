using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shooter.Gameplay
{
    // 爆炸效果类，用于处理爆炸范围内的伤害和力的作用
    public class Explosion : MonoBehaviour
    {
        // 爆炸的范围半径
        public float Radius = 5;

        // 在对象初始化时调用
        void Start()
        {
            // 获取爆炸范围内的所有碰撞体
            Collider[] colls = Physics.OverlapSphere(transform.position, Radius);
            foreach (Collider col in colls)
            {
                // 如果碰撞体是玩家
                if (col.gameObject.tag == "Player")
                {
                    // 计算玩家与爆炸中心的距离百分比（归一化）
                    float lerp = Vector3.Distance(col.bounds.center, transform.position) / (float)Radius;
                    
                    // 获取玩家的 DamageControl 组件，应用伤害
                    PlayerChar p = col.gameObject.GetComponent<PlayerChar>();
                    p.m_DamageControl.ApplyDamage(Mathf.Lerp(40, 5, lerp), p.transform.position + new Vector3(0, 2, 0) - transform.position, 4);
                }
                // 如果碰撞体是方块（或任何其他可摧毁的物体）
                else if (col.gameObject.tag == "Block")
                {
                    // 获取方块的刚体组件，应用力的影响
                    Rigidbody rb = col.gameObject.GetComponent<Rigidbody>();
                    if (rb != null)
                    {
                        // 对方块施加力，使其远离爆炸中心
                        rb.AddForceAtPosition(col.gameObject.transform.position - transform.forward, transform.position);
                    }

                    // 获取方块的 DamageControl 组件，应用伤害
                    DamageControl d = col.gameObject.GetComponent<DamageControl>();
                    if (d != null)
                    {
                        // 计算与爆炸中心的距离百分比（归一化），并根据距离应用不同的伤害
                        float lerp = Vector3.Distance(col.bounds.center, transform.position) / Radius;
                        d.ApplyDamage(Mathf.Lerp(10, 1, lerp), transform.forward, 1);
                    }
                }
            }
        }
    }
}
