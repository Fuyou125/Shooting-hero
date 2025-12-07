using System.Collections;
using System.Collections.Generic;
using Shooting.Gameplay;
using UnityEngine;
using UnityEngine.UI;
using Shooting.GamePlay;

namespace Shooting.UI
{
    // HUD类负责显示游戏中的HUD界面，包括玩家血量、武器信息、Boss血量等
    public class HUD : MonoBehaviour
    {
        // UI 元素：显示玩家收集的宝石数量的文本
        public Text m_GemCountText;

        // UI 元素：显示当前武器名称的文本
        public Text m_GunNameText;

        // UI 元素：显示目标的瞄准图标
        public Image m_AimTargetImage;

        // UI 元素：主Canvas（用于调整UI布局）
        public RectTransform m_MainCanvas;

        // UI 元素：显示武器的能量条
        public Image m_WeaponPowerTime;

        // UI 元素：显示玩家的生命值条
        public Image m_PlayerHealth;

        [Space]
        // UI 元素：Boss血量基础条
        public Image m_BossHealthBase;

        // UI 元素：Boss血量条
        public Image m_BossHealth;
        
        [Space]
        // UI 元素：显示技能能量条和技能名称
        public Image m_SkillBase;
        public Image m_SkillBar;
        public Text m_SkillNameText;
        public Text m_SkillAmountText;
        
        // 武器名称数组
        public string[] m_WeaponNames = new string[4] { "手枪", "散弹枪", "机枪", "离子炮" };

        // 静态变量：HUD的实例
        public static HUD m_Main;

        // 在Awake方法中初始化HUD实例
        void Awake()
        {
            m_Main = this;
        }

        void Start()
        {
            // Cursor.visible = false; // 隐藏鼠标光标
            m_BossHealthBase.gameObject.SetActive(false); // 隐藏Boss血量条
        }

        // Update is called once per frame
        void Update()
        {
            // 更新玩家的宝石数量显示
            GemCountDisplay();

            // 处理瞄准图标位置
            HandleTempTarget();

            // UI显示
            DisplayUI();
            
        }
        /// <summary>
        /// 显示宝石数量
        /// </summary>
        private void GemCountDisplay()
        {
            m_GemCountText.text = PlayerController.MainPlayerController.m_GemCount.ToString();
        }
        
        /// <summary>
        /// 处理瞄准目标
        /// </summary>
        private void HandleTempTarget()
        {
            // 如果目标存在，更新瞄准图标的位置
            if (PlayerCharacter.m_Current.m_TempTarget != null)
            {
                m_AimTargetImage.gameObject.SetActive(true);

                // 计算目标在屏幕上的位置
                Vector3 v = CameraController.m_Current.m_MyCamera.WorldToScreenPoint(PlayerCharacter.m_Current.m_TempTarget.m_TargetCenter.position);
                v.x = v.x / (float)Screen.width;
                v.y = v.y / (float)Screen.height;

                // 将屏幕坐标转换为UI坐标
                v.x = m_MainCanvas.sizeDelta.x * v.x;
                v.y = m_MainCanvas.sizeDelta.y * v.y;

                // 设置瞄准图标的位置
                m_AimTargetImage.rectTransform.anchoredPosition = Helpers.ToVector2(v);
            }
            else
            {
                m_AimTargetImage.gameObject.SetActive(false); // 如果没有目标，则隐藏瞄准图标
            }
        }
        
        /// <summary>
        /// 显示人物相关UI
        /// </summary>
        private void DisplayUI()
        {
            // 更新武器能量条显示
            m_WeaponPowerTime.fillAmount = PlayerCharacter.m_Current.m_WpnPowerTime / 16f;

            // 更新玩家血量显示
            DamageController damage = PlayerCharacter.m_Current.GetComponent<DamageController>();
            m_PlayerHealth.fillAmount = damage.Damage / damage.MaxDamage;

            // 更新当前武器名称显示
            m_GunNameText.text = m_WeaponNames[PlayerCharacter.m_Current.m_WeaponNum];
            
            // 如果当前有Boss，更新Boss血量显示
            if (GameController.m_Current.m_LevelBoss != null)
            {
                damage = GameController.m_Current.m_LevelBoss.GetComponent<DamageController>();
                m_BossHealth.fillAmount = damage.Damage / damage.MaxDamage;
            }
            
            // 更新玩家技能显示
            PlayerSkills p = PlayerCharacter.m_Current.GetComponent<PlayerSkills>();
            if (p.m_HaveSkill)
            {
                m_SkillBase.gameObject.SetActive(true); // 显示技能栏

                // 根据不同技能类型更新UI显示
                if (p.m_SkillID == 0)
                {
                    m_SkillNameText.text = "手榴弹"; // 投掷物
                    m_SkillAmountText.gameObject.SetActive(true); // 显示数量
                    m_SkillAmountText.text = p.m_AmmoCount.ToString();
                    m_SkillBar.gameObject.SetActive(false); // 不显示能量条
                }
            }
            else
            {
                m_SkillBase.gameObject.SetActive(false); // 如果没有技能，则隐藏技能栏
            }
        }
        
        // 显示Boss的血量条
        public void ShowBossHealth()
        {
            m_BossHealthBase.gameObject.SetActive(true);
        }
    }
}

