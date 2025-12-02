using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Shooter.Gameplay;

namespace Shooter.UI
{
    // UI_HUD类负责显示游戏中的HUD界面，包括玩家血量、武器信息、Boss血量等
    public class UI_HUD : MonoBehaviour
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
        public Image m_PowerBase;
        public Image m_PowerBar;
        public Text m_PowerNameText;
        public Text m_PowerAmountText;

        // 武器名称数组
        public string[] m_WeaponNames = new string[4] { "手枪", "散弹枪", "机枪", "离子炮" };

        // 静态变量：UI_HUD的实例
        public static UI_HUD m_Main;

        // 在Awake方法中初始化UI_HUD实例
        void Awake()
        {
            m_Main = this;
        }

        // 在游戏开始时初始化UI
        void Start()
        {
            // Cursor.visible = false; // 隐藏鼠标光标
            m_BossHealthBase.gameObject.SetActive(false); // 隐藏Boss血量条
        }

        // 每帧更新UI
        void Update()
        {
            // 更新玩家的宝石数量显示
            m_GemCountText.text = PlayerControl.MainPlayerController.m_GemCount.ToString();

            // 如果目标存在，更新瞄准图标的位置
            if (PlayerChar.m_Current.m_TempTarget != null)
            {
                m_AimTargetImage.gameObject.SetActive(true);

                // 计算目标在屏幕上的位置
                Vector3 v = CameraControl.m_Current.m_MyCamera.WorldToScreenPoint(PlayerChar.m_Current.m_TempTarget.m_TargetCenter.position);
                v.x = v.x / (float)Screen.width;
                v.y = v.y / (float)Screen.height;

                // 将屏幕坐标转换为UI坐标
                v.x = m_MainCanvas.sizeDelta.x * v.x;
                v.y = m_MainCanvas.sizeDelta.y * v.y;

                // 设置瞄准图标的位置
                m_AimTargetImage.rectTransform.anchoredPosition = Helper.ToVector2(v);
            }
            else
            {
                m_AimTargetImage.gameObject.SetActive(false); // 如果没有目标，则隐藏瞄准图标
            }

            // 更新武器能量条显示
            m_WeaponPowerTime.fillAmount = PlayerChar.m_Current.m_WpnPowerTime / 16f;

            // 更新玩家血量显示
            DamageControl damage = PlayerChar.m_Current.GetComponent<DamageControl>();
            m_PlayerHealth.fillAmount = damage.Damage / damage.MaxDamage;

            // 更新当前武器名称显示
            m_GunNameText.text = m_WeaponNames[PlayerChar.m_Current.m_WeaponNum];

            // 如果当前有Boss，更新Boss血量显示
            if (GameControl.m_Current.m_LevelBoss != null)
            {
                damage = GameControl.m_Current.m_LevelBoss.GetComponent<DamageControl>();
                m_BossHealth.fillAmount = damage.Damage / damage.MaxDamage;
            }

            // 更新玩家技能显示
            PlayerPowers p = PlayerChar.m_Current.GetComponent<PlayerPowers>();
            if (p.m_HavePower)
            {
                m_PowerBase.gameObject.SetActive(true); // 显示技能栏

                // 根据不同技能类型更新UI显示
                if (p.m_PowerNum == 0)
                {
                    m_PowerNameText.text = "手榴弹"; // 投掷物
                    m_PowerAmountText.gameObject.SetActive(true); // 显示数量
                    m_PowerAmountText.text = p.m_AmmoCount.ToString();
                    m_PowerBar.gameObject.SetActive(false); // 不显示能量条
                }
            }
            else
            {
                m_PowerBase.gameObject.SetActive(false); // 如果没有技能，则隐藏技能栏
            }
        }

        // 显示Boss的血量条
        public void ShowBossHealth()
        {
            m_BossHealthBase.gameObject.SetActive(true);
        }
    }
}
