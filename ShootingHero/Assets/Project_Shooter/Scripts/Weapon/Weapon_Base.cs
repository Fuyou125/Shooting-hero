using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Shooter.Gameplay
{
    // 这是武器基类，其他具体武器类型会继承该类并实现具体的开火逻辑
    public class Weapon_Base : MonoBehaviour
    {
        // 武器ID（用于区分不同的武器）
        public int WpnID = 0;

        // 武器名称（例如 "SMG 1"）
        public string Title = "SMG 1";

        // 是否自动开火（自动开火时，持续按住开火键可不断射击）
        public bool AutoFire = true;

        // 开火延迟（每次开火之间的时间间隔）
        public float FireDelay = 0.2f;

        // 连发延迟（多次射击时每次发射之间的延迟时间）
        public float BurstDelay = .5f;

        // 连发次数（每次连发多少次子弹）
        public int BurstFireCount = 3;

        // 后坐力速度（控制后坐力恢复的速度）
        public float RecoilSpeed = 5;

        [Space]
        // 初始化弹药量（刚拿到武器时的弹药数量）
        public int InitAmmo = 80;

        // 最大弹药量（武器的最大弹药数）
        public int MaxAmmo = 80;

        // 每次补充弹药数量（每次补充的弹药数量）
        public int AddAmmo = 10;

        // 强化武器的最大弹药量
        public int PowerWeaponMaxAmmo = 80;

        [Space]
        // 武器图标（显示在UI上的图标）
        public Sprite WeaponIcon;

        // 武器伤害（每颗子弹造成的伤害）
        public float Damage = 1;

        // 子弹速度（子弹飞行速度）
        public float ProjectileSpeed = 200;

        // 子弹射程（子弹能够飞行的最大距离）
        public float Range = 100;

        // 是否为无限弹药（如果为真，弹药永远不会减少）
        public bool InfiniteAmmo = false;

        [Space]
        // 武器模型预制件（表示武器外观的模型）
        public GameObject WeaponModelPrefab;

        // 子弹预制件（表示子弹的预制件）
        public GameObject BulletPrefab;

        // 开火特效预制件（开火时的特效，例如火焰、枪口闪光等）
        public GameObject EffectPrefab;

        [Space]
        [HideInInspector]
        // 武器是否启用（是否激活武器）
        public bool WeaponEnable = true;

        [HideInInspector]
        // 当前弹药数量
        public int AmmoCount = 50;

        // 开火点位置（子弹从这里发射）
        public Transform m_FirePoint;

        // 粒子点位置（特效的位置）
        public Transform m_ParticlePoint;

        // 武器的拥有者（通常是玩家或敌人）
        public GameObject m_Owner;

        [HideInInspector]
        // 当前武器的拥有者（具体的玩家控制对象）
        public PlayerControl Owner;

        [HideInInspector]
        // 开火延迟计时器
        public float FireDelayTimer = 0;

        [HideInInspector]
        // 连发延迟计时器
        public float BurstDelayTimer = 0;

        [HideInInspector]
        // 连发计数器，记录已经发射的子弹次数
        public int BurstFireCounter = 0;

        [HideInInspector]
        // 后坐力计时器
        public float RecoilTimer = 0;

        [HideInInspector]
        // 是否按住开火键
        public bool Input_FireHold = false;

        [HideInInspector]
        // 武器朝向
        public Vector3 Forward;

        [HideInInspector]
        // 武器的强化等级
        public int m_PowerLevel = 0;

        // 初始化函数（可以用来初始化一些数据）
        void Start()
        {

        }

        // 每帧更新函数，负责处理武器的开火逻辑和状态更新
        void Update()
        {
            // 更新开火延迟计时器
            FireDelayTimer -= Time.deltaTime;
            if (FireDelayTimer <= 0)
                FireDelayTimer = 0;

            // 更新连发延迟计时器
            BurstDelayTimer -= Time.deltaTime;
            if (BurstDelayTimer <= 0)
                BurstDelayTimer = 0;

            // 更新后坐力计时器
            RecoilTimer -= RecoilSpeed * Time.deltaTime;
            if (RecoilTimer <= 0)
                RecoilTimer = 0;

            // 如果按住开火键
            if (Input_FireHold)
            {
                if (FireDelayTimer == 0) // 如果开火延迟计时器为0，允许开火
                {
                    if (BurstDelayTimer == 0) // 如果连发延迟计时器为0，开始连发
                    {
                        // 如果有弹药或是无限弹药
                        if (AmmoCount > 0 || InfiniteAmmo)
                        {
                            // 如果是玩家控制的武器，触发屏幕震动效果
                            if (Owner == PlayerControl.MainPlayerController)
                            {
                                CameraControl.m_Current.StartShake(.2f, 1f);
                            }
                            FireWeapon();  // 开火
                            AmmoCount -= 1;  // 扣除一发弹药
                            RecoilTimer = 1;  // 设置后坐力计时器
                            BurstFireCounter++;  // 增加连发计数器
                            if (BurstFireCounter >= BurstFireCount)
                            {
                                BurstDelayTimer = BurstDelay;  // 重置连发延迟计时器
                                BurstFireCounter = 0;  // 重置连发计数器
                            }
                        }
                        else
                        {
                            // 播放空枪点击音效
                            //SoundGallery.PlaySound("EmptyFire1");
                        }
                        FireDelayTimer = FireDelay;  // 重置开火延迟计时器
                    }
                }
            }

            // 每帧结束时重置开火状态
            Input_FireHold = false;
        }

        // 开火的实际实现方法，根据武器的不同等级发射不同的子弹
        public virtual void FireWeapon()
        {
            GameObject obj;
            // 发射三颗子弹（形成一个小的散射效果）
            for (int i = -1; i < 2; i++)
            {
                obj = Instantiate(BulletPrefab);  // 实例化子弹
                obj.transform.position = m_FirePoint.position;  // 设置子弹位置为开火点
                obj.transform.forward = Quaternion.Euler(0, i * 10, 0) * m_FirePoint.forward;  // 设置子弹的发射方向（稍微偏移，形成散射效果）
                Projectile_Base proj = obj.GetComponent<Projectile_Base>();  // 获取子弹的基础组件
                proj.Creator = m_Owner;  // 设置子弹的创建者
                proj.Speed = ProjectileSpeed;  // 设置子弹的速度
                proj.Damage = Damage;  // 设置子弹的伤害
                Destroy(obj, 5);  // 5秒后销毁子弹
            }

            // 实例化开火特效
            obj = Instantiate(EffectPrefab);
            obj.transform.position = m_FirePoint.position;  // 设置特效位置为开火点
            obj.transform.forward = m_FirePoint.forward;  // 设置特效的方向
            Destroy(obj, 3);  // 3秒后销毁特效
        }
    }
}
