using System;
using UnityEngine;

namespace Shooting.Gameplay
{
    public class EnemyMove : MonoBehaviour
    {
        // 当前的移动方向
        [HideInInspector]
        public Vector3 m_MovementDirection;
        
        // 是否到达目标位置
        [HideInInspector]
        public bool m_ReachedTargetPosition = false;
        
        // 当前的目标位置
        [HideInInspector]
        public Vector3 m_MovementTargetPosition;
        
        // 是否启用移动
        [HideInInspector]
        public bool m_MovementEnable = true;

        // 移动速度
        public float m_MovementSpeed = 20;

        
        // 是否朝着移动方向转向
        [HideInInspector]
        public bool m_FaceMoveDirection = true;
        
        // 动画控制器
        public Animator m_Animator;

        private void Update()
        {
            // 如果目标位置没有达到
            if (!m_ReachedTargetPosition)
            {
                if (m_MovementTargetPosition != Vector3.zero) // 如果目标位置有效
                {
                    // 计算从当前位置到目标位置的方向
                    m_MovementDirection = m_MovementTargetPosition - transform.position;
                    m_MovementDirection.y = 0; // 保证在水平面上移动，不考虑垂直方向

                    // 如果移动距离小于设定的阈值，则认为到达目标位置
                    if (m_MovementDirection.magnitude <= .5f)
                    {
                        m_MovementTargetPosition = Vector3.zero; // 清空目标位置
                        m_MovementDirection = Vector3.zero; // 清空方向
                        m_ReachedTargetPosition = true; // 标记为已到达目标位置
                    }

                    // 归一化方向向量，确保移动速度一致
                    m_MovementDirection.Normalize();

                    // 如果需要朝着移动方向旋转
                    if (m_FaceMoveDirection && m_MovementDirection != Vector3.zero)
                    {
                        // 平滑地旋转敌人，使其面朝移动方向
                        transform.rotation = Quaternion.Lerp(transform.rotation,
                            Quaternion.LookRotation(m_MovementDirection), 10 * Time.deltaTime);
                    }
                }
            }
            
            // 不使用物理引擎时直接通过更新位置来移动
            if (m_MovementDirection != Vector3.zero)
            {
                // 每帧更新敌人位置
                transform.position += Time.deltaTime * m_MovementSpeed * m_MovementDirection;

                // 设置动画的“行走速度”参数
                if (m_Animator != null)
                    m_Animator.SetFloat("WalkSpeed", 1);
            }
            else
            {
                // 如果没有移动，设置动画参数为停止状态
                if (m_Animator != null)
                    m_Animator.SetFloat("WalkSpeed", 0);
            }
        }


        // 设置新的目标位置
        public void SetMoveTargetPosition(Vector3 position)
        {
            m_ReachedTargetPosition = false;  // 还没到达目标位置
            m_MovementTargetPosition = position;  // 设置新的目标位置
        }
    }
}