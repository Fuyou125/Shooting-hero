using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shooter.Gameplay
{
    public class ShootSwitch_A : MonoBehaviour
    {
        [HideInInspector]
        public bool m_Activated = false;  // 标记开关是否已被激活

        public Door_A m_Door;  // 关联的门（当开关激活时会打开该门）

        public GameObject[] m_ActiveBases;  // 控制开关显示的两个不同状态的对象

        // Start 在游戏开始时调用一次
        void Start()
        {
            // 初始状态下，第一个对象显示，第二个对象隐藏
            m_ActiveBases[0].SetActive(true);
            m_ActiveBases[1].SetActive(false);
        }

        // Update 每帧调用一次
        void Update()
        {
            // 获取当前物体的 DamageControl 组件（用于判断是否死亡）
            DamageControl damage = GetComponent<DamageControl>();

            // 如果开关没有被激活
            if (!m_Activated)
            {
                // 如果 DamageControl 组件判断该物体已死亡（例如被射击）
                if (damage.IsDead)
                {
                    // 激活开关
                    m_Activated = true;

                    // 将第一个对象隐藏，第二个对象显示
                    m_ActiveBases[0].SetActive(false);
                    m_ActiveBases[1].SetActive(true);

                    // 播放第二个对象的音效
                    m_ActiveBases[1].GetComponent<AudioSource>().Play();

                    // 延迟0.5秒后打开门
                    Invoke("OpenDoor", .5f);
                }
            }
        }

        // 打开门的方法
        public void OpenDoor()
        {
            // 如果门对象存在，调用门的打开方法
            if (m_Door != null)
            {
                m_Door.Open();
            }
        }
    }
}