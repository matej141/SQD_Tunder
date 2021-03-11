using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DiagramInputHandler : MonoBehaviour
{
    public static DiagramInputHandler Singleton { get; private set; } = null;

    public static DiagramTool DefaultTool = null;

    public static bool InputElementActive { get; private set; } = true;

    public DiagramTool CurrentTool { get; private set; } = null;
    public HashSet<DiagramTool> ActiveTool { get; private set; } = new HashSet<DiagramTool>();

    public DateTime PreviousTick { get; private set; }
    public DateTime CurrentTick { get; private set; }
    public TimeSpan TimeDelta { get; private set; }

    public Vector3 PreviousMouse { get; private set; }
    public Vector3 CurrentMouse { get; private set; }
    public Vector3 MouseDelta { get; private set; }

    public List<RaycastResult> GuiRaycast { get; private set; }
    public RaycastHit[] PhysicRaycast { get; private set; }

    public void Awake()
    {
        if (Singleton == null) Singleton = this;
    }

    // Start is called before the first frame update
    public void Start()
    {
        CurrentTick = DateTime.Now;
        PreviousTick = CurrentTick;
        TimeDelta = CurrentTick - PreviousTick;

        CurrentMouse = Input.mousePosition;
        PreviousMouse = CurrentMouse;
        MouseDelta = CurrentMouse - PreviousMouse;

        GuiRaycast = RaycastHelper.FilterNearest(RaycastHelper.RaycastAllWorldUI());
        PhysicRaycast = RaycastHelper.RaycastAllWorldPhysics();

        if (DefaultTool == null) DefaultTool = GameObject.FindObjectOfType<HandTool>();
        if (CurrentTool == null) ChooseTool(DefaultTool);
    }

    // Update is called once per frame
    public void Update()
    {


        // ~~~~~~~~~~~ COMMON ~~~~~~~~~~~


        // Compute time globally
        PreviousTick = CurrentTick;
        CurrentTick = DateTime.Now;
        TimeDelta = CurrentTick - PreviousTick;

        // Compare mouse change
        PreviousMouse = CurrentMouse;
        CurrentMouse = Input.mousePosition;
        MouseDelta = CurrentMouse - PreviousMouse;

        // Check for conflicts
        // https://forum.unity.com/threads/detecting-if-an-input-control-has-focus-with-solution.429858/
        if (EventSystem.current.currentSelectedGameObject == null)
        {
            InputElementActive = false;
        }
        else
        {
            InputElementActive = true;
        }

        // Skip for no tool selected
        if (CurrentTool == null)
        {
            PreviousTick = CurrentTick;
            return;
        }

        // Single global raycasts
        GuiRaycast = RaycastHelper.FilterNearest(RaycastHelper.RaycastAllWorldUI());
        PhysicRaycast = RaycastHelper.RaycastAllWorldPhysics();


        // ~~~~~~~ TOOL SPECIFIC ~~~~~~~~


        // Tool specific update operation
        CurrentTool.OnLoop(this);

        // Delegate mouse button
        if (Input.GetMouseButton(0))
        {            
            if (Input.GetMouseButtonDown(0))
            {
                // Mouse click start
                CurrentTool.OnMouseButtonStart(this);
            }
            else
            {
                // Mouse click continues
                CurrentTool.OnMouseButtonContinue(this);
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            // Mouse click ended
            CurrentTool.OnMouseButtonEnd(this);
        }
        else
        {
            // Mouse is moving without any button being pressed
            CurrentTool.OnMouseLook(this);
        }

        // Delegate keys
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Always delegate Escape key
            CurrentTool.OnEscapeKey(this);
        }
        if (InputElementActive)
        {
            // Do not delegate special keys, when UI elements are focued
        }
        else
        {
            // No UI element is focused so you can delegate these keys
            // Delete key
            if (Input.GetKeyDown(KeyCode.Delete))
                CurrentTool.OnDeleteKey(this);
        }
    }

    public DiagramTool ChooseMyTool(DiagramTool newTool)
    {
        DiagramTool oldTool = CurrentTool;
        if (newTool == null) newTool = DefaultTool;
        CurrentTool = newTool;

        // Choosing the same tool restarts the tool

        if (oldTool != null)
        {
            oldTool.OnToolEnd(newTool, this);
        }
        if (newTool != null)
        {
            newTool.OnToolStart(oldTool, this);
        }

        return oldTool;
    }

    public static DiagramTool ChooseTool(DiagramTool newTool)
    {
        return Singleton.ChooseMyTool(newTool);
    }

    public void ActivateTool(DiagramTool tool)
    {
        ActiveTool.Add(tool);
    }

    public void DectivateTool(DiagramTool tool)
    {
        ActiveTool.Remove(tool);
    }

}
