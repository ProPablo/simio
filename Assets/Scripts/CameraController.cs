using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

[RequireComponent(typeof(CinemachineVirtualCamera))]
public class CameraController : MonoBehaviour
{
    CinemachineVirtualCamera cam;
    Rigidbody rb;
    public float xOffset;
    public float zMinOffset;
    public float zMaxOffset;
    public float lerpDur = 10;
    public float minFov = 20;
    public float maxFov = 80;
    public float scrollSens = 0.75f;
    public float moveSens = 0.5f;

    float lerpTarget = 50; //Starting FoV
    Vector3 moveDir;
    Vector3 pointerPos;
    Vector3 pointerTarget;
    bool mouseDown = false;

    private void Awake()
    {
        cam = GetComponent<CinemachineVirtualCamera>();
        rb = GetComponent<Rigidbody>();
    }
    void Update()
    {
        lerpTarget -= Input.GetAxis("Mouse ScrollWheel") * scrollSens * 100;
        lerpTarget = Mathf.Clamp(lerpTarget, minFov, maxFov);

        cam.m_Lens.FieldOfView = Mathf.Lerp(cam.m_Lens.FieldOfView, lerpTarget, lerpDur * Time.deltaTime);

        moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
        moveDir.x = Mathf.Clamp(moveDir.x, transform.position.x < -HexGrid.i.XMax - xOffset ? 0 : -1, transform.position.x > HexGrid.i.XMax + xOffset ? 0 : 1);
        moveDir.z = Mathf.Clamp(moveDir.z, transform.position.z < -HexGrid.i.ZMax - zMinOffset ? 0 : -1, transform.position.z > HexGrid.i.ZMax + zMaxOffset ? 0 : 1);

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            pointerPos = MouseInWorldCoords();
            mouseDown = true;
        }
        if (Input.GetKeyUp(KeyCode.Mouse1))
            mouseDown = false;
    }
    private Vector3 MouseInWorldCoords()
    {
        Plane plane = new Plane(Vector3.up, Vector3.zero);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        plane.Raycast(ray, out float distanceToPlane);
        return ray.GetPoint(distanceToPlane);
    }

    private void FixedUpdate()
    {
        float speedMultiplier = 1;
        speedMultiplier *= Input.GetKey(KeyCode.LeftShift) ? 2 : 1;
        speedMultiplier *= Input.GetKey(KeyCode.LeftControl) ? 0.5f : 1;

        if (mouseDown)
        {
            pointerTarget = MouseInWorldCoords() - pointerPos;
            Vector3 targetPos = transform.position - pointerTarget;
            rb.MovePosition(new Vector3(Mathf.Clamp(targetPos.x, -HexGrid.i.XMax - xOffset, HexGrid.i.XMax + xOffset), transform.position.y, Mathf.Clamp(targetPos.z, -HexGrid.i.ZMax - zMinOffset, HexGrid.i.ZMax + zMaxOffset)));
        }
        else
            rb.MovePosition(transform.position + moveSens * speedMultiplier * moveDir);
    }
}