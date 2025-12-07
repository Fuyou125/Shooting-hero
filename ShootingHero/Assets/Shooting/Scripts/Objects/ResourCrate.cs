using System.Collections;
using System.Collections.Generic;
using Shooting.GamePlay;
using UnityEngine;

namespace Shooting.Gameplay
{
    public class ResourCrate : MonoBehaviour
    {
        public GameObject[] m_ItemPrefabs;  // 用于资源箱中掉落物品的预制体数组
        
        [HideInInspector]
        public DamageController m_DamageController;  // 伤害控制组件，用于检测资源箱的血量
        
        public GameObject m_ExplodeParticle;  // 爆炸粒子效果的预制体

        public GameObject m_LidObject;  // 资源箱盖子对象
        public Transform m_LidTransform;  // 资源箱盖子的变换组件
        
        bool m_Opened = false;  // 标记资源箱是否已打开
        
        // Start 在游戏开始时调用一次
        void Start()
        {
            m_DamageController = GetComponent<DamageController>();  // 获取伤害控制组件
        }

        // Update 每帧调用一次
        void Update()
        {
            // 如果资源箱没有被打开且已死亡
            if (!m_Opened)
            {
                if (m_DamageController.IsDead)  // 判断资源箱是否已破坏
                {
                    // 移除目标
                    TargetController.m_Main.RemoveTarget(GetComponent<TargetsObject>());
                    // 禁用碰撞器，避免后续与物体发生碰撞
                    GetComponent<Collider>().enabled = false;

                    // 启动打开资源箱的协程
                    StartCoroutine(CoroutineOpenChest());
                }
            }
        }
        
        // 打开资源箱的协程
        IEnumerator CoroutineOpenChest()
        {
            m_Opened = true;  // 标记资源箱已打开

            // 创建爆炸效果并显示
            GameObject obj = Instantiate(m_ExplodeParticle);
            obj.transform.position = transform.position;
            Destroy(obj, 6);  // 6秒后销毁爆炸效果

            float lerp = 0;  // 用于平滑旋转的插值变量

            // 逐渐旋转盖子
            while (true)
            {
                // 让盖子旋转
                m_LidObject.transform.localRotation = Quaternion.Euler(lerp * -130, 0, 0);
                lerp += 2 * Time.deltaTime;  // 增加插值，控制旋转速度
                if (lerp >= 1)  // 当旋转完成时停止
                    break;
                yield return null;  // 等待下一帧继续执行
            }

            // 最终确保盖子旋转到完全打开
            m_LidObject.transform.localRotation = Quaternion.Euler(-130, 0, 0);

            // 启动掉落物品的协程
            StartCoroutine(CoroutineCreateGems());

            // 等待1.4秒后再执行下一步
            yield return new WaitForSeconds(1.4f);

            // 再次生成爆炸效果
            obj = Instantiate(m_ExplodeParticle);
            obj.transform.position = transform.position;
            Destroy(obj, 6);  // 6秒后销毁爆炸效果

            // 销毁资源箱本身
            Destroy(gameObject);
        }
        
        // 创建掉落物品的协程
        IEnumerator CoroutineCreateGems()
        {
            // 循环创建10个物品
            for (int i = 0; i < 10; i++)
            {
                // 实例化掉落物品
                GameObject obj1 = Instantiate(m_ItemPrefabs[0]);
                obj1.transform.position = transform.position;

                // 计算物品的掉落位置
                Vector3 v = Helpers.RotatedLength(i * 36, 20) + new Vector3(0, 12, 0);
                obj1.GetComponent<Rigidbody>().velocity = v;  // 设置物品的速度，使其掉落

                yield return new WaitForSeconds(0.1f);  // 每次生成物品后，等待0.1秒
            }
        }
    }
}
