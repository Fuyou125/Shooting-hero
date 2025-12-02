using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shooter
{
    // 该类用于实现物体的旋转
    public class Spin : MonoBehaviour
    {
        // 定义物体旋转的速度，使用 Vector3 来表示各轴的旋转速率
        public Vector3 m_Speed = Vector3.zero;
        
        // 该方法用于实时更新物体的旋转
        void Update()
        {
            // 通过将物体当前的旋转乘以一个新的旋转值来实现旋转
            transform.localRotation *= Quaternion.Euler(Time.deltaTime * m_Speed);
        }
    }
}