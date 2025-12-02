using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shooter
{
    // Helper类，提供一些常用的数学和向量操作的辅助方法
    public class Helper : MonoBehaviour
    {
        // 将三维向量(Vector3)转换为二维向量(Vector2)，忽略z轴
        public static Vector2 ToVector2(Vector3 v)
        {
            return (new Vector2(v.x, v.y));  // 只保留x和y值，z轴丢弃
        }
        
        // 根据角度和长度计算旋转后的单位向量
        // 返回一个在水平方向上旋转指定角度的向量，长度为指定的`lenght`
        public static Vector3 RotatedLenght(float angle, float lenght)
        {
            return (Quaternion.Euler(0, angle, 0) * (lenght * Vector3.forward)); 
            // 使用Quaternion旋转，Vector3.forward代表(0,0,1)，然后乘以长度`lenght`
        }

        // 根据给定的角度旋转一个向量
        public static Vector3 RotatedVector(float angle, Vector3 vector)
        {
            return (Quaternion.Euler(0, angle, 0) * (vector)); 
            // 使用Quaternion旋转指定的`vector`，返回旋转后的向量
        }
    }
}