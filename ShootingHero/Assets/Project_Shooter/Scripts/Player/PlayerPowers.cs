using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shooter.Gameplay
{
    public class PlayerPowers : MonoBehaviour
    {
        // 玩家是否拥有当前的特殊能力
        [HideInInspector]
        public bool m_HavePower = false;

        // 当前特殊能力的编号
        [HideInInspector]
        public int m_PowerNum = 0;

        // 是否有计时器，表示该能力是否有时间限制
        [HideInInspector]
        public bool m_HasTimer = false;

        // 计时器的时间（秒）
        [HideInInspector]
        public float m_Timer = 0;

        // 当前特殊能力的弹药数
        [HideInInspector]
        public int m_AmmoCount = 0;

        // 在脚本启动时，初始化玩家的能力状态
        void Start()
        {
            m_PowerNum = -1;  // 初始化能力编号为-1，表示没有能力
            m_HavePower = false;  // 初始化玩家没有能力
        }

        // 每帧更新玩家能力状态
        void Update()
        {
            // 如果玩家有能力
            if (m_HavePower)
            {
                // 如果能力有计时器
                if (m_HasTimer)
                {
                    m_Timer -= Time.deltaTime;  // 每帧减少计时器的时间
                    if (m_Timer <= 0)  // 如果计时器结束
                    {
                        m_HavePower = false;  // 玩家失去能力
                        m_PowerNum = -1;  // 清空能力编号
                    }
                }
                else
                {
                    // 如果能力没有计时器，则依赖弹药数量
                    if (m_AmmoCount <= 0)  // 如果弹药数为零
                    {
                        m_HavePower = false;  // 玩家失去能力
                        m_PowerNum = -1;  // 清空能力编号
                    }
                }
            }
        }

        // 设置新的能力
        public void SetNewPower(int num)
        {
            m_HavePower = true;  // 玩家获得新能力
            switch(num)  // 根据能力编号执行不同的逻辑
            {
                case 0:
                    m_HasTimer = false;  // 该能力没有计时器
                    m_AmmoCount = 3;  // 设置该能力的弹药数量为3
                    m_PowerNum = 0;  // 设置能力编号为0
                    break;
                // 其他能力可以在此添加
            }
        }
    }
}
