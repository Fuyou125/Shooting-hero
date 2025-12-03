using System;
using UnityEngine;

namespace Shooting
{
    public class WeaponBase : MonoBehaviour
    {
        // 武器ID（用于区分不同的武器）
        public int WpnID = 0;

        // 武器名称（例如 "SMG 1"）
        public string Title = "SMG 1";

        // 是否自动开火（自动开火时，持续按住开火键可不断射击）
        public bool AutoFire = true;

        // 开火延迟（每次开火之间的时间间隔）
        public float FireDelay = 0.2f;

        // 连发延迟（多次射击时每次发射之间的延迟时间）
        public float BurstDelay = .5f;

        // 连发次数（每次连发多少次子弹）
        public int BurstFireCount = 3;

        // 后坐力速度（控制后坐力恢复的速度）
        public float RecoilSpeed = 5;
        
        [HideInInspector]
        // 是否按住开火键
        public bool Input_FireHold = false;

        private void Update()
        {
            
        }
    }
}