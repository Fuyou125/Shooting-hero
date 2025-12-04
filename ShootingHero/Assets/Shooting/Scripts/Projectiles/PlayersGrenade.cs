using UnityEngine;
using System.Collections;

namespace Shooting.Gameplay
{
    public class PlayersGrenade : MonoBehaviour
    {
        // 手榴弹的起始位置
        [HideInInspector]
        public Vector3 m_StartPosition;

        // 手榴弹的目标位置
        [HideInInspector]
        public Vector3 m_TargetPosition;

        // 移动路径的动画曲线，用于控制手榴弹的飞行轨迹
        public AnimationCurve m_MoveCurve;

        // 手榴弹爆炸时产生的粒子效果
        public GameObject m_ExplodeParticle;
        
        // 开始时执行的函数
        void Start()
        {
            // 启动手榴弹的移动协程
            StartCoroutine(CoroutineMove());
        }
        
        // 协程：处理手榴弹的移动
        IEnumerator CoroutineMove()
        {
            float lerp = 0;  // 进度值（从0到1）

            // 持续移动手榴弹，直到其达到目标位置
            while (lerp < 1)
            {
                // 使用线性插值来改变手榴弹的位置
                transform.position = Vector3.Lerp(m_StartPosition, m_TargetPosition, lerp);
                
                // 根据曲线调整手榴弹的垂直高度，使其轨迹更自然
                transform.position += new Vector3(0, m_MoveCurve.Evaluate(lerp), 0);

                // 增加进度，逐步接近目标位置
                lerp += Time.deltaTime;

                // 等待下一帧继续执行
                yield return null;
            }

            // 手榴弹最终到达目标位置
            transform.position = m_TargetPosition;

            // 手榴弹爆炸
            CameraController.m_Current.StartShake(0.3f, 0.15f);  // 摄像机震动效果
            GameObject obj = Instantiate(m_ExplodeParticle);  // 实例化爆炸粒子效果
            obj.transform.position = transform.position;  // 设置粒子效果的位置为手榴弹的当前位置
            Destroy(obj, 3);  // 3秒后销毁粒子效果

            // 销毁手榴弹对象
            Destroy(gameObject);
        }
    }
}