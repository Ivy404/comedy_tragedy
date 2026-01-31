using System;
using System.Data.Common;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerActions : MonoBehaviour
{
    [SerializeField] public Transform PlayerTransform;
    [SerializeField] public float maxSpeed;
    [SerializeField] public Camera mainCamera;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Move(Vector2 direction)
    {   
        if (direction != Vector2.zero)
        {
            Vector3 movedir = new Vector3(direction.x, 0, direction.y);
            PlayerTransform.position += movedir * maxSpeed * Time.deltaTime;
        }
    }
}