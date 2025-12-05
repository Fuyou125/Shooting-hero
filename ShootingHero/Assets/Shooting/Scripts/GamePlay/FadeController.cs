using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Shooting.Gameplay
{
    // 负责控制淡入淡出的效果
    public class FadeController : MonoBehaviour
    {
        // 静态实例，方便其他类访问当前的 FadeControl 实例
        public static FadeController m_Current;

        // 用于显示淡入淡出效果的 UI 图像
        public Image m_FadeImage;

        // 在对象初始化时调用
        void Awake()
        {
            m_Current = this; // 将当前实例赋值给静态变量 m_Current
        }
        
        void Start()
        {
            // 初始时将淡入淡出图像的游戏对象隐藏
            m_FadeImage.gameObject.SetActive(false);    

            // 启动淡入效果
            StartFadeIn();
        }
        
        // 启动淡出效果
        public void StartFadeOut()
        {
            StartCoroutine(CoroutineStartFadeOut()); // 调用协程执行淡出效果
        }

        // 启动淡入效果
        public void StartFadeIn()
        {
            StartCoroutine(CoroutineStartFadeIn()); // 调用协程执行淡入效果
        }
        
        // 协程：执行淡出效果
        IEnumerator CoroutineStartFadeOut()
        {
            m_FadeImage.gameObject.SetActive(true); // 显示淡入淡出图像
            float fade = 0; // 初始透明度为 0（完全透明）
            
            // 渐变透明度直到完全不透明
            while (fade < 1f)
            {
                fade += .5f * Time.deltaTime; // 每帧增加透明度
                Color c = Color.black; // 设置为黑色
                c.a = fade; // 将透明度应用到颜色的 alpha 通道
                m_FadeImage.color = c; // 更新图像颜色
                yield return null; // 等待下一帧
            }

            m_FadeImage.color = Color.black; // 确保最终颜色为黑色（完全不透明）
        }

        // 协程：执行淡入效果
        IEnumerator CoroutineStartFadeIn()
        {
            m_FadeImage.gameObject.SetActive(true); // 显示淡入淡出图像
            m_FadeImage.color = Color.black; // 初始颜色为黑色
            float fade = 1; // 初始透明度为 1（完全不透明）
            
            // 渐变透明度直到完全透明
            while (fade > 0f)
            {
                fade -= .5f * Time.deltaTime; // 每帧减少透明度
                Color c = Color.black; // 设置为黑色
                c.a = fade; // 将透明度应用到颜色的 alpha 通道
                m_FadeImage.color = c; // 更新图像颜色
                yield return null; // 等待下一帧
            }

            m_FadeImage.gameObject.SetActive(false); // 当淡入完成后隐藏图像
        }
    }
}