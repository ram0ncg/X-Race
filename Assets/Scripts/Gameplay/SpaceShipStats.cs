using UnityEngine;

[CreateAssetMenu(fileName = "SpaceShipStats", menuName = "Scriptable Objects/SpaceShipStats")]
public class SpaceShipStats : ScriptableObject
{
    [Header("Properties")]
    public float mass;
    public float maxV;
    public float maxAV;

    [Header("Trhust")]
    public float maxThrust;
    public float counterThrust;
    public float thrustAcceleration;

    [Header("Rotation")]
    public Vector3 maxTorque;
    public float counterTorque;
    public float torqueAcceleration;
    

    [Header("Telemetry")]
    public Vector3 currentThrust;
    public Vector3 currentTorque;
    public Vector3 linearVelocity;
    public Vector3 angularVelocity;
    public Vector3 rotation;
    public void Spaceship_1()
    {
        mass = 6f;
        maxV = 0f;
        maxAV = 0.2f;

        maxThrust = 80f;
        counterThrust = 8f;
        thrustAcceleration = 90f;

        maxTorque = new Vector3(
            45f,  // pitch
            60f,  // yaw
            45f   // roll
        );
        counterTorque = 25f;
        torqueAcceleration = 100f;

        currentThrust = Vector3.zero;
        currentTorque = Vector3.zero;
        linearVelocity = Vector3.zero;
        angularVelocity = Vector3.zero;
        rotation = Vector3.zero;
    }
    public void Spaceship_2()
    {

    }
    public void Spaceship_3()
    {

    }
}
