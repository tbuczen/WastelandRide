using UnityEngine;
using System.Collections;
using System;

public class CamMouseLookController : MonoBehaviour {

    Vector2 mouseLook, smoothV;
    static Animator animator;
    public float sensivity = 90.0f;
    public float smoothing = 2.0f;
    public float maxLookUpDownAngle = 70;
    public float maxLookRightLeftAngle = 70;

    private float upDownRotation = 0.0f;
    private float rightLeftRotation = 0.0f;
    
    //public GameObject character;
    public Camera fp, tp;
    private Camera current;

    public GameObject fpCameraPlaceholder, tpCameraPlaceholder;
    private Vector3 fpCameraHeadDistance;
    private GameObject currentPlaceholder;

    public GameObject headBone;
    private static readonly int IsRunning = Animator.StringToHash("isRunning");
    private static readonly int IsJumping = Animator.StringToHash("isJumping");


    // Use this for initialization
    void Start ()
    {
        tp.enabled = true;
        fp.enabled = false;
        fpCameraPlaceholder.transform.position =  headBone.transform.position;
        if (fp.enabled)
        {
            current = fp;
            currentPlaceholder = fpCameraPlaceholder;
        } else {
            current = tp;
            currentPlaceholder = tpCameraPlaceholder;
        }

        
        animator = GetComponent<Animator>();
    }

    void Update () {
        changeCamera();
        PollForAxisAdjustments();
    }

    //Set bones rotation after animation in late update
    void LateUpdate() {
        //set Y of camera to match position of head (height)
        if (fp.enabled)
        {
            currentPlaceholder.transform.position = headBone.transform.position;
        }
        //Set head rotation to camera rotation
        headBone.transform.localEulerAngles = currentPlaceholder.transform.localRotation.eulerAngles;
        //Set neck rotation to player rotation
        headBone.transform.parent.transform.rotation = transform.rotation;
    }

    private void currentRotation()
    {
        var localEulerAngles = currentPlaceholder.transform.localEulerAngles;
        //left-right
        rightLeftRotation = localEulerAngles.y + Input.GetAxis("Mouse X") * sensivity;
        //up-dwon
        upDownRotation = localEulerAngles.x + Input.GetAxis("Mouse Y") * -sensivity;
        localEulerAngles = new Vector3(upDownRotation, rightLeftRotation, 0);
        currentPlaceholder.transform.localEulerAngles = localEulerAngles;
    }

    private void PollForAxisAdjustments()
    {
        float xAxisMovement = Input.GetAxis("Mouse X") * sensivity * Time.deltaTime;
        float yAxisMovement = Input.GetAxis("Mouse Y") * sensivity * Time.deltaTime;
        makeXAxisAdjustments(xAxisMovement);
        makeYAxisAdjustments(yAxisMovement);
    }

    private void makeXAxisAdjustments(float adjustment)
    {
        if (adjustment == 0) return;
        Vector3 currentRotation = currentPlaceholder.transform.localRotation.eulerAngles;
        
        float newRot = (currentRotation.y + adjustment);
        newRot = ClampAngle(newRot,-maxLookUpDownAngle, maxLookUpDownAngle);

        Quaternion newRotation = Quaternion.Euler(currentRotation.x, newRot, currentRotation.z);
        currentPlaceholder.transform.localRotation = newRotation;
    }

    private void makeYAxisAdjustments(float adjustment)
    {
        if (adjustment == 0) return;
        Vector3 currentRotation = currentPlaceholder.transform.localRotation.eulerAngles;

        float newRot = (currentRotation.x - adjustment);
        newRot = ClampAngle(newRot, -maxLookRightLeftAngle, maxLookRightLeftAngle);

        Quaternion newRotation = Quaternion.Euler(newRot, currentRotation.y, currentRotation.z);
        currentPlaceholder.transform.localRotation = newRotation;
    }

    private void changeCamera()
    {
        if (Input.GetButtonDown("ToggleCamera")) {
            fp.enabled = !fp.enabled;
            tp.enabled = !tp.enabled;
            if (fp.enabled)
            {
                current = fp;
                currentPlaceholder = fpCameraPlaceholder;
            }
            else
            {
                current = tp;
                currentPlaceholder = tpCameraPlaceholder;
            }
        }
    }

    public static float ClampAngle(float angle, float min, float max)
    {
        angle = Mathf.Repeat(angle, 360);
        min = Mathf.Repeat(min, 360);
        max = Mathf.Repeat(max, 360);
        bool inverse = false;
        var tmin = min;
        var tangle = angle;
        if (min > 180)
        {
            inverse = !inverse;
            tmin -= 180;
        }
        if (angle > 180)
        {
            inverse = !inverse;
            tangle -= 180;
        }
        var result = !inverse ? tangle > tmin : tangle < tmin;
        if (!result)
            angle = min;

        inverse = false;
        tangle = angle;
        var tmax = max;
        if (angle > 180)
        {
            inverse = !inverse;
            tangle -= 180;
        }
        if (max > 180)
        {
            inverse = !inverse;
            tmax -= 180;
        }

        result = !inverse ? tangle < tmax : tangle > tmax;
        if (!result)
            angle = max;
        return angle;
    }
}
