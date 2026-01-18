using UnityEngine;

public class RingContoller : MonoBehaviour
{
    public Material on; //Material de aro inactivo.
    public Material off; //Material de aro activo.
    public Material ba; //Material base.
    public int id;
    public bool crossed;
    public bool active;

    private GameManager manager;
    private Vector3 baseScale;
    private Vector3 exitGate;
    private Vector3 enterPosition;
    void Awake()
    {
        exitGate = gameObject.transform.position + transform.forward * 4f;
        gameObject.GetComponent<MeshRenderer>().material = ba;
        baseScale = gameObject.transform.localScale;
        crossed = false;
    }
    void Start()
    {
        manager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    void Update()
    {
        float scaleOffset = Mathf.Sin(Time.time * 2f) * 0.01f;
        gameObject.transform.localScale = baseScale * (1f + scaleOffset);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (active && other.gameObject.name == "Xwing" && !crossed)
        {
            enterPosition = other.transform.position; //En trigger enter, captura la posicion de la nave.
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (active && other.gameObject.name == "Xwing" && !crossed)
        { //Captura la posicion de salida la compara junto a la de entrada si esta mas o menos cerca de la salida del aro.
            if((exitGate - other.transform.position).sqrMagnitude < (exitGate - enterPosition).sqrMagnitude){
                crossed = true;
                manager.RingCrossed(SetOff());
            }
        }
    }
    public void SetOn()
    {
        active = true;
        gameObject.GetComponent<MeshRenderer>().material = on;
    }
    public int SetOff()
    {
        active = false;
        gameObject.GetComponent<MeshRenderer>().material = off;
        return id;
    }
}
