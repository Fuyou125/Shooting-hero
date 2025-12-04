
using UnityEngine;

namespace Shooting.Gameplay
{
    // 这是手枪武器类，继承自 Weapon_Base，负责手枪的射击逻辑。
    public class P_Pistol : WeaponBase
    {
        
        // 每帧更新手枪的状态，包括开火延迟、后坐力和射击逻辑
        void Update()
        {
            // 根据武器的当前等级设置不同的开火延迟
            if (m_PowerLevel == 0)
            {
                FireDelay = .2f;  // 普通模式下的开火延迟
            }
            else if (m_PowerLevel == 1)
            {
                FireDelay = .17f; // 强化模式下的开火延迟
            }

            // 更新开火延迟计时器
            FireDelayTimer -= Time.deltaTime;
            if (FireDelayTimer <= 0)
                FireDelayTimer = 0;

            // 更新后坐力计时器
            RecoilTimer -= 10 * Time.deltaTime;
            if (RecoilTimer <= 0)
                RecoilTimer = 0;

            // 按住开火键时，执行开火操作
            if (Input_FireHold)
            {
                if (FireDelayTimer == 0)  // 如果开火延迟已经结束，可以射击
                {
                    FireWeapon(); // 调用 FireWeapon 方法执行开火
                    FireDelayTimer = FireDelay;  // 重置开火延迟计时器
                    RecoilTimer = 1f;  // 重置后坐力计时器
                }
            }

            // Input_FireHold = false; // 这里注释掉的代码没有起作用，可以移除
        }
        
        // 开火逻辑，根据不同的武器等级执行不同的射击方式
        public override void FireWeapon()
        {
            GameObject obj;

            // 普通模式下，发射单颗子弹
            if (m_PowerLevel == 0)
            {
                obj = Instantiate(BulletPrefab);  // 实例化子弹Prefab
                obj.transform.position = m_FirePoint.position;  // 设置子弹的位置为开火点
                obj.transform.forward = m_FirePoint.forward;    // 设置子弹的方向为开火点的方向
                ProjectileBase proj = obj.GetComponent<ProjectileBase>();
                proj.Creator = m_Owner;
                proj.Speed = ProjectileSpeed;
                proj.Damage = Damage;
                proj.m_Range = Range;
                Destroy(obj,5);  // 5秒后销毁子弹对象
            }
            // 强化模式下，发射三颗子弹（中心+两侧）
            else if (m_PowerLevel == 1)
            {
                // 从-1到1之间发射三颗子弹，分别朝不同的角度发射
                for (int i = -1; i < 2; i++)
                {
                    obj = Instantiate(BulletPrefab);  // 实例化子弹Prefab
                    obj.transform.position = m_FirePoint.position;  // 设置子弹的位置为开火点
                    obj.transform.forward = Quaternion.Euler(0, i * 10, 0) * m_FirePoint.forward;  // 设置子弹的发射方向，左右偏移一定角度
                    ProjectileBase proj = obj.GetComponent<ProjectileBase>();  // 获取子弹的组件
                    proj.Creator = m_Owner;  // 设置子弹的创建者
                    proj.Speed = ProjectileSpeed;  // 设置子弹的速度
                    proj.Damage = Damage;  // 设置子弹的伤害
                    proj.m_Range = Range;  // 设置子弹的射程
                    Destroy(obj, 5);  // 5秒后销毁子弹对象
                }
            }
            
            // 实例化并播放射击效果（例如火焰或烟雾）
            obj = Instantiate(EffectPrefab);  
            obj.transform.SetParent(m_ParticlePoint);  // 设置效果的父物体为粒子点
            obj.transform.localPosition = Vector3.zero;  // 设置效果的局部位置为(0, 0, 0)
            obj.transform.forward = m_ParticlePoint.forward;  // 设置效果的方向
            Destroy(obj, 3);  // 3秒后销毁效果对象
        }
    }
}