using System.Drawing;

using UnityEngine.InputSystem;
using UnityEngine;
using Color = UnityEngine.Color;

public class SpaceShipController : MonoBehaviour
{
    public InputActionReference move;
    public InputActionReference thrust;
    public InputActionReference boost;
    public SpaceShipStats stats;
    public Rigidbody rb;
    public Material trailMaterial;

    private GameManager manager;
    private GameObject[] thrusters;
    private TrailRenderer[] trails;
    private Vector3 controllInput;
    private float thrustInput;
    private bool boostInput;
    
    
    void Start()
    {
        manager = GameObject.Find("GameManager").GetComponent<GameManager>();
        stats.Spaceship_1();
        rb.mass = stats.mass;
        thrusters = GameObject.FindGameObjectsWithTag("Thrusters");
        trails = new TrailRenderer[thrusters.Length];

        for (int i = 0; i < thrusters.Length; i++)
        {
            trails[i] = thrusters[i].AddComponent<TrailRenderer>();
            trails[i].time = 0.1f;                    
            trails[i].minVertexDistance = 0.2f;      
            trails[i].emitting = false;
            trails[i].startWidth = 0.04f;
            trails[i].endWidth = 0f;
            trails[i].material = trailMaterial;
            trails[i].textureMode = LineTextureMode.Stretch;
        }
    }
    public void TrailsOn(bool on = false)
    {
        foreach(TrailRenderer trail in trails)
        {
            if (on)
            {
                trail.emitting = true;
                if (boostInput)
                {
                    trail.startWidth = Mathf.Lerp(trail.startWidth,0.1f,Time.fixedDeltaTime);
                    trail.time = Mathf.Lerp(trail.time, 0.3f, Time.fixedDeltaTime);
                }
                else
                {
                    trail.startWidth = Mathf.Lerp(trail.startWidth, 0.04f, Time.fixedDeltaTime);
                    trail.time = Mathf.Lerp(trail.time, 0.2f, Time.fixedDeltaTime);
                }
                
            }
            else
            {
                trail.emitting = false;
                //trail.Clear();
            }
            
        }
    }
    private void Update()
    {
        controllInput = move.action.ReadValue<Vector3>().normalized;
        thrustInput = thrust.action.ReadValue<float>();
        boostInput = boost.action.IsPressed();
    }
    
    private void FixedUpdate()
    {
        LinearControl();
        AngularControl();
    } 
    public void AngularControl()
    {
        stats.angularVelocity = transform.InverseTransformDirection(rb.angularVelocity);
        stats.rotation = manager.ConvertEuler(transform.eulerAngles);
        Vector3 targetTorque = Vector3.zero;

        for (int i = 0; i < 3; i++)
        {
            if (Mathf.Abs(controllInput[i]) > 0.01f)
            {
                targetTorque[i] = controllInput[i] * stats.maxTorque[i];
                if (Mathf.Sign(controllInput[i]) != Mathf.Sign(stats.angularVelocity[i]) && Mathf.Abs(stats.angularVelocity[i]) > 0.01f)
                {
                    targetTorque[i] = -stats.angularVelocity[i] * stats.counterTorque;
                }
            }
            else
            {
                if (i == 2 && Mathf.Abs(stats.rotation[i]) > 0.1f)
                {
                    float rollAngle = Mathf.DeltaAngle(stats.rotation[i], 0f) * Mathf.Deg2Rad; 
                    targetTorque[i] = rollAngle * stats.maxTorque[i] - stats.angularVelocity[i] * stats.counterTorque;
                    targetTorque[i] = Mathf.Clamp(targetTorque[i], -stats.maxTorque[i], stats.maxTorque[i]);
                }
                else
                {
                    targetTorque[i] = -stats.angularVelocity[i] * stats.counterTorque;
                }
            }
        }

        stats.currentTorque = Vector3.Slerp(
            stats.currentTorque,
            targetTorque,
            stats.torqueAcceleration * Time.fixedDeltaTime
        );
        rb.AddRelativeTorque(stats.currentTorque, ForceMode.Acceleration);

        if (stats.angularVelocity.magnitude > stats.maxAV)
        {
            stats.angularVelocity = stats.angularVelocity.normalized * stats.maxAV;
            rb.angularVelocity = transform.TransformDirection(stats.angularVelocity);
        }
        else if (stats.angularVelocity.magnitude < 0.1f)
        {
            stats.angularVelocity = Vector3.zero;
            rb.angularVelocity = transform.TransformDirection(stats.angularVelocity);
        }

    }
    public void LinearControl()
    {
        stats.linearVelocity = rb.transform.InverseTransformDirection(rb.linearVelocity);
        Vector3 targetThrust = Vector3.zero;
        if(thrustInput > 0.01f)
        {
            if (boostInput)
            {  
                stats.maxV = Mathf.Lerp(stats.maxV, 10f, Time.fixedDeltaTime);
            }
            else
            {
                stats.maxV = Mathf.Lerp(stats.maxV, 2f, Time.fixedDeltaTime);
            }
            targetThrust = Vector3.forward * stats.maxThrust;
            TrailsOn(true);
        }
        else if (Mathf.Abs(thrustInput) < 0.01f && Mathf.Abs(stats.linearVelocity.magnitude) > 0.1f)
        {
            targetThrust = -stats.linearVelocity.normalized * stats.counterThrust;
            TrailsOn(false);
        }
        stats.currentThrust = Vector3.MoveTowards(
            stats.currentThrust,
            targetThrust,
            stats.thrustAcceleration * Time.fixedDeltaTime
        );
        rb.AddRelativeForce(stats.currentThrust, ForceMode.Acceleration);

        if (stats.linearVelocity.magnitude > stats.maxV)
        {
            stats.linearVelocity = stats.linearVelocity.normalized * stats.maxV;
            rb.linearVelocity = transform.TransformDirection(stats.linearVelocity);
        }
        else if(stats.linearVelocity.magnitude < 0.1f)
        {
            stats.linearVelocity = Vector3.zero;
            rb.linearVelocity = transform.TransformDirection(stats.linearVelocity);
        }
    }
    
}
