using System.Collections;
using System.Collections.Generic;
using Shooting.GamePlay;
using UnityEngine;

namespace Shooting.Gameplay
{
    public class PlayerCharacter : MonoBehaviour
    {
        public bool hasForwardGround;
        public bool isOnGround;
        public DamageController m_DamageControl; // 伤害控制组件
        public static PlayerCharacter m_Current; // 当前玩家实例
        
        private Rigidbody rigidBody;

        [SerializeField] 
        private Transform m_TurnBase; // 转动基础物体
        [SerializeField] 
        private Transform m_AimBase; // 瞄准基础物体

        //-------------------------------
        [HideInInspector] 
        public bool m_InControl = false; // 是否控制玩家

        public Transform[] m_WeaponHands; // 玩家武器手的 Transform
        public Transform m_FirePoint; // 开火点
        public GameObject m_WeaponPowerParticle; // 武器能量粒子效果
        public GameObject m_DeathParticle; // 死亡粒子效果

        Vector3 m_MovementInput; // 玩家移动输入
        Vector3 m_DashDirection; // 冲刺方向
        
        // 台阶攀爬参数
        public float stepHeight = 0.3f;       // 可攀爬最大台阶高度（根据游戏调整，如0.25-0.35）
        public float checkDistance = 0.6f;    // 前方检测距离（建议：胶囊体半径+0.1）
        public float stepSmoothSpeed = 8f;    // 台阶过渡平滑度（值越大越灵敏）
        public LayerMask groundLayer;         // 地面/台阶的Layer（避免检测其他物体）
        
        // 内部状态
        private Vector3 currentVelocity;      // 用于SmoothDamp的临时变量
        private float capsuleHalfHeight;      // 胶囊体半高（缓存以优化性能）

        bool m_Input_Fire; // 开火输入
        bool m_Input_Fire2; // 另一个开火输入（暂时没有用）
        bool m_Input_LockAim; // 锁定瞄准输入

        public CapsuleCollider capsuleCollider;
        
        [HideInInspector]
        public bool m_IsDead = false; // 玩家是否死亡
        
        public WeaponBase[] m_Weapons; // 玩家所拥有的武器
        [HideInInspector]
        public int m_WeaponNum = 0; // 当前装备的武器编号
        
        public TargetsObject m_TempTarget; // 临时瞄准目标
        
        [HideInInspector]
        public int m_WpnPowerLevel = 0; // 武器能量等级
        [HideInInspector]
        public float m_WpnPowerTime = 0; // 武器能量剩余时间

        public Animator m_Animator; // 玩家动画控制器
        
        public GameObject m_GrenadePrefab1; // 手雷预设
        
        [HideInInspector]
        public PlayerSkills m_PlayerSkills; // 玩家技能控制
        
        public GameObject m_ShieldObject; // 玩家护盾对象

        void Awake()
        {
            m_Current = this; // 初始化当前玩家实例
            m_PlayerSkills = GetComponent<PlayerSkills>(); // 获取玩家技能控制组件
            capsuleCollider = GetComponent<CapsuleCollider>();
            rigidBody = GetComponent<Rigidbody>();
            // 计算胶囊体半高（中心到地面的距离）
            capsuleHalfHeight = capsuleCollider.height / 2f - capsuleCollider.radius;
        }

        void Start()
        {
            m_DamageControl = GetComponent<DamageController>(); // 获取伤害控制组件
            m_DamageControl.OnDamaged.AddListener(HandleDamage); // 伤害事件监听
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
                
                // 判断是否使用技能(如投掷手雷)
                if (Input.GetKeyDown(KeyCode.Z))
                {
                    if (m_PlayerSkills.m_HaveSkill) // 判断是否有技能
                    {
                        switch (m_PlayerSkills.m_SkillID)
                        {
                            case 0: // 当前技能：投掷手雷
                                if (m_PlayerSkills.m_AmmoCount > 0)
                                {
                                    ThrowGrenade(); // 投掷手雷
                                    m_PlayerSkills.m_AmmoCount--; // 消耗一个手雷
                                }
                                break;
                        }
                    }
                }

                // 获取玩家的移动输入
                m_MovementInput = PlayerController.MainPlayerController.m_Input_Movement;
                // 计算玩家的转动角度
                Vector3 axis = Vector3.Cross(Vector3.up, m_MovementInput); // 计算移动方向的法线
                
                // 查找目标
                List<TargetsObject> targets = TargetController.m_Main.m_Targets; // 获取目标列表
                
                TargetsObject bestTarget = null;
                float minAngle = 40; // 初始化最小角度为40
                foreach (TargetsObject target in targets)
                {
                    if (target == null) continue;

                    Vector3 targetPos = target.m_TargetCenter.position; // 获取目标位置
                    Vector3 dir = targetPos - transform.position; // 计算目标与玩家的方向
                    dir.y = 0; // 忽略高度差
                    float delta = Vector3.Angle(m_TurnBase.forward, dir); // 计算角度差
                    float distance = dir.magnitude; // 计算距离

                    if (distance > 30) continue; // 距离太远，跳过

                    if (delta < minAngle) // 如果角度差更小
                    {
                        bestTarget = target;
                        minAngle = delta;
                    }
                }
                
                // 如果找到最佳目标，进行瞄准
                if (bestTarget != null)
                {
                    Vector3 targetPos = bestTarget.m_TargetCenter.position;
                    Vector3 targetDir = targetPos - m_FirePoint.position;
                    targetDir.y = 0;
                    m_AimBase.rotation = Quaternion.Lerp(m_AimBase.rotation, Quaternion.LookRotation(targetDir), 20 * Time.deltaTime); // 平滑过渡瞄准方向
                    m_TempTarget = bestTarget; // 更新临时目标
                }
                else
                {
                    m_TempTarget = null; // 没有目标时，清空临时目标
                    m_AimBase.localRotation = Quaternion.Lerp(m_AimBase.localRotation, Quaternion.identity, 20 * Time.deltaTime); // 恢复原始瞄准
                }
                
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
            }

            if (m_WpnPowerLevel == 1)
            {
                m_WpnPowerTime -= Time.deltaTime;
                if (m_WpnPowerTime <= 0)
                {
                    m_WeaponPowerParticle.SetActive(false); // 关闭能量粒子效果
                    SetWeaponPowerLevel(0); // 重置武器能量等级
                }
            }

            // 更新动画参数
            Vector3 vSpeed = GetComponent<Rigidbody>().velocity;
            vSpeed.y = 0; // 仅关注水平速度
            float runSpeed = Mathf.Clamp(vSpeed.magnitude / 10f, 0, 1); // 计算跑步速度
            m_Animator.SetFloat("RunSpeed", runSpeed); // 设置动画中的跑步速度参数
            
            // 更新护盾位置
            m_ShieldObject.transform.position = transform.position + new Vector3(0, 1, 0);

            // 检查玩家是否死亡
            if (!m_IsDead)
            {
                if (m_DamageControl.Damage <= 0) // 玩家死亡条件
                {
                    m_IsDead = true; // 标记玩家死亡
                    GameObject obj = Instantiate(m_DeathParticle); // 创建死亡粒子效果
                    obj.transform.position = transform.position + new Vector3(0, 1, 0);
                    Destroy(obj, 3); // 3秒后销毁粒子效果
                    gameObject.SetActive(false); // 玩家对象不可见
                }
            }
        }

        // 固定时间更新（用于物理）
        void FixedUpdate()
        {
            Vector3 totalVelocity = rigidBody.velocity;
            if (m_MovementInput != Vector3.zero)
            {
                totalVelocity += 5 * m_MovementInput; // 加上移动输入的速度
                totalVelocity.y = 0; // 保持水平速度
                totalVelocity = Vector3.ClampMagnitude(totalVelocity, 11); // 限制速度最大值
                totalVelocity.y = rigidBody.velocity.y - 4.5f; // 保留垂直速度
                rigidBody.velocity = totalVelocity;
                HandleStepClimb(m_MovementInput);
            }
            else
            {
                totalVelocity -= .4f * totalVelocity; // 减速
                totalVelocity.y = rigidBody.velocity.y - 4.5f;
                rigidBody.velocity = totalVelocity;
            }
        }

        private void HandleStepClimb(Vector3 moveDir)
        {
            // 射线1：检测当前地面Y坐标（从胶囊体中心向下）
            RaycastHit currentGroundHit;
            Vector3 currentRayOrigin = transform.position + Vector3.up * (capsuleHalfHeight + 0.1f); // 略微抬高起点，避免穿模
            isOnGround = Physics.Raycast(
                currentRayOrigin,
                Vector3.down,
                out currentGroundHit,
                capsuleHalfHeight + 0.2f,
                groundLayer
            );
            if (!isOnGround) return; // 不在地面上时不处理

            GizmosDisplay.currentGroundY = currentGroundHit.point.y;

            // 射线2：检测前方台阶Y坐标（从胶囊体前方向下）
            RaycastHit forwardGroundHit;
            hasForwardGround = Physics.CapsuleCast(
                transform.position, // 胶囊体底部
                transform.position + Vector3.up * capsuleCollider.height, // 胶囊体顶部
                capsuleCollider.radius,                              // 胶囊体半径
                moveDir,                                             // 检测方向
                out forwardGroundHit,                                  // 碰撞信息
                checkDistance,                                       // 检测距离
                groundLayer                                          // 检测层级
            );
            if (!hasForwardGround) return; // 前方无地面时不处理

            GizmosDisplay.forwardGroundY = forwardGroundHit.point.y;
            GizmosDisplay.heightDiff = GizmosDisplay.forwardGroundY - GizmosDisplay.currentGroundY;
            
            // 条件判断：是否为可攀爬台阶
            bool canClimbStep = GizmosDisplay.heightDiff > 0.05f && GizmosDisplay.heightDiff <= stepHeight; // 排除微小误差和过高台阶
            if (!canClimbStep) return;
            
            // 替换平滑调整为施加力
            float upwardForce = GizmosDisplay.heightDiff * stepSmoothSpeed * 100f;
            rigidBody.AddForce(Vector3.up * upwardForce, ForceMode.Force);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.white;
            Vector3 currentRayStart = transform.position + Vector3.up * (capsuleHalfHeight + 0.1f);
            Vector3 currentRayEnd = currentRayStart + Vector3.down * (capsuleHalfHeight + 0.2f);
            Gizmos.DrawLine(currentRayStart, currentRayEnd);
        }

        // 处理玩家受到伤害
        public void HandleDamage()
        {
            CameraController.m_Current.StartShake(.2f, .1f); // 震动效果
        }
        
        // 更新玩家武器的后坐力
        void LateUpdate()
        {
            float recoil = m_Weapons[m_WeaponNum].RecoilTimer;
            m_WeaponHands[0].localRotation *= Quaternion.Euler(0, -4 * recoil, 0); // 调整武器位置
            m_WeaponHands[1].localRotation *= Quaternion.Euler(0, -4 * recoil, 0);
            m_WeaponHands[0].localPosition += new Vector3(0, 0, -.5f * recoil); // 调整武器位置
        }
        
        // 设置武器的能量等级
        public void SetWeaponPowerLevel(int level)
        {
            m_WpnPowerLevel = level;
            if (level == 1)
            {
                m_WpnPowerTime = 16; // 设置能量持续时间
                m_WeaponPowerParticle.SetActive(true); // 启用能量粒子效果
            }

            // 设置所有武器的能量等级
            foreach (WeaponBase w in m_Weapons)
            {
                w.m_PowerLevel = level;
            }
        }
        
        // 设置玩家装备的武器
        public void SetWeapon(int num)
        {
            foreach (WeaponBase w in m_Weapons)
            {
                w.Input_FireHold = false; // 禁用所有武器的开火输入
            }
            m_WeaponNum = num; // 设置当前装备的武器编号
        }
        
        // 玩家投掷手雷
        public void ThrowGrenade()
        {
            Vector3 start = transform.position;
            Vector3 end = PlayerController.MainPlayerController.AimPosition + new Vector3(0, 1, 0);
            GameObject obj = Instantiate(m_GrenadePrefab1); // 创建手雷
            obj.transform.position = transform.position;
            PlayersGrenade g = obj.GetComponent<PlayersGrenade>(); // 获取手雷控制
            g.m_StartPosition = start; // 设置起始位置
            g.m_TargetPosition = end; // 设置目标位置
        }
        
        // 处理玩家拾取的物品
        public void HandlePickup(string itemType, int count)
        {
            if (itemType == "Gem")
            {
                PlayerController.MainPlayerController.m_GemCount++; // 增加宝石数量
            }
            else if (itemType == "WeaponPower")
            {
                SetWeaponPowerLevel(1); // 设置武器能量
            }
            else if (itemType == "Weapon_Pistol")
            {
                SetWeapon(0); // 切换为手枪
            }
            else if (itemType == "Weapon_Shotgun")
            {
                SetWeapon(1); // 切换为霰弹枪
            }
            else if (itemType == "Skill_Grenade")
            {
                m_PlayerSkills.SetNewSkill(0); // 设置新的技能
            }
            else if (itemType == "Health")
            {
                m_DamageControl.AddHealth(count); // 增加玩家生命
            }
        }
    }
}
