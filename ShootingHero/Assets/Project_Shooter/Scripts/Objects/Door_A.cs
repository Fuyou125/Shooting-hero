using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shooter.Gameplay
{
    public class Door_A : MonoBehaviour
    {
        public Transform[] m_DoorBases;  // 门的基础部分（通常是门的左右两扇以及底部）

        // 打开门的方法
        public void Open()
        {
            //m_DoorBody.gameObject.SetActive(false);  // 可选：关闭门的主体部分（此处代码被注释掉）
            StartCoroutine(CoroutineOpen());  // 启动打开门的协程
        }

        // 关闭门的方法（当前未实现功能）
        public void Close()
        {
            //m_DoorBody.gameObject.SetActive(true);  // 可选：重新激活门的主体部分（此处代码被注释掉）
        }

        // 打开门的协程
        IEnumerator CoroutineOpen()
        {
            // 先将门的底部部分向下移动
            for (int i = 0; i < 50; i++)
            {
                // 修改门底部的局部位置，模拟门底部向下打开的效果
                m_DoorBases[2].localPosition = new Vector3( 0, -3.74f * i * 0.02f, 0);
                yield return null;  // 等待下一帧继续执行
            }
            
            yield return new WaitForSeconds(0.5f);  // 等待0.5秒，模拟门底部运动后的停顿

            // 接着将门的左右两扇门分别向外打开
            for (int i = 0; i < 100; i++)
            {
                // 左门向左移动
                m_DoorBases[0].localPosition = new Vector3(4 * i * 0.01f, 0, 0);
                // 右门向右移动
                m_DoorBases[1].localPosition = new Vector3(-4 * i * 0.01f, 0, 0);
                yield return null;  // 等待下一帧继续执行
            }
        }
    }
}