using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderController : MonoBehaviour
{
    public static ColliderController m_Current;
    
    [HideInInspector]
    public Collider m_colliders;
    // Start is called before the first frame update
    void Start()
    {
        m_colliders = GetComponent<Collider>(); 
        // m_colliders.isTrigger = true;
        // 动态检测新生成的物体（可选，处理动态场景）
        IgnoreNonPlayerColliders();
    }

    void Update()
    {
        // 动态检测新生成的物体（可选，处理动态场景）
        IgnoreNonPlayerColliders();
    }

    /// <summary>
    /// 忽略所有非Player物体的碰撞
    /// </summary>
    private void IgnoreNonPlayerColliders()
    {
        Collider[] allColliders = FindObjectsOfType<Collider>();
        foreach (Collider col in allColliders)
        {
            if (col.gameObject != gameObject && !col.gameObject.CompareTag("Player"))
            {
                Physics.IgnoreCollision(m_colliders, col, true);
            }
        }
    }
    
    // void OnTriggerEnter(Collider other)
    // {
    //     if (other.gameObject.CompareTag("Player"))
    //     {
    //         Debug.Log("与Player触发碰撞");
    //         // 处理与Player的碰撞逻辑
    //         StartCoroutine(HandleCollider());
    //         
    //     }
    // }
    //
    // IEnumerator HandleCollider()
    // {
    //     m_colliders.isTrigger = false;
    //     yield return new WaitForSeconds(5f);
    // }
    //
    // void OnTriggerExit(Collider other)
    // {
    //     m_colliders.isTrigger = true;
    // }
}
