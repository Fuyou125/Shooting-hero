using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Shooting.GamePlay
{
    public class End : MonoBehaviour
    {
        public void BtnExit()
        {
            Time.timeScale = 1;
            SceneManager.LoadScene("Menu");
        }
    }

}
