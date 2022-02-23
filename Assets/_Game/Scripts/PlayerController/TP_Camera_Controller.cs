using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TP_Camera_Controller : MonoBehaviour{

    [SerializeField] GameObject CinemachineVirtualCameraTarget;
    [SerializeField] private float MouseSensitivityX;
    [SerializeField] private float MouseSensitivityY;
    [SerializeField] private float minCameraPitch;
    [SerializeField] private float maxCameraPitch;
    private float CinemachineTargetYaw;
    private float CinemachineTargetPitch;
    private float MouseX;
    private float MouseY;

    private void LateUpdate() {
        CameraRotation();
    }

    private void CameraRotation() {
        MouseX = Input.GetAxis("Mouse X");
        MouseY = Input.GetAxis("Mouse Y");
        CinemachineTargetYaw += MouseX * Time.deltaTime * MouseSensitivityX;
        CinemachineTargetPitch -= MouseY * Time.deltaTime * MouseSensitivityY;
        CinemachineTargetPitch = Mathf.Clamp(CinemachineTargetPitch, minCameraPitch, maxCameraPitch);
        CinemachineVirtualCameraTarget.transform.rotation = Quaternion.Euler(CinemachineTargetPitch, CinemachineTargetYaw, 0);
    }

}
