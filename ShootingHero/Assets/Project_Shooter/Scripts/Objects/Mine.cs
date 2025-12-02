using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shooter
{
    public class Mine : MonoBehaviour
    {
        public GameObject m_ExplodeParticle;  // 用于爆炸效果的粒子预制体

        bool exploded = false;  // 标记地雷是否已经爆炸

        // 当触发器与其他物体发生碰撞时调用
        void OnTriggerEnter(Collider coll)
        {
            if (!exploded)  // 如果地雷尚未爆炸
            {
                // 如果碰撞的物体不是玩家
                if (coll.gameObject.tag != "Player")
                {
                    exploded = true;  // 设置地雷为已爆炸
                    Invoke("Explode", 0.2f);  // 延迟0.2秒后调用爆炸方法
                }
            }
        }

        // 处理爆炸的逻辑
        public void Explode()
        {
            // 实例化爆炸粒子效果
            GameObject obj = Instantiate(m_ExplodeParticle);
            obj.transform.position = transform.position;  // 设置粒子效果的位置为地雷的位置
            Destroy(obj, 6);  // 6秒后销毁爆炸粒子效果

            // 销毁当前地雷
            Destroy(gameObject);
        }
    }
}