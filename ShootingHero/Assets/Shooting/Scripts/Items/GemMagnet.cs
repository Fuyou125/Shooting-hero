using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shooting.Gameplay
{
    public class GemMagnet : MonoBehaviour
    {
        public PlayerCharacter Character;
    
        void Start()
        {
            Character = PlayerCharacter.m_Current;
            StartCoroutine(HandleMagnet());
        }

        IEnumerator HandleMagnet()
        {
            yield return new WaitForSeconds(3f);
            Vector3 startPos = transform.position;  // 记录物品初始位置
            float lerp = 0;  // 插值变量，控制物品移动
            while (lerp <= 1)
            {
                // 物品位置根据插值缓慢移动到玩家的位置（+1是让物品在玩家上方）
                transform.position = Vector3.Lerp(startPos, Character.transform.position + new Vector3(0, 1, 0), lerp);
                lerp += 10 * Time.deltaTime;  // 更新插值
                yield return null;  // 等待下一帧
            }
        }

    }
}

