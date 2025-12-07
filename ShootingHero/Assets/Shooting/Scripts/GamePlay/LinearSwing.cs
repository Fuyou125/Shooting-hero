using UnityEngine;

namespace Shooting.Scripts.GamePlay
{
    // 该类用于实现物体沿某一轴线的摆动效果
    public class LinearSwing : MonoBehaviour
    {
        // 摆动的速度，控制摆动的快慢
        public float m_Speed = 1;

        // 摆动的半径，控制摆动的幅度
        public float m_Radius = 1;

        // 摆动的轴向，指定摆动沿着哪个轴（如 X、Y、Z）
        public Vector3 m_Axis = Vector3.up;

        // 用于保存物体的初始位置
        [HideInInspector]
        public Vector3 m_InitPosition;

        // Start 方法在游戏开始时调用一次，通常用于初始化
        void Start()
        {
            // 记录物体的初始位置
            m_InitPosition = transform.localPosition;
        }

        // Update 方法在每一帧调用，用于实时更新物体的摆动位置
        void Update()
        {
            // 根据当前时间计算物体的摆动位置
            // 使用正弦函数 Math.Sin 来创建一个周期性摆动效果
            // `m_Radius` 控制摆动的幅度，`m_Speed` 控制摆动的速度，`m_Axis` 控制摆动的轴向
            transform.localPosition = m_InitPosition + m_Radius * Mathf.Sin(m_Speed * Time.time) * m_Axis;
        }
    }
}