using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    public static Player Instance { get; private set; }    // �̱��� ����

    public event EventHandler<OnSelectedCounterChangedEventArgs> OnSelectedCounterChanged;  // �̺�Ʈ ����
    public class OnSelectedCounterChangedEventArgs : EventArgs {    // ����� ���� �̺�Ʈ ���� Ŭ����
        public ClearCounter selectedCounter;
    }

    [SerializeField] private float moveSpeed = 5f;  // �̵� �ӵ�
    [SerializeField] private GameInput gameInput;   // �Է� ������
    [SerializeField] private LayerMask counterLayerMask;    // ���̾� �ĺ�

    private bool isWalking = false;     // ���� �Ȱ� �ִ��� ����
    private Vector3 lastInteractDir;    // ������ ��ȣ�ۿ� ����
    private ClearCounter selectedCounter;   // ���õ� ī����

    private void Awake() {
        if (Instance != null) {
            Debug.LogError("There is more than one Player intance.");
        }
        Instance = this;
    }

    private void Start() {
        // �̺�Ʈ �ڵ鷯 ����
        // OnInteractAction �̺�Ʈ�� �߻��� �� GameInput_OnInteractAction �޼��� ȣ��
        gameInput.OnInteractAction += GameInput_OnInteractAction;   
    }

    // ��ȣ�ۿ� �̺�Ʈ �ڵ鷯
    private void GameInput_OnInteractAction(object sender, System.EventArgs e) {    
       
        if (selectedCounter != null) {
            selectedCounter.Interact();
        }
    }

    private void Update() {
        HandleMovement();   // �̵�
        HandleInteractions();   // ��ȣ�ۿ� ó��
    }

    // �÷��̾ �Ȱ� �ִ��� ���θ� ��ȯ�ϴ� �޼���
    public bool IsWalking() {
        return isWalking;
    }

    private void HandleInteractions() {
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();  // �Է¿� ���� �̵�
        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);

        // ������ ���� ������Ʈ
        // ������ ���� ������ ��ֹ� �ν� ����
        if (moveDir != Vector3.zero) {
            lastInteractDir = moveDir;
        }

        float interactDistance = 2f;    // ��ȣ�ۿ� �Ÿ�
        if (Physics.Raycast(transform.position, lastInteractDir, out RaycastHit raycastHit, interactDistance, counterLayerMask)) {    // ����ĳ��Ʈ�� ��ü �˻�
            if (raycastHit.transform.TryGetComponent(out ClearCounter clearCounter)) {
                if(clearCounter != selectedCounter) {
                    SetSelectedCounter(clearCounter); // ���õ� ī���� ������Ʈ
                }
            } else {
                SetSelectedCounter(null); // ���õ� ī���� X
            }
        } else {
            SetSelectedCounter(null); // ���õ� ī���� X
        }
    }

    // �̵� ó�� �޼���
    private void HandleMovement() {
        Vector2 inputVector = gameInput.GetMovementVectorNormalized(); 
        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);  // �̵� ���� ���
        float moveDistance = moveSpeed * Time.deltaTime;   // �̵� �Ÿ� ���

        // ĸ�� ĳ��Ʈ�� ���� �̵� ���� ���� �Ǵ�
        float playerRadius = .7f;
        float playerHeight = 2f;
        bool canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDir, moveDistance);

        // �̵� ���⺰�� ĸ�� ĳ��Ʈ�� ��˻�
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
            transform.position += moveDir * moveDistance;   // ���� �̵� ����
        }

        isWalking = (moveDir != Vector3.zero);  // �Ȱ� �ִ��� ���� ������Ʈ
        float rotateSpeed = 10f;
        transform.forward = Vector3.Slerp(transform.forward, moveDir, Time.deltaTime * rotateSpeed);    // �÷��̾� ȸ�� ó��
    }

    private void SetSelectedCounter(ClearCounter selectedCounter) {
        this.selectedCounter = selectedCounter;

        OnSelectedCounterChanged?.Invoke(this, new OnSelectedCounterChangedEventArgs { // ���� ��ȭ �̺�Ʈ �߻�
            selectedCounter = selectedCounter
        }) ;
    }
}
