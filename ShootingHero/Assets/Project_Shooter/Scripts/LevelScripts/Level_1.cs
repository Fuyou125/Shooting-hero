using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Shooter.UI;

namespace Shooter.Gameplay
{
    public class Level_1 : MonoBehaviour
    {
        public GameObject m_BossObject;  // Boss的游戏对象
        [HideInInspector]
        public bool m_KilledBoss = false;  // 是否击败了Boss
        public GameObject m_BossBlock1;  // 用于显示Boss战斗场景的UI元素
        public GameObject m_BossBlockBack1;  // Boss战斗背景UI元素

        public EnemySpawnPoint[] m_BossSpawnPoints;  // 小怪物生成点的数组
        
        void Start()
        {
            // 初始化时隐藏Boss战斗背景
            m_BossBlockBack1.SetActive(false);
        }

        void Update()
        {
            if (!m_KilledBoss)
            {
                // 如果Boss尚未击败，检查Boss是否死亡
                DamageControl bossDamage = m_BossObject.GetComponent<DamageControl>();
                if (bossDamage.IsDead)
                {
                    m_KilledBoss = true;  // 设置Boss已击败
                    CameraControl.m_Current.m_BossTarget = null;  // 移除Boss目标
                    m_BossBlock1.SetActive(false);  // 隐藏Boss战UI元素
                    CameraControl.m_Current.m_BackBlock.gameObject.SetActive(true);  // 显示背景UI
                }
            }
        }

        public void StartBossFight()
        {
            // 开始Boss战斗
            CameraControl.m_Current.m_BossTarget = m_BossObject.transform;  // 设置镜头锁定在Boss身上
            m_BossObject.GetComponent<Enemy>().EnableEnemy();  // 激活Boss
            m_BossBlockBack1.SetActive(true);  // 显示Boss战斗背景UI
            UI_HUD.m_Main.ShowBossHealth();  // 显示Boss血条
            CameraControl.m_Current.m_BackBlock.gameObject.SetActive(false);  // 隐藏背景UI
            StartCoroutine(CoroutineSpawnSmallEnemies());  // 开启小怪生成协程
        }

        // 小怪生成协程
        IEnumerator CoroutineSpawnSmallEnemies()
        {
            int num = 0;
            yield return new WaitForSeconds(6);  // 等待6秒钟后开始生成小怪

            for (int i = 0; i < 10; i++)  // 生成10个小怪
            {
                m_BossSpawnPoints[num].SpawnEnemy();  // 在指定位置生成小怪
                if (num == 0)
                    num = 1;  // 如果当前是0，则切换为1
                else
                    num = 0;  // 否则切换为0
                yield return new WaitForSeconds(5);  // 每次生成小怪后等待5秒
            }
        }

        public void EndLevel()
        {
            // 结束关卡
            StartCoroutine(CoroutineEndLevel());
        }

        // 结束关卡的协程
        IEnumerator CoroutineEndLevel()
        {
            GameControl.m_Current.m_MainSaveData.m_CheckpointNumber = 0;  // 重置存档的检查点编号
            GameControl.m_Current.m_MainSaveData.Save();  // 保存游戏进度
            yield return new WaitForSeconds(1);  // 等待1秒
            FadeControl.m_Current.StartFadeOut();  // 开始渐出动画
            yield return new WaitForSeconds(2);  // 等待2秒
            SceneManager.LoadScene("EndScene");  // 加载“EndScene”场景，结束关卡
        }
    }
}
