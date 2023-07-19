using Photon.Pun;
using System;
using UnityEngine;

public class WeaponSway : MonoBehaviour
{

    [Header("Old Sway Settings")]
    [SerializeField] private float oldSmooth;
    [SerializeField] private float oldMultiplier;
    [SerializeField]PlayerController controller;
    [SerializeField] PhotonView PV;


    [Header("Sway")]
    public float step = 0.01f;
    public float maxStepDistance = 0.06f;
    Vector3 swayPos;


    [Header("Sway Rotation")]
    public float rotationStep = 4f;
    public float maxRotationStep = 5f;
    Vector3 swayEulerRot;
    [Header("Bobbing")]
    public float speedCurve;
    float curveSin { get => Mathf.Sin(speedCurve); }
    float curveCos { get => Mathf.Cos(speedCurve); }

    public AnimationCurve animCurve;
    public Vector3 travelLimit = Vector3.one * 0.025f;
    public Vector3 bobLimit = Vector3.one * 0.01f;
    private Vector3 bobPosition;

    public float smooth = 10f;
    float smoothRot = 12f;

    private Vector2 walkInput;
    private Vector2 lookInput;
    public float bobExaggeration;

    [Header("Bob Rotation")]
    public Vector3 bobMultiplier;
    Vector3 bobEulerRotation;

    void GetInput()
    {
        walkInput.x = Input.GetAxis("Horizontal");
        walkInput.y = Input.GetAxis("Vertical");
        walkInput = walkInput.normalized;
        lookInput.x = Input.GetAxis("Mouse X");
        lookInput.y = Input.GetAxis("Mouse Y");

    }
    void BobOffset()
    {
        speedCurve += Time.deltaTime * 8f* bobExaggeration + 0.01f;

        bobPosition.x = (curveCos * bobLimit.x * (controller.isGrounded ? 1 : 0)) - (walkInput.x * travelLimit.x);
        bobPosition.y = (curveSin * bobLimit.y) - (Input.GetAxis("Vertical") * travelLimit.y);
        bobPosition.z = -(walkInput.y * travelLimit.z);
    }

    void BobRotation()
    {
        bobEulerRotation.x = (walkInput != Vector2.zero ? bobMultiplier.x * animCurve.Evaluate(controller.GetCurrentPlayerVelocity()/18f)*8 * (Mathf.Sin(2 * speedCurve)) : 0);
        bobEulerRotation.y = (walkInput != Vector2.zero ? bobMultiplier.y * animCurve.Evaluate(controller.GetCurrentPlayerVelocity()/18f)* controller.GetCurrentPlayerVelocity() * curveCos : 0);
        bobEulerRotation.z = (walkInput != Vector2.zero ? bobMultiplier.z  * animCurve.Evaluate(controller.GetCurrentPlayerVelocity()/18f)*8 * curveCos * walkInput.x : 0);
        Debug.Log(bobEulerRotation.z);
    }
    void Sway()
    {
        Vector3 invertLook = lookInput * -step;
        invertLook.x = Mathf.Clamp(invertLook.x, -maxStepDistance, maxStepDistance);
        invertLook.y = Mathf.Clamp(invertLook.y, -maxStepDistance, maxStepDistance);

        swayPos = invertLook;
    }

    void SwayRotation()
    {
        Vector2 invertLook = lookInput * -rotationStep;
        invertLook.x = Mathf.Clamp(invertLook.x, -maxRotationStep, maxRotationStep);
        invertLook.y = Mathf.Clamp(invertLook.y, -maxRotationStep, maxRotationStep);
        swayEulerRot = new Vector3(invertLook.y, invertLook.x, invertLook.x);
    }

    void CompositePositionRotation()
    {
        //transform.localPosition = Vector3.Lerp(transform.localPosition, swayPos + bobPosition, Time.deltaTime * smooth);
        transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(swayEulerRot) * Quaternion.Euler(bobEulerRotation), Time.deltaTime * smoothRot);
    }

    void OldSway()//old sway script
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * oldMultiplier;
        float mouseY = Input.GetAxisRaw("Mouse Y") * oldMultiplier;
        Quaternion rotationX = Quaternion.AngleAxis(-mouseY, Vector3.right);
        Quaternion rotationY = Quaternion.AngleAxis(mouseX, Vector3.up);

        Quaternion targetRotation = rotationX * rotationY;

        // rotate 
        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, oldSmooth * Time.deltaTime);
    }

    private void Update()
    {
        if (PV != null)
        {
            if (PV.IsMine)
            {
                Debug.Log(animCurve.Evaluate(controller.GetCurrentPlayerVelocity()/18f)+ " Curve");
                Debug.Log(controller.GetCurrentPlayerVelocity()+ " Velocity");
                //get key input
                GetInput();
                Sway();
                SwayRotation();
                //bobbing
                BobOffset();
                BobRotation();
                CompositePositionRotation();

                //OldSway();
                
                
                // get mouse input
                //float mouseX = Input.GetAxisRaw("Mouse X") * multiplier;
                //float mouseY = Input.GetAxisRaw("Mouse Y") * multiplier;
                //swaying
                // calculate target rotation
                /**
                Quaternion rotationX = Quaternion.AngleAxis(-mouseY, Vector3.right);
                Quaternion rotationY = Quaternion.AngleAxis(mouseX, Vector3.up);

                Quaternion targetRotation = rotationX * rotationY;

                // rotate 
                transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, smooth * Time.deltaTime);
                */
            }
        }
       
    }
}