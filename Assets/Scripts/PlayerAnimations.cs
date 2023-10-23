using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerMovementCC;

public class PlayerAnimations : MonoBehaviour
{
    [SerializeField] private PlayerMovementCC player;
    [SerializeField] private Animator animator;
    [SerializeField] private MovementState currentState;


    private void Update()
    {
        currentState = player.GetMovementState();
        if(currentState == MovementState.walking)
        {
            animator.SetBool("IsWalking", true);
        }
        else
        {
            animator.SetBool("IsWalking", false);
        }
        if (currentState == MovementState.sprinting)
        {
            animator.SetBool("IsRunning", true);
        }
        else
        {
            animator.SetBool("IsRunning", false);
        }
        if (currentState == MovementState.crouching)
        {
            animator.SetBool("IsCrouching", true);
        }
        else
        {
            animator.SetBool("IsCrouching", false);
        }
    }

}
