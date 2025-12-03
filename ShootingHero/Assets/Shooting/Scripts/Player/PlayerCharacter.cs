using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shooting
{
    public class PlayerCharacter : MonoBehaviour
    {
        [HideInInspector] public static PlayerCharacter m_Current; // 当前玩家实例

        [SerializeField] private Transform m_TurnBase; // 转动基础物体
        [SerializeField] private Transform m_AimBase; // 瞄准基础物体

        //-------------------------------
        [HideInInspector] public bool m_InControl = false; // 是否控制玩家

        public Transform[] m_WeaponHands; // 玩家武器手的 Transform
        public Transform m_FirePoint; // 开火点
        public GameObject m_WeaponPowerParticle; // 武器能量粒子效果

        Vector3 m_MovementInput; // 玩家移动输入
        Vector3 m_DashDirection; // 冲刺方向

        bool m_Input_Fire; // 开火输入
        bool m_Input_Fire2; // 另一个开火输入（暂时没有用）
        bool m_Input_LockAim; // 锁定瞄准输入

        public WeaponBase[] m_Weapons; // 玩家所拥有的武器
        [HideInInspector]
        public int m_WeaponNum = 0; // 当前装备的武器编号

        public Animator m_Animator; // 玩家动画控制器

        void Awake()
        {
            m_Current = this; // 初始化当前玩家实例
            // m_PlayerPowers = GetComponent<PlayerPowers>(); // 获取玩家技能控制组件
        }

        void Start()
        {
            // m_DamageControl = GetComponent<DamageControl>(); // 获取伤害控制组件
            // m_DamageControl.OnDamaged.AddListener(HandleDamage); // 伤害事件监听
            m_InControl = true; // 玩家可以控制
        }

        // 每帧更新
        void Update()
        {
            m_Input_Fire = false;       // 重置开火输入
            m_Input_Fire2 = false;      // 重置第二开火输入
            m_Input_LockAim = false;    // 重置锁定瞄准输入
            
            // 玩家输入控制
            if (m_InControl)
            {
                m_Input_Fire = PlayerController.MainPlayerController.Input_FireHold; // 获取开火输入
            }

            // 获取玩家的移动输入
            m_MovementInput = PlayerController.MainPlayerController.m_Input_Movement;
            // 计算玩家的转动角度
            Vector3 axis = Vector3.Cross(Vector3.up, m_MovementInput); // 计算移动方向的法线

            // 玩家转向控制
            if (m_MovementInput != Vector3.zero)
            {
                Vector3 faceDirection = m_MovementInput; // 计算面朝方向
                faceDirection.y = 0;
                faceDirection.Normalize(); // 单位化方向
                m_TurnBase.rotation = Quaternion.Lerp(m_TurnBase.rotation, Quaternion.LookRotation(faceDirection),
                    10 * Time.deltaTime); // 平滑过渡旋转
            }
            
            m_Weapons[m_WeaponNum].Input_FireHold = m_Input_Fire;

            // 更新动画参数
            Vector3 vSpeed = GetComponent<Rigidbody>().velocity;
            vSpeed.y = 0; // 仅关注水平速度
            float runSpeed = Mathf.Clamp(vSpeed.magnitude / 10f, 0, 1); // 计算跑步速度
            m_Animator.SetFloat("RunSpeed", runSpeed); // 设置动画中的跑步速度参数
        }

        // 固定时间更新（用于物理）
        void FixedUpdate()
        {
            Rigidbody rigidBody = GetComponent<Rigidbody>();

            Vector3 totalVelocity = rigidBody.velocity;
            if (m_MovementInput != Vector3.zero)
            {
                totalVelocity += 5 * m_MovementInput; // 加上移动输入的速度
                totalVelocity.y = 0; // 保持水平速度
                totalVelocity = Vector3.ClampMagnitude(totalVelocity, 11); // 限制速度最大值
                totalVelocity.y = rigidBody.velocity.y; // 保留垂直速度
                rigidBody.velocity = totalVelocity;
            }
            else
            {
                totalVelocity -= .4f * totalVelocity; // 减速
                totalVelocity.y = rigidBody.velocity.y;
                rigidBody.velocity = totalVelocity;
            }
        }
    }
}
