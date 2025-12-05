using UnityEngine;


namespace Shooting
{
    /// <summary>
    /// Helper类，提供一些常用的数学和向量操作的辅助方法
    /// </summary>
    public class Helpers : MonoBehaviour
    {
        /// <summary>
        /// 根据给定的角度旋转一个向量
        /// </summary>
        /// <param name="angle">角度</param>
        /// <param name="vector">向量</param>
        /// <returns></returns>
        public static Vector3 RotatedVector(float angle, Vector3 vector)
        {
            return (Quaternion.Euler(0,angle,0) * (vector));
            // 使用Quaternion旋转指定的`vector`，返回旋转后的向量
        }

        /// <summary>
        /// 将三维向量(Vector3)转换为二维向量(Vector2)，忽略z轴
        /// </summary>
        /// <param name="v">输入的三维向量</param>
        /// <returns>二维向量(Vector2)</returns>
        public static Vector3 ToVector2(Vector3 v)
        {
            return (new Vector2(v.x, v.y));
        }
    }
}