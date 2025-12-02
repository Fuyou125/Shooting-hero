using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Shooter.Gameplay
{
    public class InvisibleAtStart : MonoBehaviour
    {
        // Use this for initialization
        void Start()
        {
            GetComponent<Renderer>().enabled = false;
        }
    }
}