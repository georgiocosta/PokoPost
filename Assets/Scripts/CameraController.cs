using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    public GameObject target;
    public float offset;

    private Vector3 newPos;
    private Vector3 velocity;

    void Start()
    {
        target = GameObject.Find("Player");
        velocity = Vector3.zero;
    }

    void Update()
    {
        newPos = new Vector3(target.transform.position.x, 7.5f, target.transform.position.z - offset);
        transform.position = Vector3.SmoothDamp(transform.position, newPos, ref velocity, 0.1f);
    }

    public void StartShake()
    {
        StartCoroutine("ShakeCamera");
    }

    public IEnumerator ShakeCamera()
    {
        float timePassed = 0f;

        while (timePassed < 0.05f)
        {
            float x = Random.Range(-0.1f, 0.1f);
            float y = Random.Range(-0.1f, 0.1f);

            transform.position = new Vector3(transform.position.x + x, transform.position.y + y, transform.position.z);
            timePassed += Time.deltaTime;
            yield return 0;
        }
    }
}