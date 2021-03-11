using UnityEngine;
using UnityEngine.EventSystems;

public class CameraControl : MonoBehaviour
{
    [Space(20)]
    [Header("Mouse Look Settings :")]
    public Vector2 ClampInDegrees = new Vector2(360, 180);
    public Vector2 Sensitivity = new Vector2(2, 2);
    public Vector2 Smoothing = new Vector2(3, 3);
    public Vector2 TargetDirection;
    public Vector2 TargetCharacterDirection;
    // Assign this if there's a parent object controlling motion, such as a Character Controller.
    // Yaw rotation will affect this object instead of the camera if set.
    public GameObject CharacterBody = null;
    public bool MouseLookEnabled
    {
        get { return mouseLookEnabled; }
        set
        {
            mouseLookEnabled = value;
            if (mouseLookEnabled)
            {
                Cursor.lockState = CursorLockMode.Locked;
                // Unselect focused UI object
                // https://forum.unity.com/threads/how-to-deselect-one-specific-button.428833/
                GameObject.Find("EventSystem").GetComponent<EventSystem>().SetSelectedGameObject(null);
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
            }
        }
    }
    private bool mouseLookEnabled = false;
    public KeyCode[] EnableDisableKey = new KeyCode[2] { KeyCode.LeftAlt, KeyCode.RightAlt };


    [Space(20)]
    [Header("Camera Move Settings :")]

    public float Speed = 500.0f;

    public KeyCode ForwardKey = KeyCode.W;
    public KeyCode LeftKey = KeyCode.A;
    public KeyCode BackKey = KeyCode.S;
    public KeyCode RightKey = KeyCode.D;

    private Vector2 _mouseAbsolute;
    private Vector2 _smoothMouse;
    /*
    public void EnableMouseLook()
    {
        MouseLookEnabled = true;

        
    }

    public void DisableMouseLook()
    {
        MouseLookEnabled = false;

        // Configure mouse cursor
        
    }
    */
    void Start()
    {
        // Set target direction to the camera's initial orientation.
        TargetDirection = transform.localRotation.eulerAngles;

        // Set target direction for the character body to its inital state.
        if (CharacterBody)
            TargetCharacterDirection = CharacterBody.transform.localRotation.eulerAngles;

        // Initialise camera mode
        /*if (MouseLookEnabled)
        {
            EnableMouseLook();
        }
        else
        {
            DisableMouseLook();
        }*/
    }

    void Update()
    {
        foreach (KeyCode key in EnableDisableKey)
        {
            if ((Input.GetKeyDown(key)))
            {
                MouseLookEnabled = !MouseLookEnabled;
                break;
            }
        }
        /*if ((Input.GetKeyDown(KeyCode.LeftAlt) || Input.GetKeyDown(KeyCode.RightAlt)))
        {
            MouseLookEnabled = !MouseLookEnabled;
            if (MouseLookEnabled)
            {
                DisableMouseLook();
            }
            else
            {
                EnableMouseLook();
            }
        }*/

        if (MouseLookEnabled)
        {
            // When cursor is locked, it is not visible. When locked, it must be set visible in each Upate iteration.
            Cursor.visible = true;

            // Allow the script to clamp based on a desired target value.
            var targetOrientation = Quaternion.Euler(TargetDirection);
            var targetCharacterOrientation = Quaternion.Euler(TargetCharacterDirection);

            // Get raw mouse input for a cleaner reading on more sensitive mice.
            var mouseDelta = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));

            // Scale input against the sensitivity setting and multiply that against the smoothing value.
            mouseDelta = Vector2.Scale(mouseDelta, new Vector2(Sensitivity.x * Smoothing.x, Sensitivity.y * Smoothing.y));

            // Interpolate mouse movement over time to apply smoothing delta.
            _smoothMouse.x = Mathf.Lerp(_smoothMouse.x, mouseDelta.x, 1f / Smoothing.x);
            _smoothMouse.y = Mathf.Lerp(_smoothMouse.y, mouseDelta.y, 1f / Smoothing.y);

            // Find the absolute mouse movement value from point zero.
            _mouseAbsolute += _smoothMouse;

            // Clamp and apply the local x value first, so as not to be affected by world transforms.
            if (ClampInDegrees.x < 360)
                _mouseAbsolute.x = Mathf.Clamp(_mouseAbsolute.x, -ClampInDegrees.x * 0.5f, ClampInDegrees.x * 0.5f);

            var xRotation = Quaternion.AngleAxis(-_mouseAbsolute.y, targetOrientation * Vector3.right);
            transform.localRotation = xRotation;

            // Then clamp and apply the global y value.
            if (ClampInDegrees.y < 360)
                _mouseAbsolute.y = Mathf.Clamp(_mouseAbsolute.y, -ClampInDegrees.y * 0.5f, ClampInDegrees.y * 0.5f);

            transform.localRotation *= targetOrientation;

            // If there's a character body that acts as a parent to the camera
            if (CharacterBody)
            {
                var yRotation = Quaternion.AngleAxis(_mouseAbsolute.x, CharacterBody.transform.up);
                CharacterBody.transform.localRotation = yRotation;
                CharacterBody.transform.localRotation *= targetCharacterOrientation;
            }
            else
            {
                var yRotation = Quaternion.AngleAxis(_mouseAbsolute.x, transform.InverseTransformDirection(Vector3.up));
                transform.localRotation *= yRotation;
            }

            // Allow movement only when no GUI element is focused
            if (!DiagramInputHandler.InputElementActive)
            {
                if (Input.GetKey(RightKey))
                {
                    transform.Translate(new Vector3(Speed * Time.deltaTime, 0, 0));
                }
                if (Input.GetKey(LeftKey))
                {
                    transform.Translate(new Vector3(-Speed * Time.deltaTime, 0, 0));
                }
                if (Input.GetKey(BackKey))
                {
                    transform.Translate(new Vector3(0, 0, -Speed * Time.deltaTime));
                }
                if (Input.GetKey(ForwardKey))
                {
                    transform.Translate(new Vector3(0, 0, Speed * Time.deltaTime));
                }
            }
        }
    }

 
}
