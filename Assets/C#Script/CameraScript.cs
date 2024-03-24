using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public float cameraSpeed = 1f;
    public Transform target;

    [SerializeField] float CameraLimitLeft = -2f;
    [SerializeField] float CameraLimitRight = 8.5f;
    [SerializeField] float CameraLimitY = 5f;
    [SerializeField] float CameraLimitZ = -16f;

    void Start()
    {

    }

    void Update()
    {
        Vector3 newPos = new Vector3(Mathf.Clamp(target.position.x, CameraLimitLeft, CameraLimitRight), Mathf.Clamp(target.position.y, CameraLimitY, CameraLimitY), Mathf.Clamp(target.position.z, CameraLimitZ, CameraLimitZ));

        transform.position = Vector3.Slerp(transform.position, newPos, cameraSpeed * Time.deltaTime);

    }
}
