using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInput : MonoBehaviour {

    public event EventHandler OnInteractAction; // 특정 상호작용을 했을 때 발생하는 이벤트
   
    private PlayerInputActions playerInputActions;  // 플레이어 입력 처리

    private void Awake() {
        playerInputActions = new PlayerInputActions();                      // 인스턴스를 생성하여 다양한 입력을 설정
        playerInputActions.Player.Enable();                                 // 입력 액션 활성화
        playerInputActions.Player.Interact.performed += Interact_performed; // 상호작용 키를 누르면 이 메서드가 호출되도록 설정
    }

    // 상호작용 키를 누르면 실행
    private void Interact_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        // OnInteractAction이 null이 아니면 이벤트 발생, 연결된 메서드 호출
        // OnInteractAction에 연결된 메서드는 Player 클래스의 GameInput_OnInteractAction
        OnInteractAction?.Invoke(this, EventArgs.Empty); 
    }

    public Vector2 GetMovementVectorNormalized() {

        Vector2 inputVector = playerInputActions.Player.Move.ReadValue<Vector2>();

        inputVector = inputVector.normalized;

        return inputVector;
    }
}
