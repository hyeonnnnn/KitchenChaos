using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    public static Player Instance { get; private set; }    // 싱글톤 패턴

    public event EventHandler<OnSelectedCounterChangedEventArgs> OnSelectedCounterChanged;  // 이벤트 선언
    public class OnSelectedCounterChangedEventArgs : EventArgs {    // 사용자 정의 이벤트 인자 클래스
        public ClearCounter selectedCounter;
    }

    [SerializeField] private float moveSpeed = 5f;  // 이동 속도
    [SerializeField] private GameInput gameInput;   // 입력 관리자
    [SerializeField] private LayerMask counterLayerMask;    // 레이어 식별

    private bool isWalking = false;     // 현재 걷고 있는지 여부
    private Vector3 lastInteractDir;    // 마지막 상호작용 방향
    private ClearCounter selectedCounter;   // 선택된 카운터

    private void Awake() {
        if (Instance != null) {
            Debug.LogError("There is more than one Player intance.");
        }
        Instance = this;
    }

    private void Start() {
        // 이벤트 핸들러 연결
        // OnInteractAction 이벤트가 발생할 때 GameInput_OnInteractAction 메서드 호출
        gameInput.OnInteractAction += GameInput_OnInteractAction;   
    }

    // 상호작용 이벤트 핸들러
    private void GameInput_OnInteractAction(object sender, System.EventArgs e) {    
       
        if (selectedCounter != null) {
            selectedCounter.Interact();
        }
    }

    private void Update() {
        HandleMovement();   // 이동
        HandleInteractions();   // 상호작용 처리
    }

    // 플레이어가 걷고 있는지 여부를 반환하는 메서드
    public bool IsWalking() {
        return isWalking;
    }

    private void HandleInteractions() {
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();  // 입력에 따라 이동
        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);

        // 마지막 방향 업데이트
        // 가만히 있을 때에도 장애물 인식 가능
        if (moveDir != Vector3.zero) {
            lastInteractDir = moveDir;
        }

        float interactDistance = 2f;    // 상호작용 거리
        if (Physics.Raycast(transform.position, lastInteractDir, out RaycastHit raycastHit, interactDistance, counterLayerMask)) {    // 레이캐스트로 객체 검사
            if (raycastHit.transform.TryGetComponent(out ClearCounter clearCounter)) {
                if(clearCounter != selectedCounter) {
                    SetSelectedCounter(clearCounter); // 선택된 카운터 업데이트
                }
            } else {
                SetSelectedCounter(null); // 선택된 카운터 X
            }
        } else {
            SetSelectedCounter(null); // 선택된 카운터 X
        }
    }

    // 이동 처리 메서드
    private void HandleMovement() {
        Vector2 inputVector = gameInput.GetMovementVectorNormalized(); 
        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);  // 이동 방향 계산
        float moveDistance = moveSpeed * Time.deltaTime;   // 이동 거리 계산

        // 캡슐 캐스트를 통해 이동 가능 여부 판단
        float playerRadius = .7f;
        float playerHeight = 2f;
        bool canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDir, moveDistance);

        // 이동 방향별로 캡슐 캐스트로 재검사
        if (!canMove) {
            Vector3 moveDirX = new Vector3(moveDir.x, 0, 0).normalized;
            canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirX, moveDistance);

            if (canMove) {
                moveDir = moveDirX;
            } else {
                Vector3 moveDirZ = new Vector3(0, 0, moveDir.z).normalized;
                canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirZ, moveDistance);

                if (canMove) {
                    moveDir = moveDirZ;
                } else {
                    // Cannot move in any direction
                }
            }
        }

        if (canMove) {
            transform.position += moveDir * moveDistance;   // 실제 이동 수행
        }

        isWalking = (moveDir != Vector3.zero);  // 걷고 있는지 상태 업데이트
        float rotateSpeed = 10f;
        transform.forward = Vector3.Slerp(transform.forward, moveDir, Time.deltaTime * rotateSpeed);    // 플레이어 회전 처리
    }

    private void SetSelectedCounter(ClearCounter selectedCounter) {
        this.selectedCounter = selectedCounter;

        OnSelectedCounterChanged?.Invoke(this, new OnSelectedCounterChangedEventArgs { // 상태 변화 이벤트 발생
            selectedCounter = selectedCounter
        }) ;
    }
}
