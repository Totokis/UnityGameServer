using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class Player : MonoBehaviour
{
    [SerializeField] Transform shootOrigin;
    [SerializeField] CharacterController characterController;
    [SerializeField] float gravity = -9.81f;
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float jumpSpeed = 5f;
    [SerializeField] float health;
    [SerializeField] float maxHealth = 100f;
    [SerializeField] int id;
    [SerializeField] string username;
    
    public int Id => id;
    public string Username => username;

    public float Health => health;

    bool[] _inputs;
    float _yVelocity = 0f;

    private void Start()
    {
        gravity *= Time.fixedDeltaTime * Time.fixedDeltaTime;
        moveSpeed *= Time.fixedDeltaTime;
        jumpSpeed *= Time.fixedDeltaTime;
    }
    public void FixedUpdate()
    {
        if (health <= 0f)
        {
            return;
        }
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
        moveDirection *= moveSpeed;

        if (characterController.isGrounded)
        {
            _yVelocity = 0f;
            if (_inputs[4])
            {
                _yVelocity = jumpSpeed;
            }
            
        }
        _yVelocity += gravity;

        moveDirection.y = _yVelocity;
        characterController.Move(moveDirection);
        
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
        this.id = id;
        this.username = playerName;
        _inputs = new bool[5];
        health = maxHealth;
    }

    public void Shoot(Vector3 viewDirection)
    {
        Debug.DrawRay(shootOrigin.position,viewDirection*25f, Color.red,5f);
        if (Physics.Raycast(shootOrigin.position, viewDirection, out RaycastHit hit, 25f))
        {
            Debug.Log($"A {hit.collider.name} has been hit");
            if (hit.collider.GetComponent<Player>()!=null)
            {
                Debug.Log("a player has been shot");
                hit.collider.GetComponent<Player>().TakeDamage(50f);
            }
        }
        
    }

    public void TakeDamage(float damage)
    {
        if (health <= 0)
        {
            return;
        }

        health -= damage;
        if (health <= 0f)
        {
            health = 0f;
            characterController.enabled = false;
            transform.position = new Vector3(0f, 25f, 0f);
            ServerSend.PlayerPosition(this);
            StartCoroutine(Respawn());
        }
        
        ServerSend.PlayerHealth(this);
    }

    IEnumerator Respawn()
    {
        yield return new WaitForSeconds(5f);

        health = maxHealth;
        characterController.enabled = true;
        ServerSend.PlayerRespawned(this);
    }
}
