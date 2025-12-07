using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GizmosDisplay : MonoBehaviour
{

    public static float currentGroundY;
    public static float forwardGroundY;
    public  float currentY;
    public float forwardY;
    public static float radius;
    public float radiusSet;
    public static float heightDiff;
    public float height;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        currentY = currentGroundY;
        forwardY = forwardGroundY;
        // radius = radiusSet;
        height = heightDiff;
    }
}
