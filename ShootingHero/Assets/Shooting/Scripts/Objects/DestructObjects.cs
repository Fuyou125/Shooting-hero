using UnityEngine;

namespace Shooting.Gameplay
{
    public class DestructObjects : MonoBehaviour
    {
        [HideInInspector]
        public DamageController MyDamageController;  // 用于控制物体伤害的DamageControl组件
        
        public GameObject BrakeEffectPrefab1;       // 破碎效果预制体
        public GameObject DestroyedObjectPrefab1;   // 物体被摧毁后显示的替代物体预制体
        public Transform m_ExplodeCenter;           // 爆炸中心的位置（用于破碎效果）
        
        Quaternion InitRotation;  // 物体初始旋转角度
        Vector3 InitPosition;  // 物体初始位置
        public float ShakeRadius = 10;  // 抖动的半径（未使用）
        
        // 初始化
        void Start()
        {
            MyDamageController = GetComponent<DamageController>();  // 获取物体的DamageControl组件
            InitRotation = transform.rotation;  // 保存物体的初始旋转角度
            InitPosition = transform.position;  // 保存物体的初始位置
        }

        // 每帧更新
        void Update()
        {
            // 如果物体的DamageControl组件标记为死亡
            if (MyDamageController.IsDead)
            {
                // 实例化破碎效果
                GameObject obj = Instantiate(BrakeEffectPrefab1);
                if (m_ExplodeCenter != null)
                    obj.transform.position = m_ExplodeCenter.position; // 如果指定了爆炸中心，则使用其位置
                else
                    obj.transform.position = transform.position; // 否则使用物体本身的位置
                Destroy(obj, 4); // 4秒后销毁破碎效果

                // 如果有被摧毁后显示的物体预制体，则实例化该物体
                if (DestroyedObjectPrefab1 != null)
                {
                    obj = Instantiate(DestroyedObjectPrefab1);
                    obj.transform.position = transform.position; // 设置为当前物体的位置
                    obj.transform.rotation = transform.rotation; // 设置为当前物体的旋转角度
                }

                // 销毁当前物体
                Destroy(gameObject);
            }
        }
    }
}