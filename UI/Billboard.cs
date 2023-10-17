using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class Billboard : MonoBehaviour
{
    void LateUpdate()
    {
        if (CameraManager.Instance != null)
            transform.LookAt(transform.position + CameraManager.Instance.GetMainCamera().transform.forward);
        else
            transform.LookAt(transform.position + Camera.main.transform.forward);
    }
}
