using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationControl : MonoBehaviour
{
    [SerializeField] private float movespeed;
    [SerializeField] private float walkspeed;
    [SerializeField] private float runspeed;
    private Vector3 moveDirection;

    private CharacterController controller;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();

    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }
    private void Move()
    {
        float moveZ = Input.GetAxis("Vertical");

        moveDirection = new Vector3(0,0,moveZ);
        if(moveDirection != Vector3.zero && !Input.GetKey(KeyCode.LeftShift)) 
        {
            Walk();// walk 
        }
        else if(moveDirection != Vector3.zero && Input.GetKey(KeyCode.LeftShift))
        {
            Run();// run
        }
        else if(moveDirection == Vector3.zero)
        {
            Idle();//Idle
        }
        moveDirection *= movespeed;

        controller.Move(moveDirection*Time.deltaTime);
    }
    private void Idle()
    {
            
    }
    private void Walk()
    {
        movespeed = walkspeed;
    }
    private void Run()
    {
        movespeed = runspeed;
    }
}
