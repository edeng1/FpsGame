using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundCheck : MonoBehaviour
{
    PlayerController playerController;
    LayerMask groundLayer;
    private void Awake()
    {
        groundLayer = LayerMask.GetMask("Ground");
        playerController = GetComponentInParent<PlayerController>();
    }

    private void Update()
    {
        if (Physics.CheckSphere(transform.position, .4f, groundLayer))
        {
            playerController.SetGroundedState(true);
        }
        else
            playerController.SetGroundedState(false);
    }
    /*
    private void OnTriggerEnter(Collider other)
    {
        
        if (other.gameObject == playerController.gameObject)
            return;
        if(other.gameObject.layer==groundLayer)
            playerController.SetGroundedState(true);
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == playerController.gameObject)
            return;
        if (other.gameObject.layer == groundLayer)
            playerController.SetGroundedState(false);
    }

    private void OnTriggerStay(Collider other)
    {
        
        if (other.gameObject == playerController.gameObject)
            return;
        if (other.gameObject.layer == groundLayer)
            playerController.SetGroundedState(true);
    }

   

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == playerController.gameObject)
            return;
        playerController.SetGroundedState(true);
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject == playerController.gameObject)
            return;
        playerController.SetGroundedState(false);
    }
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject == playerController.gameObject)
            return;
        playerController.SetGroundedState(true);
    }
    */
}
