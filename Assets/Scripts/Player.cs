using UnityEngine;

public class Player : MonoBehaviour
{
    public int Id;
    public string Username;

    private readonly float _moveSpeed = 5f / Constants.TICKS_PER_SEC;
    private bool[] _inputs;
    public void FixedUpdate()
    {
        var inputDirection = Vector2.zero;
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

        var moveDirection = transform.right * inputDirection.x + transform.forward * inputDirection.y;
        transform.position += moveDirection * _moveSpeed;

        ServerSend.PlayerPosition(this);
        ServerSend.PlayerRotation(this);

    }


    public void SetInput(bool[] inputs, Quaternion rotation)
    {
        _inputs = inputs;
        transform.rotation = rotation;
    }

    public void Initialize(int id, string playerName)
    {
        Id = id;
        Username = playerName;
        _inputs = new bool[4];
    }
}
