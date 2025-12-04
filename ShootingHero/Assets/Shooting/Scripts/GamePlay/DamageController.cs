using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using Shooting.GamePlay;

namespace Shooting
{
    public class DamageController : MonoBehaviour
    {
        // 当前伤害值
        [HideInInspector]
        public float Damage = 100;

        // 最大伤害值
        public float MaxDamage = 100;
        
        // 是否死亡
        [HideInInspector]
        public bool IsDead = false;

        // 是否免疫伤害
        [HideInInspector]
        public bool m_NoDamage = false;
        
        // 上次受到伤害的方向
        [HideInInspector]
        public Vector3 LastDamageDirection;
        
        // 上次伤害的系数
        [HideInInspector]
        public float LastDamageFactor = 1;
        
        // 伤害时的屏幕抖动幅度
        [HideInInspector]
        public float DamageShakeAmount;
        
        // 伤害时的屏幕抖动角度
        [HideInInspector]
        public float DamageShakeAngle;
        
        // 伤害事件，受到伤害时触发
        public UnityEvent OnDamaged;

        
        // 在对象初始化时调用
        void Awake()
        {
            // 初始化伤害事件
            OnDamaged = new UnityEvent();
        }
        
        // 在开始时调用
        void Start()
        {
            // 设置初始伤害为最大伤害值
            Damage = MaxDamage;
            IsDead = false;
            LastDamageDirection = Vector3.forward;
            DamageShakeAmount = 0;
            DamageShakeAngle = 0;
        }
        
        // 每帧更新
        void Update()
        {
            // 逐渐减少伤害时的抖动幅度
            DamageShakeAmount -= 12 * Time.deltaTime;
            if (DamageShakeAmount <= 0)
                DamageShakeAmount = 0;
        }
        
        // 施加伤害的函数
        public void ApplyDamage(float dmg, Vector3 dir, float DamageFactor)
        {
            // 如果免疫伤害或者已经死亡，直接返回
            if (m_NoDamage || IsDead)
                return;

            // 触发伤害抖动效果
            ApplyDamageShake();

            // 更新上次伤害的方向
            LastDamageDirection = dir;
            LastDamageDirection.Normalize();
            LastDamageFactor = DamageFactor;

            // 减少当前生命值
            Damage -= dmg;

            // 如果生命值小于等于0，标记为死亡
            if (Damage <= 0)
            {
                Damage = 0;
                IsDead = true;
            }

            // 触发伤害事件
            OnDamaged.Invoke();

            // 启动受击闪光协程
            StartCoroutine(CoroutineHitGlow());
        }
        
        // 受击时的闪光效果
        public IEnumerator CoroutineHitGlow()
        {
            // 获取所有子物体上的 HitGlowControl 组件
            HitGlowController[] glowControls = GetComponentsInChildren<HitGlowController>();
            foreach (HitGlowController item in glowControls)
            {
                item.SetGlow(); // 设置发光效果
            }
        
            // 等待0.1秒
            yield return new WaitForSeconds(.1f);
        
            foreach (HitGlowController item in glowControls)
            {
                item.SetOriginal(); // 恢复原始效果
            }
        
            yield return null;
        }
        
        // 增加生命值的函数
        public void AddHealth(float h)
        {
            // 将当前生命值增加 h，确保不会超过最大生命值或小于0
            Damage = Mathf.Clamp(Damage + h, 0, MaxDamage);
        }

        // 施加伤害时的抖动效果
        public void ApplyDamageShake()
        {
            // 如果当前没有抖动效果，则开始抖动
            if (DamageShakeAmount == 0)
            {
                DamageShakeAmount = 1;
                DamageShakeAngle = Random.Range(-1f, 1f); // 随机生成抖动角度
            }
        }
        
        // 重置生命值和死亡状态
        public void Reset()
        {
            Damage = MaxDamage; // 生命值重置为最大值
            IsDead = false; // 标记为未死亡
        }
    }
}