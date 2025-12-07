using UnityEngine;

namespace Shooting.Gameplay
{
    public class EnemyBase : MonoBehaviour
    {
        // 伤害控制组件
        protected DamageController m_DamageControl;
        // 死亡粒子效果
        [SerializeField]
        protected GameObject m_DeathParticlePrefab;
        // 出现时的粒子效果
        [SerializeField]
        protected GameObject m_SpawnParticlePrefab;
        
        // 移动方向
        [HideInInspector]
        public Vector3 MoveDirection;
        // 初始位置
        [HideInInspector]
        public Vector3 InitPosition;
        
        // 摇晃效果的基础
        public Transform ShakeBase;
        // 旋转基础
        public Transform m_RotationBase;
        // 发射点位置
        public Transform m_FirePoint;
        
        // 掉落的物品预制件
        public GameObject[] m_ItemPrefabs;
        
        // 是否面朝玩家
        [HideInInspector]
        public bool m_FacePlayer = false;
        
        // AI相关
        bool m_CanChangeTargetPosition = true;  // 是否可以改变目标位置
        bool m_FrontClear = true;  // 前方是否清除
        
        bool m_CanDamage = true;  // 是否可以造成伤害
        
        // 是否开始攻击
        public bool m_StartAttack = false;
        // 开始攻击时的距离
        public float m_StartWalkDistance = 10;
        
        // 是否死亡
        [HideInInspector]
        public bool m_IsDead = false;
        
        // 是否被警觉到
        [HideInInspector]
        public bool m_Alerted = false;
        
        // 动画控制器
        public Animator m_Animator;
        
        // 掉落物品的数量
        public int m_ItemDropCount = 1;

        // 游戏开始时初始化
        void Start()
        {
            m_CanDamage = true;  // 允许造成伤害
            m_DamageControl = GetComponent<DamageController>();  // 获取伤害控制组件
            
            InitPosition = transform.position;  // 记录初始位置

            // 出现时播放粒子效果
            GameObject obj = Instantiate(m_SpawnParticlePrefab);
            obj.transform.position = InitPosition;
            Destroy(obj, 3);  // 3秒后销毁该粒子效果
        }
        
        // 每帧更新
        void Update()
        {
            // AI行为：根据敌人当前位置朝向玩家
            Vector3 forward = Vector3.zero - transform.position;
            forward.y = 0;  // 保证敌人只在XZ平面上旋转
            Quaternion rotation = Quaternion.LookRotation(forward);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, 10 * Time.deltaTime);  // 平滑旋转朝向玩家方向

            // 根据伤害的程度控制摇晃效果
            Vector3 axis = Quaternion.Euler(0, 30 * m_DamageControl.DamageShakeAngle, 0) * Vector3.right;
            ShakeBase.transform.localRotation = Quaternion.AngleAxis(-30 * m_DamageControl.DamageShakeAmount, axis);

            HandleDeath();  // 处理敌人的死亡

            CheckAlert();  // 检查敌人是否被警觉到
        }

        // 处理敌人的死亡逻辑
        public virtual void HandleDeath()
        {
            m_DamageControl = GetComponent<DamageController>();  // 重新获取伤害控制组件
            
            // 如果敌人血量为0或更低，执行死亡逻辑
            if (m_DamageControl.Damage <= 0)
            {
                GameObject obj = Instantiate(m_DeathParticlePrefab);
                obj.transform.position = transform.position;
                Destroy(obj, 3);  // 3秒后销毁死亡粒子

                // 掉落物品
                DropItem(m_ItemDropCount);

                // 销毁敌人对象
                Destroy(gameObject);
            }
            
            // 如果敌人超出相机范围，也销毁敌人
            if (transform.position.z < CameraController.m_Current.m_CameraBottomPosition.z - 10)
            {
                Destroy(gameObject);  // 销毁敌人对象
            }
        }
        
        // 让敌人面向玩家
        public virtual void HandleFacePlayer()
        {
            if (m_FacePlayer)  // 如果需要面向玩家
            {
                // 计算从敌人到玩家的方向
                Vector3 dir = PlayerCharacter.m_Current.transform.position - transform.position;
                dir.y = 0;  // 保证只在水平面旋转

                dir.Normalize();  // 归一化方向向量
                m_RotationBase.rotation = Quaternion.Lerp(m_RotationBase.rotation, Quaternion.LookRotation(dir), 10 * Time.deltaTime);  // 平滑旋转
            }
        }

        // 检查敌人是否被警觉到
        public virtual void CheckAlert()
        {
            if (!m_Alerted)  // 如果敌人还未被警觉
            {
                // 如果敌人接近相机的可视范围，则开始警觉
                if (CameraController.m_Current.m_CameraTopPosition.z > transform.position.z - 5f)
                {
                    StartAlert();  // 启动警觉
                }
            }
        }
        
        // 启动敌人的警觉状态
        public virtual void StartAlert()
        {
            m_Alerted = true;  // 设置为警觉状态
        }
        
        // 启用敌人的相关行为（可由继承类覆盖）
        public virtual void EnableEnemy()
        {
            // 可以在子类中覆盖该方法，来定义敌人启用后的行为
        }
        
        // 掉落物品
        public virtual void DropItem(int count)
        {
            for (int i = 0; i < count; i++)
            {
                // 实例化一个物品并设置物理属性
                GameObject obj1 = Instantiate(m_ItemPrefabs[0]);
                obj1.transform.position = transform.position;// 设置物品的初始位置
                obj1.GetComponent<Rigidbody>().velocity =
                    new Vector3(Random.Range(-5, 5), Random.Range(10, 20), Random.Range(-5, 5));// 随机速度
                obj1.GetComponent<Rigidbody>().angularVelocity = 
                    new Vector3(Random.Range(-20, 20), Random.Range(-20, 20), Random.Range(-20, 20));  // 随机角速度
            }
        }
        
        // 绘制Gizmos，用于在编辑器中调试
        void OnDrawGizmos()
        {
            Gizmos.color = Color.red;  // 设置颜色为红色
        }
    }
}