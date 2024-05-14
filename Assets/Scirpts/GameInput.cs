using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInput : MonoBehaviour {

    public event EventHandler OnInteractAction; // Ư�� ��ȣ�ۿ��� ���� �� �߻��ϴ� �̺�Ʈ
   
    private PlayerInputActions playerInputActions;  // �÷��̾� �Է� ó��

    private void Awake() {
        playerInputActions = new PlayerInputActions();                      // �ν��Ͻ��� �����Ͽ� �پ��� �Է��� ����
        playerInputActions.Player.Enable();                                 // �Է� �׼� Ȱ��ȭ
        playerInputActions.Player.Interact.performed += Interact_performed; // ��ȣ�ۿ� Ű�� ������ �� �޼��尡 ȣ��ǵ��� ����
    }

    // ��ȣ�ۿ� Ű�� ������ ����
    private void Interact_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        // OnInteractAction�� null�� �ƴϸ� �̺�Ʈ �߻�, ����� �޼��� ȣ��
        // OnInteractAction�� ����� �޼���� Player Ŭ������ GameInput_OnInteractAction
        OnInteractAction?.Invoke(this, EventArgs.Empty); 
    }

    public Vector2 GetMovementVectorNormalized() {

        Vector2 inputVector = playerInputActions.Player.Move.ReadValue<Vector2>();

        inputVector = inputVector.normalized;

        return inputVector;
    }
}
