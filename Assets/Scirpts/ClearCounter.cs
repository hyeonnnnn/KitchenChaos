using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearCounter : MonoBehaviour {

    [SerializeField] private KitchenObjectSO kitchenObjectSO;
    [SerializeField] private Transform counterTopPoint;
    [SerializeField] private ClearCounter secondClearCounter;
    [SerializeField] private bool testing;

    private KitchenObject kitchenObject;

    private void Update() {
        if (testing && Input.GetKeyDown(KeyCode.T)) {
            if (kitchenObject != null) {
                kitchenObject.SetClearCounter(secondClearCounter);
            }
        }
    }

    public void Interact() {
        if (kitchenObject == null) {    // ī���� ���� ��ü X
            Transform kitchenObjectTransform = Instantiate(kitchenObjectSO.prefab, counterTopPoint);
            kitchenObjectTransform.GetComponent<KitchenObject>().SetClearCounter(this); // 
        } else {    // ī���� ���� ��ü O
            Debug.Log(kitchenObject.GetClearCounter());
        }
    }

    public Transform GetKitchenObjectFollowTransform() {
        return counterTopPoint;
    }

    // ��ü ����
    public void SetKitchenObject(KitchenObject kitchenObject) {
        this.kitchenObject = kitchenObject;
    }

    // ��ü ��������
    public KitchenObject GetKitchenObject() {
        return kitchenObject;
    }

    // ��ü ���ֱ�
    public void ClearKitchenObject() {
        kitchenObject = null;
    }

    // ��ü�� �ִ��� ����
    public bool HasKitchenObject() {
        return kitchenObject != null;
    }
}
