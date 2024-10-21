using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class FPController : MonoBehaviour
{
    public GameObject cam;
    public Animator anim;
    float speed = 0.1f;
    float Xsensitivity = 2;
    float Ysensitivity = 2;
    float MinimunX = -90;
    float MaximumX = 90;

    bool cursorIsLocked = true;
    bool lockedCursor = true;

    Rigidbody rb;
    CapsuleCollider capsule;

    Quaternion cameraRot;
    Quaternion CharacterRot;

    // Start is called before the first frame update
    void Start()
    {
        rb = this.GetComponent<Rigidbody>();
        capsule = this.GetComponent<CapsuleCollider>();

        cameraRot = cam.transform.localRotation;
        CharacterRot = this.transform.localRotation;

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
            anim.SetBool("Arm", !anim.GetBool("Arm"));

        if (Input.GetMouseButtonDown(0))
            anim.SetBool("Fire", true);
        else if (Input.GetMouseButtonUp(0))
            anim.SetBool("Fire", false);    
    }

    void FixedUpdate()
    {
        float yRot = Input.GetAxis("Mouse X") * Ysensitivity;
        float xRot = Input.GetAxis("Mouse Y") * Xsensitivity;

        cameraRot *= Quaternion.Euler(-xRot, 0, 0);
        CharacterRot *= Quaternion.Euler(0, yRot, 0);

        cameraRot = ClampRotationAroundXAxis(cameraRot);

        this.transform.localRotation = CharacterRot;
        cam.transform.localRotation = cameraRot;

        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
            rb.AddForce(0, 300, 0);

        float x = Input.GetAxis("Horizontal") * speed;
        float z = Input.GetAxis("Vertical") * speed;

        transform.position += cam.transform.forward * z + cam.transform.right * x; //new Vector3(x * speed, 0, z * speed);

        UpdateCursorLock();
    }


    Quaternion ClampRotationAroundXAxis(Quaternion q)
    {
        q.x /= q.w;
        q.y /= q.w;
        q.z /= q.w;
        q.w = 1.0f;

        float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);
        angleX = Mathf.Clamp(angleX, MinimunX, MaximumX);
        q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);

        return q;
    }

    bool IsGrounded()
    {
        RaycastHit hitInfo;
        if (Physics.SphereCast(transform.position, capsule.radius, Vector3.down, out hitInfo,
                (capsule.height / 2f) - capsule.radius + 0.1f))
        {
            return true;
        }
        return false;
    }

    public void SetCursorLock(bool value)
    {
        lockedCursor = value;
        if (!lockedCursor)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void UpdateCursorLock()
    {
        if (lockedCursor)
            InternalLockedUpdate();
    }

    public void InternalLockedUpdate()
    {
        if(Input.GetKeyUp(KeyCode.Escape))
            cursorIsLocked = false;
        else if (Input.GetMouseButtonUp(0))
        cursorIsLocked = true;

        if (cursorIsLocked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else if(!cursorIsLocked)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

}
