using UnityEngine;

namespace Shooting.UI
{
    public class CanvasSize : MonoBehaviour
    {
        void Awake()
        {
            RectTransform r = gameObject.GetComponent<RectTransform>();
            float ratio = (float)Screen.width / (float)Screen.height;
            r.sizeDelta = new Vector2(ratio * 800, 800);
        }
    }
}