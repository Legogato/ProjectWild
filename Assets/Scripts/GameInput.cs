using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class GameInput : MonoBehaviour
{
    public event EventHandler OnJump;
    public event EventHandler OnSprint;
    public event EventHandler OnSprintCanceled;
    public event EventHandler OnCrouch;
    public event EventHandler OnCrouchCanceled;
    public event EventHandler OnSuperJump;
    public event EventHandler OnSuperJumpCanceled;
    public event EventHandler OnDash;
    public event EventHandler OnAirWave;
    PlayerInputActions inputActions;
    private void Awake()
    {
        inputActions = new PlayerInputActions();
        inputActions.Player.Enable();
        inputActions.Player.Jump.performed += JumpPerformed;
        inputActions.Player.Sprint.performed += SprintPerformed;
        inputActions.Player.Sprint.canceled += SprintCanceled; 
        inputActions.Player.Crouch.performed += CrouchPerformed;
        inputActions.Player.Crouch.canceled += CrouchCanceled;
        inputActions.Player.SuperJump.performed += SuperJumpPerformed;
        inputActions.Player.SuperJump.canceled += SuperJumpCanceled;
        inputActions.Player.Dash.performed += DashPerformed;
        inputActions.Player.AirWave.performed += AirWavePerformed;
    }

    private void AirWavePerformed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnAirWave?.Invoke(this, EventArgs.Empty);
    }

    private void DashPerformed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnDash?.Invoke(this, EventArgs.Empty);
    }

    private void SuperJumpCanceled(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnSuperJumpCanceled?.Invoke(this, EventArgs.Empty);
    }

    private void SuperJumpPerformed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnSuperJump?.Invoke(this, EventArgs.Empty);
    }

    private void SprintCanceled(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnSprintCanceled?.Invoke(this, EventArgs.Empty);
    }

    private void CrouchCanceled(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnCrouchCanceled?.Invoke(this, EventArgs.Empty);
    }

    private void CrouchPerformed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnCrouch?.Invoke(this, EventArgs.Empty);
    }

    private void SprintPerformed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnSprint?.Invoke(this, EventArgs.Empty);
    }

    private void JumpPerformed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnJump?.Invoke(this,EventArgs.Empty);

    }

    public Vector2 GetMovementNormalized()
    {
        Vector2 inputVector = inputActions.Player.Move.ReadValue<Vector2>();

        inputVector = inputVector.normalized;
        return inputVector;
    }

    public bool GetJumpPerformed()
    {
        return inputActions.Player.Jump.IsPressed();
    }

    public bool GetSprintPerformed()
    {
        return inputActions.Player.Sprint.IsPressed();
    }

    public bool GetCrouchPerformed()
    {
        return inputActions.Player.Crouch.IsPressed();
    }


}
