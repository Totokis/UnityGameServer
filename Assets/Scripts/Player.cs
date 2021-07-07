using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int Id;
    public string Username;
    

    private float _moveSpeed = 5f / Constants.TICKS_PER_SEC;
    private bool[] _inputs;
    public void FixedUpdate()
    {
        Vector2 inputDirection = Vector2.zero;
        if (_inputs[0])
            inputDirection.y += 1;
        if (_inputs[1])
            inputDirection.y -= 1;
        if (_inputs[2])
            inputDirection.x -= 1;
        if (_inputs[3])
            inputDirection.x += 1;

        Move(inputDirection);
    }
    private void Move(Vector2 inputDirection)
    {
        
        Vector3 moveDirection = transform.right * inputDirection.x + transform.forward * inputDirection.y;
        transform.position += moveDirection * _moveSpeed;

        ServerSend.PlayerPosition(this);
        ServerSend.PlayerRotation(this);

    }


    public void SetInput(bool[] inputs, Quaternion rotation)
    {
        this._inputs = inputs;
        transform.rotation  = rotation;
    }

    public void Initialize(int id, string playerName)
    {
        Id = id;
        Username = playerName;
        _inputs = new bool[4];
    }
}
