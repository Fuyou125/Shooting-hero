using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shooter.Gameplay
{
    public class ProjectileMovement : MonoBehaviour
    {
        public float m_Speed = 50;             // 投射物的移动速度
        public float m_TurnSpeed = 0;          // 投射物的旋转速度

        // Start 是在游戏对象激活时调用的函数
        void Start()
        {
            // 启动投射物初始生长的协程
            StartCoroutine(CoroutineInitGrow());
        }

        // Update 是每帧调用一次
        void Update()
        {
            // 如果旋转速度不为零，则让投射物根据旋转速度旋转
            if (m_TurnSpeed != 0)
            {
                transform.forward = Quaternion.Euler(0, Time.deltaTime * m_TurnSpeed, 0) * transform.forward;
            }

            // 根据速度更新投射物的位置
            transform.position += m_Speed * Time.deltaTime * transform.forward;
        }

        // 协程：让投射物从小到大逐渐生长
        IEnumerator CoroutineInitGrow()
        {
            float lerp = 0;
            // 持续增长直到达到最大尺寸
            while(true)
            {
                transform.localScale = lerp * Vector3.one;  // 设置投射物的缩放比例
                lerp += 5 * Time.deltaTime;  // 每帧增加生长值
                yield return null;  // 等待下一帧

                // 如果生长值达到或超过1，停止增长
                if (lerp >= 1)
                    break;
            }

            // 设置投射物的最终大小为正常大小
            transform.localScale = Vector3.one;
        }
    }
}