using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

public class ControlsEvents : MonoBehaviour
{
    public delegate void Axis(Vector2 axisValues);
    public static event Axis AxisEvent;

    public delegate void Jump();
    public static event Jump JumpEvent;

    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector2 axisValues = new Vector2(horizontal, vertical);

        if(Input.GetButtonDown("XboxA"))
        {
            JumpEvent();
        }

        AxisEvent(axisValues);
    }
}
