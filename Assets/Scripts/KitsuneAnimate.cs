using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KitsuneAnimate : MonoBehaviour
{
    public Transform headBone;
    Transform parent;

    void Start()
    {
        parent = transform.parent;
    }

    void LateUpdate()
    {
        transform.localRotation = Quaternion.Euler(transform.localRotation.x, -parent.transform.localRotation.y, transform.localRotation.z);
        //headBone.localRotation = Quaternion.Euler(transform.localRotation.x, parent.transform.localRotation.y * 180 + 90f, transform.localRotation.z);
    }
}
