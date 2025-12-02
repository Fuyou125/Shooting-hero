using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Shooter.Gameplay
{
    public class Projectile_Homming_A : MonoBehaviour
    {
        // 是否启用追踪模式
        bool m_Chase = true;

        // Start 是在游戏对象激活时调用的函数
        void Start()
        {
            m_Chase = true;  // 启动追踪模式
            Invoke("StopChase", 4);  // 4秒后停止追踪
        }

        // Update 是每帧调用一次
        void Update()
        {
            // 如果追踪模式开启
            if (m_Chase)
            {
                // 计算投射物与玩家之间的方向（忽略Y轴，确保投射物只在水平面上进行追踪）
                Vector3 dir = PlayerChar.m_Current.transform.position - transform.position;
                dir.y = 0;  // 设置Y轴为0，避免竖直方向上的追踪

                // 设置投射物的前进方向为目标方向
                transform.forward = dir;
            }
        }

        // 停止追踪目标
        void StopChase()
        {
            m_Chase = false;  // 关闭追踪模式
        }
    }
}