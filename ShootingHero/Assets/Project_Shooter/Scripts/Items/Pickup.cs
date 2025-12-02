using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shooter.Gameplay
{
    public class Pickup : MonoBehaviour
    {
        public GameObject m_ScoreParticle;  // 用于显示得分的粒子效果
        bool picked = false;  // 标记物品是否已经被拾取

        [HideInInspector]
        public PlayerChar m_TouchedPlayer;  // 被触碰的玩家角色

        [Space]
        public string m_ItemType = "Cash";  // 物品类型（例如 "Cash"）
        public int m_ItemCount = 1;  // 物品数量

        public bool m_CanPick = false;  // 是否可以拾取该物品

        // Start is called before the first frame update
        void Start()
        {
            m_CanPick = false;  // 开始时物品不可拾取
            Invoke("AllowPick", .5f);  // 延迟0.5秒后允许拾取
        }

        // Update is called once per frame
        void Update()
        {
            // 物品旋转的代码被注释掉了
            //m_Base.localRotation = Quaternion.Euler(0, Time.deltaTime * 100, 0) * m_Base.localRotation;

            // 如果物品未被拾取并且可以拾取
            if (!picked && m_CanPick)
            {
                // 检查玩家与物品之间的距离是否小于等于5单位
                if (Vector3.Distance(transform.position, PlayerChar.m_Current.transform.position) <= 5f)
                {
                    m_TouchedPlayer = PlayerChar.m_Current;  // 记录当前接触的玩家
                    picked = true;  // 标记物品已被拾取
                    Collider m_PhysCollider = GetComponent<Collider>();  // 获取物体的碰撞器

                    if (m_PhysCollider != null)
                    {
                        m_PhysCollider.enabled = false;  // 禁用碰撞器，防止再次拾取
                    }

                    StartCoroutine(CoroutineHandlePick());  // 开始协程处理拾取过程
                }
            }
        }

        // 虚方法，处理拾取物品的逻辑
        public virtual void HandlePickup()
        {
            m_TouchedPlayer.HandlePickup(m_ItemType, m_ItemCount);  // 通过玩家角色来处理拾取的物品
        }

        // 协程处理物品从地面到玩家手中的动画
        IEnumerator CoroutineHandlePick()
        {
            Vector3 startPos = transform.position;  // 记录物品初始位置
            float lerp = 0;  // 插值变量，控制物品移动
            while (lerp <= 1)
            {
                // 物品位置根据插值缓慢移动到玩家的位置（+1是让物品在玩家上方）
                transform.position = Vector3.Lerp(startPos, m_TouchedPlayer.transform.position + new Vector3(0, 1, 0), lerp);
                lerp += 10 * Time.deltaTime;  // 更新插值
                yield return null;  // 等待下一帧
            }

            // 确保物品最终位置在玩家上方
            transform.position = m_TouchedPlayer.transform.position + new Vector3(0, 1, 0);

            // 调用拾取物品的逻辑
            HandlePickup();

            // 实例化得分粒子效果并设置位置
            GameObject obj = Instantiate(m_ScoreParticle);
            obj.transform.position = transform.position;
            Destroy(obj, 1);  // 1秒后销毁粒子效果

            // 销毁物品本身
            Destroy(gameObject);
        }

        // 允许物品被拾取
        private void AllowPick()
        {
            m_CanPick = true;  // 设置物品为可拾取状态
        }
    }
}
