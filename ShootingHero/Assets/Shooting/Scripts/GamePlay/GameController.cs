using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Shooting
{
    public class GameController : MonoBehaviour
    {
        // 当前游戏控制器的静态引用
        public static GameController m_Current;
        
        // 游戏的主存档数据
        public SaveDatas m_MainSaveData;
        
        // 在游戏开始时初始化
        void Awake()
        {
            m_Current = this;
        }
        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            
        }
        
        // 处理存档点
        public void HandleCheckpoint(int num)
        {
            if (num > m_MainSaveData.m_CheckpointNumber)
            {
                // 如果新的存档点比当前存档点大，更新存档数据
                m_MainSaveData.m_CheckpointNumber = num;
                m_MainSaveData.Save(); // 保存数据
            }
        }
        
        // 处理玩家死亡
        public void HandlePlayerDeath()
        {
            // StartCoroutine(CoroutineHandleGameOver());
        }
    }
}