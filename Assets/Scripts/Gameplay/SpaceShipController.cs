using UnityEngine.InputSystem;
using UnityEngine;

public class SpaceShipController : MonoBehaviour
{
    public InputActionReference move; //Player input
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
        stats.Spaceship_1(); //Carga los stats de la nave 1.
        rb.mass = stats.mass; //Otorga la masa a la nave.
        thrusters = GameObject.FindGameObjectsWithTag("Thrusters");
        trails = new TrailRenderer[thrusters.Length];

        for (int i = 0; i < thrusters.Length; i++) //Genera el efecto de propulsion en cada motor.
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
    public void TrailsOn(bool on = false) //Enciende o apaga el efecto de propulsión. 
    {
        foreach (TrailRenderer trail in trails)
        {
            if (on)
            {
                trail.emitting = true;
                if (boostInput)
                {
                    trail.startWidth = Mathf.Lerp(trail.startWidth, 0.1f, Time.fixedDeltaTime);
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
    public void AngularControl() //Control de la rotacion de la nave.
    {
        stats.angularVelocity = transform.InverseTransformDirection(rb.angularVelocity);
        stats.rotation = manager.ConvertEuler(transform.eulerAngles);
        Vector3 targetTorque = Vector3.zero; //Fuerza objetivo.

        for (int i = 0; i < 3; i++) //Por cada eje de rotacion calcula que fureza hay que aplicar.
        {
            if (Mathf.Abs(controllInput[i]) > 0.01f) //Si hay input en el eje.
            {
                targetTorque[i] = controllInput[i] * stats.maxTorque[i];
                if (Mathf.Sign(controllInput[i]) != Mathf.Sign(stats.angularVelocity[i]) && Mathf.Abs(stats.angularVelocity[i]) > 0.01f) //Si el input, es contrario a la velocidad angular actual.
                {
                    targetTorque[i] = -stats.angularVelocity[i] * stats.counterTorque;
                }
            }
            else //Si no hay input.
            {
                if (i == 2 && Mathf.Abs(stats.rotation[i]) > 0.1f) //Si la rotacion en el eje z es mayor a 0, estabiliza la nave en ese mismo eje.
                {
                    float rollAngle = Mathf.DeltaAngle(stats.rotation[i], 0f) * Mathf.Deg2Rad; 
                    targetTorque[i] = rollAngle * stats.maxTorque[i] - stats.angularVelocity[i] * stats.counterTorque;
                    targetTorque[i] = Mathf.Clamp(targetTorque[i], -stats.maxTorque[i], stats.maxTorque[i]);
                }
                else //Desacelera el giro cuando no hay ningun input.
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
        rb.AddRelativeTorque(stats.currentTorque, ForceMode.Acceleration); //Aplica la fuerza.

        if (stats.angularVelocity.magnitude > stats.maxAV) //Capa la velocidad angular a la velocidad maxima permitida.
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
    public void LinearControl() //Control de la velocidad de la nave.
    {
        stats.linearVelocity = rb.transform.InverseTransformDirection(rb.linearVelocity);
        Vector3 targetThrust = Vector3.zero;
        if(thrustInput > 0.01f) //Si avanza.
        {
            if (boostInput) //Si se activa el boost canvia la velocidad maxima que puede alcanzar la nave.
            {  
                stats.maxV = Mathf.Lerp(stats.maxV, 10f, Time.fixedDeltaTime);
            }
            else
            {
                stats.maxV = Mathf.Lerp(stats.maxV, 2f, Time.fixedDeltaTime);
            }
            targetThrust = Vector3.forward * stats.maxThrust;
            TrailsOn(true); //Enciende los efectos de propulsion.
        }
        else if (Mathf.Abs(thrustInput) < 0.01f && Mathf.Abs(stats.linearVelocity.magnitude) > 0.1f) //Si no hay input y esta en movimiento.
        {
            targetThrust = -stats.linearVelocity.normalized * stats.counterThrust; //Cambia el sentido de la fuerza.
            TrailsOn(false);
        }
        stats.currentThrust = Vector3.MoveTowards(
            stats.currentThrust,
            targetThrust,
            stats.thrustAcceleration * Time.fixedDeltaTime
        );
        rb.AddRelativeForce(stats.currentThrust, ForceMode.Acceleration); //Aplica la fuerza.

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
