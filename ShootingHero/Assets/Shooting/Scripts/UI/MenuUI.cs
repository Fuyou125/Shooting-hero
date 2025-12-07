using UnityEngine;
using UnityEngine.SceneManagement;

namespace Shooting.GamePlay
{
    public class MenuUI : MonoBehaviour
    {
        public SaveDatas m_SaveData;
        // Start is called before the first frame update
        void Start()
        {
            m_SaveData.Load();
        }

        public void BtnStart()
        {
            SceneManager.LoadScene("Level");
        }
    }

}
