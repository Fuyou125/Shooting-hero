using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shooter.Gameplay
{
    // HitGlowControl 控制物体在受到伤害时发光的效果
    public class HitGlowControl : MonoBehaviour
    {
        [HideInInspector]
        public Renderer m_MainRenderer;  // 主渲染器，用于控制物体的材质
        [HideInInspector]
        public Material m_OriginalMat;  // 物体的原始材质
        public Material m_GlowMaterial;  // 物体发光时使用的材质

        // Start is called before the first frame update
        void Start()
        {
            // 获取物体的渲染器组件
            m_MainRenderer = GetComponent<Renderer>();
            // 保存物体的原始材质
            m_OriginalMat = m_MainRenderer.material;
        }

        // 恢复物体的原始材质
        public void SetOriginal()
        {
            m_MainRenderer.material = m_OriginalMat;  // 恢复为原始材质
        }

        // 设置物体为发光材质
        public void SetGlow()
        {
            m_MainRenderer.material = m_GlowMaterial;  // 设置为发光材质
        }
    }
}