using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingHook : MonoBehaviour
{
    // Lets all three of these be changed in the inspector on unity
    [SerializeField] private float grappleLen;  // how far can the grapple grab to
    [SerializeField] private LayerMask grappleLayer;    // what set of items can we grapple to
    [SerializeField] private LineRenderer rope;         // initialize the rope

    private Vector3 grapplePoint;   // what we are grappling to
    private DistanceJoint2D joint;  // intialize 2d joint for the rope
    // Start is called before the first frame update
    void Start()
    {
        joint = gameObject.GetComponent<DistanceJoint2D>(); // grabs the 2d joint
        joint.enabled = false;  // want to start as disabled since we aren't constantly using the grappling hook
        rope.enabled = false;   // don't need the roop drawn if there isn't a click yet
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            // intilizing my ray cast
            RaycastHit2D hit = Physics2D.Raycast(
            origin: Camera.main.ScreenToWorldPoint(Input.mousePosition), 
            direction: Vector2.zero, 
            distance: Mathf.Infinity,
            layerMask: grappleLayer);

            if(hit.collider != null) // if the ray actually hits something intialize grapple
            {
                grapplePoint = hit.point;   // set grapple to hit
                grapplePoint.z = 0;     // don't need the z coordinate to change since its 2d
                joint.connectedAnchor = grapplePoint;
                joint.enabled = true;
                // set joint distance to grapple length so that the 2d joint is the right size
                joint.distance = grappleLen;   
                rope.SetPosition(0, grapplePoint);
                rope.SetPosition(1, transform.position);
                rope.enabled = true;
            }
        }

        if(Input.GetMouseButtonUp(0))
        {
           joint.enabled = false;
           rope.enabled = false;
        }

        if(rope.enabled == true)
        {
            rope.SetPosition(1, transform.position);
        }
    }
}