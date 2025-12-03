using UnityEngine;
using UnityEngine.Events;

namespace Shooting
{
    public class DamageController : MonoBehaviour
    {
        // 当前伤害值
        [HideInInspector]
        public float Damage = 100;

        // 最大伤害值
        public float MaxDamage = 100;

        
        // 伤害事件，受到伤害时触发
        public UnityEvent OnDamaged;
        
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
        
        // 在对象初始化时调用
        void Awake()
        {
            // 初始化伤害事件
            OnDamaged = new UnityEvent();
        }
        
        // 施加伤害的函数
        public void ApplyDamage(float dmg, Vector3 dir, float DamageFactor)
        {
            // 如果免疫伤害或者已经死亡，直接返回
            if (m_NoDamage || IsDead)
                return;

            // 触发伤害抖动效果
            // ApplyDamageShake();

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
            // StartCoroutine(CoroutineHitGlow());
        }
    }
}