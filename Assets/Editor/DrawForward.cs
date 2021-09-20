using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]
public class DrawForward : MonoBehaviour
{
    void Update()
    {
        Debug.DrawRay(transform.position, transform.forward, Color.red);
    }
}
