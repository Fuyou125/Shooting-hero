using System.Collections;
using System.Collections.Generic;
using Shooter.Gameplay;
using Shooting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [HideInInspector]
    public PlayerCharacter MyPlayerChar;  // 玩家角色
    
    // 玩家重生点
    public Transform m_SpawnPoint;
    
    // 玩家预设体
    public GameObject PlayerPrefab1;
    
    // 玩家是否死亡
    public bool m_IsDead = false;

    
    // 静态变量，存储主玩家控制器
    public static PlayerController MainPlayerController;
    
    [HideInInspector]
    public Vector3 m_Input_Movement;  // 玩家移动输入
    [HideInInspector]
    public Vector3 AimPosition;  // 玩家瞄准位置
    [HideInInspector]
    public bool Input_FireHold = false;  // 持续开火输入
    
    [Space]
    public Transform m_AimPointTransform;  // 用于表示瞄准点的Transform

    // 游戏开始时初始化玩家控制器
    void Awake()
    {
        MainPlayerController = this;  // 设置主玩家控制器
    }
    
    // 游戏开始时进行设置
    void Start()
    {
        Respawn();  // 进行玩家重生
    }

    void Update()
    {
        UpdateInputs();  // 更新玩家输入
    }

    private void UpdateInputs()
    {
        m_Input_Movement = Vector3.zero;  // 初始化移动输入为零
        
        Input_FireHold = false;  // 初始化持续开火输入
        
        // 获取相机的前方向，并忽略Y轴，规范化为单位向量
        Vector3 cameraForward = CameraController.m_Current.transform.forward;
        cameraForward.y = 0;
        cameraForward.Normalize();
        //获取相机的右方向
        Vector3 cameraRight = Helpers.RotatedVector(90, cameraForward);
        
        // 检查玩家是否按下了移动控制键
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
        {
            m_Input_Movement += cameraForward;  // 向前移动
        }
        else if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
        {
            m_Input_Movement -= cameraForward;  // 向后移动
        }

        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            m_Input_Movement -= cameraRight;  // 向左移动
        }
        else if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            m_Input_Movement += cameraRight;  // 向右移动
        }
        
        // 检查是否按下开火按钮（Z或K键）
        if (Input.GetKey(KeyCode.Z) || Input.GetKey(KeyCode.K))
        {
            Input_FireHold = true;  // 持续开火
        }
        
        // 获取鼠标的位置并计算瞄准点
        Ray ray = CameraControl.m_Current.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
        float dis = 0;
        new Plane(Vector3.up, Vector3.zero).Raycast(ray, out dis);  // 计算射线与平面的交点
        AimPosition = ray.origin + dis * ray.direction;  // 计算瞄准位置
        m_AimPointTransform.position = AimPosition;  // 更新瞄准点的位置
    }
    
    // 玩家重生函数
    public void Respawn()
    {
        // 创建新的玩家角色
        GameObject obj = Instantiate(PlayerPrefab1);
        MyPlayerChar = obj.GetComponent<PlayerCharacter>();  // 获取玩家角色组件

        // 判断游戏存档的检查点，根据检查点设置玩家重生位置
        if (GameController.m_Current.m_MainSaveData.m_CheckpointNumber == 0)
        {
            MyPlayerChar.transform.position = m_SpawnPoint.position + new Vector3(0, .1f, 0);  // 初始重生点
        }
        else
        {
            int num = GameController.m_Current.m_MainSaveData.m_CheckpointNumber - 1;  // 根据存档检查点的编号来设置重生位置
            MyPlayerChar.transform.position = CheckpointController.m_Main.m_Checkpoints[num].m_SpawnPoint.position;  // 设置玩家位置
        }
    }
}
