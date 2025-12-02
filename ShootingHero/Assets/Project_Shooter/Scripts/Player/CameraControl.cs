using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shooter.Gameplay
{
    public class CameraControl : MonoBehaviour
    {
        private float m_ShakeTimer;  // 摄像机晃动的持续时间计时器
        private float m_ShakeArc;    // 摄像机晃动的角度（目前没有使用）
        private float m_ShakeRadius = 1;  // 摄像机晃动的幅度

        public float m_MinZ = 0;  // 摄像机的最小Z轴位置

        [HideInInspector]
        public Transform m_Target;  // 摄像机的目标（一般是玩家）
        
        [HideInInspector]
        public Vector3 m_TargetPoint;  // 目标点（例如摄像机的视角焦点）
        
        [SerializeField]
        private Transform m_TargetPointTransform;  // 目标点的 Transform，通常用于跟随玩家

        public static CameraControl m_Current;  // 当前的 CameraControl 实例

        public Camera m_MyCamera;  // 当前摄像机对象

        public Transform m_BossTarget;  // 如果有Boss，摄像机会跟随 Boss 进行调整

        public Transform m_BackBlock;  // 后景物体（可能是为了实现遮挡效果）

        [HideInInspector]
        public Vector3 m_CameraBottomPosition;  // 摄像机底部位置
        [HideInInspector]
        public Vector3 m_CameraTopPosition;  // 摄像机顶部位置

        Vector3 Direction;  // 摄像机的前进方向

        // Awake 在游戏开始时调用一次，通常用于初始化静态变量
        void Awake()
        {
            m_Current = this;  // 设置当前的 CameraControl 实例
        }

        // Start 在第一次更新前调用一次
        void Start()
        {
            Direction = transform.forward;  // 初始的摄像机前进方向
            m_MyCamera = GetComponent<Camera>();  // 获取摄像机组件
            m_MinZ = PlayerChar.m_Current.transform.position.z + 10;  // 设置最小Z轴位置为玩家当前位置的Z轴+10
            m_CameraBottomPosition = new Vector3(0, 0, -100);  // 初始底部位置
            m_CameraTopPosition = new Vector3(0, 0, -100);  // 初始顶部位置

            float distance = 80;  // 摄像机与目标之间的距离
            Direction = Quaternion.Euler(40, 0, 0) * Vector3.forward;  // 摄像机的初始旋转方向
            Vector3 targetPosition = PlayerChar.m_Current.transform.position;  // 目标位置，默认是玩家的位置
            targetPosition.z = m_MinZ;  // 设置目标的最小Z轴位置
            targetPosition.x = 0.4f * targetPosition.x;  // 根据某种比例调整目标位置的X轴
            transform.position = targetPosition + -distance * Direction;  // 设置摄像机的位置
            transform.forward = Direction;  // 设置摄像机的朝向
        }

        // Update 每帧调用一次
        void Update()
        {
            m_ShakeTimer -= Time.deltaTime;  // 每帧减少震动计时器的时间

            if (m_ShakeTimer <= 0)
                m_ShakeTimer = 0;  // 如果震动时间已经结束，则停止震动

            Vector3 ShakeOffset = Vector3.zero;  // 震动偏移量初始化
            float shakeSin = Mathf.Cos(30 * Time.time) * Mathf.Clamp(m_ShakeTimer, 0, 0.5f);  // 基于正弦波计算震动的X轴偏移
            float shakeCos = Mathf.Sin(50 * Time.time) * Mathf.Clamp(m_ShakeTimer, 0, 0.5f);  // 基于余弦波计算震动的Y轴偏移
            ShakeOffset = new Vector3(m_ShakeRadius * shakeCos, m_ShakeRadius * shakeSin, 0);  // 计算总的震动偏移量

            // 更新最小Z轴位置（玩家的Z轴位置+8）
            if (PlayerControl.MainPlayerController.MyPlayerChar.transform.position.z + 8 > m_MinZ)
            {
                m_MinZ = PlayerControl.MainPlayerController.MyPlayerChar.transform.position.z + 8;
            }

            float distance = 80;  // 设置摄像机与目标之间的距离
            Direction = Quaternion.Euler(40, 0, 0) * Vector3.forward;  // 摄像机的前进方向
            Vector3 targetPosition = PlayerChar.m_Current.transform.position;  // 默认目标位置是玩家位置
            targetPosition.z = m_MinZ;  // 设置目标位置的Z轴位置

            if (m_BossTarget != null)
            {
                // 如果有Boss目标，计算目标位置是玩家位置与Boss位置的平均
                targetPosition = PlayerChar.m_Current.transform.position + m_BossTarget.position;
                targetPosition = 0.5f * targetPosition;  // 目标位置是玩家与Boss位置的中点
                targetPosition.x = 0.6f * targetPosition.x;  // 调整X轴位置
            }

            // 使用平滑插值让摄像机平滑地跟随目标
            transform.position = Vector3.Lerp(transform.position, targetPosition + -distance * Direction, 5 * Time.deltaTime);
            transform.position += ShakeOffset;  // 加上震动偏移量
            transform.forward = Vector3.Lerp(transform.forward, Direction, 5 * Time.deltaTime);  // 平滑过渡摄像机朝向

            // 更新底部和顶部位置以及背景物体的位置
            float range = 200;
            Ray ray = GetComponent<Camera>().ScreenPointToRay(new Vector3(0.5f * Screen.width, 0, 0));  // 从屏幕中心发射射线
            float dis = 0;
            new Plane(Vector3.up, Vector3.zero).Raycast(ray, out dis);  // 计算射线与平面的交点
            m_CameraBottomPosition = ray.origin + dis * ray.direction;  // 更新摄像机底部位置
            m_BackBlock.position = m_CameraBottomPosition;  // 更新背景物体的位置

            ray = GetComponent<Camera>().ScreenPointToRay(new Vector3(0.5f * Screen.width, Screen.height, 0));  // 从屏幕底部发射射线
            dis = 0;
            new Plane(Vector3.up, Vector3.zero).Raycast(ray, out dis);  // 计算射线与平面的交点
            m_CameraTopPosition = ray.origin + dis * ray.direction;  // 更新摄像机顶部位置

            m_TargetPointTransform.position = m_CameraTopPosition;  // 设置目标点位置
        }

        // 开始摄像机震动
        public void StartShake(float t, float r)
        {
            if (m_ShakeTimer == 0 || m_ShakeRadius < r)  // 如果当前没有震动或震动幅度较大
                m_ShakeRadius = r;  // 更新震动幅度

            m_ShakeTimer = t;  // 设置震动的持续时间
        }
    }
}
