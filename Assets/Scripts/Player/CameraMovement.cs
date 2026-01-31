using System;
using Unity.Mathematics;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] public Transform playerObj;
    [SerializeField] public Transform cameraObj;
    [SerializeField] public float factor;
    [SerializeField] public float distance;
    [SerializeField] public double angle;

    private Vector3 initialPosition;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {   
        cameraObj.position = new Vector3(playerObj.position.x, playerObj.position.y + (float)Math.Sin(angle)*distance, playerObj.position.z - (float)Math.Cos(angle)*distance);
        cameraObj.LookAt(playerObj.position);
        initialPosition = cameraObj.position - playerObj.position;
    }

    // Update is called once per frame
    void Update()
    {
        cameraObj.position = math.lerp(cameraObj.position, playerObj.position+initialPosition, factor*Time.deltaTime);
    }
}
