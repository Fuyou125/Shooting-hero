using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Shooter
{
    public class ForwardGizmo : MonoBehaviour
    {
        void OnDrawGizmos()
        {
            // 绘制一条线，表示物体的前进方向（从物体的位置到物体前方 2 个单位的地方）
            Gizmos.DrawLine(transform.position, transform.position + 2 * transform.forward);
        }
    }
}