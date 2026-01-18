using UnityEngine;

public class TeseractController : MonoBehaviour
{
    public float oscilationSpeed = 2f;
    public float range = 0.05f;
    public GameObject hitPrefab;

    private Transform spaceshipTr;
    private Vector3 defaultPosition;
    private GameManager manager;
    void Start()
    {
        manager = GameObject.Find("GameManager").GetComponent<GameManager>();
        defaultPosition = transform.position; 
        spaceshipTr = GameObject.Find("Xwing").transform;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Xwing")
        {
            GameObject crystals = Instantiate(hitPrefab, transform.position, Quaternion.identity);
            gameObject.SetActive(false);
            manager.stats.teseracts--;
            Destroy(gameObject, 1.5f);
            Destroy(crystals, 1.5f);
        }
    }
    void Update()
    {
        float y = Mathf.Sin(Time.time * oscilationSpeed) * range;
        Vector3 spaceshipPosition = spaceshipTr.position; 
        transform.position = defaultPosition + new Vector3(0, y,0); //Oscilacion arriba y abajo.
        transform.Rotate(0, 0, 200f / Mathf.Max((transform.position - spaceshipPosition).magnitude, 0.1f) * Time.deltaTime); //Rotacion dependiendo de la distancia de la a la nave.
    }

}
