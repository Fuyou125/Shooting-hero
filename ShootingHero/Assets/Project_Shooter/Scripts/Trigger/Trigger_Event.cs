using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Shooter
{
    // 触发事件的脚本
    public class Trigger_Event : MonoBehaviour
    {
        public UnityEvent Event_OnEnter; // 玩家进入触发器时调用的事件

        // 当其他物体进入触发器时调用
        void OnTriggerEnter(Collider coll)
        {
            // 如果碰撞物体是玩家
            if (coll.gameObject.tag == "Player")
            {
                // 执行在 Unity Inspector 中绑定的事件
                Event_OnEnter.Invoke();

                // 注释掉的代码：禁用碰撞器（此功能被禁用）
                //GetComponent<Collider>().enabled = false;

                // 禁用当前的游戏对象，避免重复触发
                gameObject.SetActive(false);
            }
        }
    }
}