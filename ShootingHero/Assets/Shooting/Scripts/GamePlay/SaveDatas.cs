using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shooting
{
    // SaveData 类用于保存和加载游戏进度的数据，包括存储关卡进度和最后解锁的关卡
    // 使用 CreateAssetMenu 特性，使得可以在 Unity 编辑器中创建 SaveData 脚本化对象
    [CreateAssetMenu(fileName = "SaveDatas", menuName = "CustomObjects/SaveDatas", order = 1)]
    public class SaveDatas : ScriptableObject
    {
        // 存储当前玩家进度的检查点编号
        public int m_CheckpointNumber = 0;

        // 将数据保存到 PlayerPrefs
        public void Save()
        {
            // 保存检查点的编号到 PlayerPrefs
            PlayerPrefs.SetInt("m_CheckpointNumber", m_CheckpointNumber);

            // 确保数据持久化保存
            PlayerPrefs.Save();
        }

        // 从 PlayerPrefs 中加载保存的数据
        public void Load()
        {
            // 从 PlayerPrefs 中读取检查点的编号，若未找到则默认为 0
            m_CheckpointNumber = PlayerPrefs.GetInt("m_CheckpointNumber", 0);
        }
    }
}