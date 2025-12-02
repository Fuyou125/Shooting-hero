using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shooter.Gameplay
{
    public class Barrel : MonoBehaviour
    {
        [HideInInspector]
        public DamageControl m_DamageControl;  // 用于处理伤害的控制器

        public GameObject m_ExplodeParticle;  // 爆炸时的粒子效果

        bool exploded = false;  // 标记是否已爆炸

        // Start是游戏开始时调用一次的方法
        void Start()
        {
            m_DamageControl = GetComponent<DamageControl>();  // 获取物体上的DamageControl组件
        }

        // Update是每一帧调用的方法
        void Update()
        {
            if (!exploded)  // 如果尚未爆炸
            {
                // 如果检测到Barrel已死亡
                if (m_DamageControl.IsDead)
                {
                    exploded = true;  // 设置为已爆炸
                    Invoke("Explode", .2f);  // 延迟0.2秒调用爆炸方法
                }
            }
        }

        // 处理爆炸的逻辑
        public void Explode()
        {
            GameObject obj = Instantiate(m_ExplodeParticle);  // 实例化爆炸粒子效果
            obj.transform.position = transform.position;  // 将粒子效果放置在Barrel的位置
            Destroy(obj, 6);  // 6秒后销毁粒子效果
            Destroy(gameObject);  // 销毁当前Barrel物体
        }
    }
}