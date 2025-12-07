using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Shooting.Gameplay
{ 
    // 游戏控制类，用于管理游戏流程、关卡、存档、暂停等功能
    public class GameController : MonoBehaviour
    {
        // 当前游戏控制器的静态引用
        public static GameController m_Current;
        
        // 是否进入了Boss战
        public bool m_EnteredBossFight = false;
        // 
        public GameObject m_LevelBoss;
        
        // 游戏的主存档数据
        public SaveDatas m_MainSaveData;
        
        // 用于显示文本UI的对象
        public GameObject m_TextUI_1;

        // 存储教程UI的对象
        public GameObject[] m_Tutorials;
        
        // 暂停UI面板
        public GameObject m_PauseUI;
        
        // 是否已暂停
        public bool m_Pausesd = false;
        
        // 在游戏开始时初始化
        void Awake()
        {
            m_Current = this;
        }
        // 游戏开始时的协程
        void Start()
        {
            StartCoroutine(CoroutineStart());
        }
        
        // 游戏开始时的协程，负责显示初始界面和教程
        IEnumerator CoroutineStart()
        {
            
            // 如果是第一次进入游戏
            if (m_MainSaveData.m_CheckpointNumber == 0)
            {
                // 显示游戏开始的提示文本UI
                m_TextUI_1.SetActive(true);
                // 开始淡入效果
                FadeController.m_Current.StartFadeIn();
                yield return new WaitForSeconds(0.5f);
                // 开始淡出效果
                FadeController.m_Current.StartFadeOut();
                yield return new WaitForSeconds(0.5f);
                m_TextUI_1.SetActive(false);
                FadeController.m_Current.StartFadeIn();

                // 等待并显示第一个教程
                yield return new WaitForSeconds(1f);
                m_Tutorials[0].SetActive(true);
                yield return new WaitForSeconds(1f);
                m_Tutorials[0].SetActive(false);

                // 等待并显示第二个教程
                m_Tutorials[1].SetActive(true);
                yield return new WaitForSeconds(1f);
                m_Tutorials[1].SetActive(false);
            }
            else
            {
                // 如果玩家不是第一次进入游戏，则只进行淡入效果
                FadeController.m_Current.StartFadeIn();
                yield return new WaitForSeconds(0.5f);
            }
        }

        // 每帧更新
        void Update()
        {
            // 如果按下了Escape键，切换暂停状态
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (!m_Pausesd)
                {
                    m_Pausesd = true;
                    Time.timeScale = 0; // 暂停游戏时间
                    m_PauseUI.SetActive(true); // 显示暂停UI
                }
                else
                {
                    m_Pausesd = false;
                    Time.timeScale = 1; // 恢复游戏时间
                    m_PauseUI.SetActive(false); // 隐藏暂停UI
                }
            }
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
            StartCoroutine(CoroutineHandleGameOver());
        }
        
        // 处理游戏结束的协程
        IEnumerator CoroutineHandleGameOver()
        {
            // 摄像机震动效果
            CameraController.m_Current.StartShake(.4f, .3f);
            yield return new WaitForSeconds(1);
            // 开始淡出效果
            FadeController.m_Current.StartFadeOut();
            yield return new WaitForSeconds(2);
            // 加载当前场景，实现重生
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        
        // 恢复游戏
        public void Resume()
        {
            m_Pausesd = false;
            Time.timeScale = 1; // 恢复游戏时间
            m_PauseUI.SetActive(false); // 隐藏暂停UI
        }

        // 退出到主菜单
        public void Exit()
        {
            m_Pausesd = false;
            Time.timeScale = 1; // 恢复游戏时间
            SceneManager.LoadScene("MainMenu"); // 加载主菜单场景
        }
    }
}