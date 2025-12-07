using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Shooting.UI;

namespace Shooting.Gameplay
{
    public class LevelDemo : MonoBehaviour
    {
        public GameObject m_BossObject;  // Boss的游戏对象
        [HideInInspector]
        public bool m_KilledBoss = false;  // 是否击败了Boss 
        public bool isopen = false;
        public GameObject m_BossBlock1;  // 用于显示Boss战斗场景的UI元素
        public GameObject m_BossBlockBack1;  // Boss战斗背景UI元素

        public float totalTargetAngle = 45f;
        public GameObject EndDoor1;
        public GameObject EndDoor2;
        public float rotationTime = 1.0f;

        public EnemySpawn[] m_BossSpawnPoints;  // 小怪物生成点的数组
        
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
                DamageController bossDamage = m_BossObject.GetComponent<DamageController>();
                if (bossDamage.IsDead)
                {
                    m_KilledBoss = true;  // 设置Boss已击败
                    CameraController.m_Current.m_BossTarget = null;  // 移除Boss目标
                    m_BossBlock1.SetActive(false);  // 隐藏Boss战UI元素
                    CameraController.m_Current.m_BackBlock.gameObject.SetActive(true);  // 显示背景UI
                    StartCoroutine(HandleDoorRotate());
                }
            }
        }

        IEnumerator HandleDoorRotate()
        {
            // 记录初始旋转状态
            Quaternion startRotation1 = EndDoor1.transform.rotation;
            Quaternion startRotation2 = EndDoor2.transform.rotation;
            
            float targetAngle1 = isopen ? 0f : -totalTargetAngle; // 左门目标角度
            float targetAngle2 = isopen ? 0f : totalTargetAngle;  // 右门目标角度
            
            Quaternion targetRotation1 = startRotation1 * Quaternion.Euler(0, targetAngle1, 0);
            Quaternion targetRotation2 = startRotation2 * Quaternion.Euler(0, targetAngle2, 0);
            
            float elapsedTime = 0;
            while (elapsedTime < rotationTime)
            {
                // Slerp：球面插值，适合旋转平滑过渡
                EndDoor1.transform.rotation = Quaternion.Slerp(startRotation1, targetRotation1, elapsedTime/rotationTime);
                EndDoor2.transform.rotation = Quaternion.Slerp(startRotation2, targetRotation2, elapsedTime/rotationTime);
                
                elapsedTime += Time.deltaTime;
                yield return null; // 等待下一帧
            }
            // 确保最终旋转准确（避免插值误差）
            EndDoor1.transform.rotation = targetRotation1;
            EndDoor2.transform.rotation = targetRotation2;
            isopen = true;
        }
        
         // 缓动函数（核心优化：让旋转先加速后减速）
        private float ApplyEasing(float t, EasingType type)
        {
            switch (type)
            {
                case EasingType.EaseInOutQuad: // 推荐：最自然的缓动曲线
                    return t < 0.5f ? 2f * t * t : -1f + (4f - 2f * t) * t;
                case EasingType.Linear:
                default:
                    return t;
            }
        }
        
        // 缓动类型枚举
        public enum EasingType
        {
            Linear,
            EaseInOutQuad,
        }

        public void StartBossFight()
        {
            // 开始Boss战斗
            CameraController.m_Current.m_BossTarget = m_BossObject.transform;  // 设置镜头锁定在Boss身上
            m_BossObject.GetComponent<EnemyBase>().EnableEnemy();  // 激活Boss
            m_BossBlockBack1.SetActive(true);  // 显示Boss战斗背景UI
            m_BossBlock1.SetActive(false); 
            HUD.m_Main.ShowBossHealth();  // 显示Boss血条
            CameraController.m_Current.m_BackBlock.gameObject.SetActive(false);  // 隐藏背景UI
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
            GameController.m_Current.m_MainSaveData.m_CheckpointNumber = 0;  // 重置存档的检查点编号
            GameController.m_Current.m_MainSaveData.Save();  // 保存游戏进度
            yield return new WaitForSeconds(1);  // 等待1秒
            FadeController.m_Current.StartFadeOut();  // 开始渐出动画
            yield return new WaitForSeconds(2);  // 等待2秒
            SceneManager.LoadScene("EndScene");  // 加载“EndScene”场景，结束关卡
        }
    }
}