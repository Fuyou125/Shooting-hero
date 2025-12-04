using System.Collections;
using UnityEngine;

namespace Shooting.Gameplay
{
    // PowerUp 类用于管理游戏中的道具功能，当玩家拾取道具时触发相应的效果
    public class OutBreak : MonoBehaviour
    {
        public int m_SkillId = 0;  // 当前道具的编号，根据不同编号应用不同的效果
        [HideInInspector]
        public bool m_Picked = false;  // 标记道具是否已被拾取
        
        public GameObject m_BulletPrefab1;  // 用于发射的子弹预制体
        public GameObject m_ParticlePrefab1;  // 道具拾取时的粒子效果预制体

        public GameObject m_DisablingPart;  // 禁用的部件（如外观上的一部分，需要禁用或隐藏）
        
        // Start 是在游戏对象激活时调用的初始化方法
        void Start()
        {
            m_Picked = false;  // 初始化道具未被拾取
        }
        
        // 当玩家与道具发生碰撞时触发的事件
        void OnTriggerEnter(Collider coll)
        {
            if (!m_Picked)  // 确保道具未被拾取
            {
                if (coll.gameObject.tag == "Player")  // 判断碰撞对象是否是玩家
                {
                    // 创建道具拾取时的粒子效果
                    GameObject obj = Instantiate(m_ParticlePrefab1);
                    obj.transform.position = transform.position;  // 粒子效果的位置与道具位置一致
                    Destroy(obj, 3);  // 3秒后销毁粒子效果

                    m_Picked = true;  // 标记道具已被拾取
                    GetComponent<Collider>().enabled = false;  // 禁用道具的碰撞体，防止再次拾取
                    m_DisablingPart.SetActive(false);  // 禁用道具的一部分（例如外观上需要隐藏的部件）
                    ApplyPower();  // 应用道具的效果
                }
            }
        }
        
        
        // 应用道具的效果，根据道具编号执行不同的效果
        public void ApplyPower()
        {
            switch(m_SkillId)
            {
                case 0:
                    StartCoroutine(CoroutineFireRingBullets());  // 如果道具编号是0，则执行发射环形子弹的效果
                    break;
            }
        }
        
        // 发射环形子弹的协程
        IEnumerator CoroutineFireRingBullets()
        {
            // 循环发射40个子弹，形成一个环形阵列
            for (int i = 0; i < 40; i++)
            {
                // 创建子弹
                GameObject obj1 = Instantiate(m_BulletPrefab1);
                obj1.transform.position = transform.position;  // 子弹出生位置与道具位置一致

                // 计算子弹的方向，使其沿环形轨迹分布
                Vector3 v = Quaternion.Euler(0, i * 12, 0) * Vector3.forward;  // 每个子弹的发射角度为12度
                obj1.transform.forward = v;  // 设置子弹的前进方向

                // 设置子弹的创建者为当前玩家
                obj1.GetComponent<ProjectileCollisions>().m_Creator = PlayerCharacter.m_Current.gameObject;

                yield return new WaitForSeconds(.01f);  // 每发射一个子弹后暂停0.01秒
            }

            // 子弹发射完成后销毁道具本身
            Destroy(gameObject);
        }
    }
}