using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Splines;

public class GameManager : MonoBehaviour
{
    
    public GameStats stats;
    public SplineContainer spline;
    public GameObject teseractPrefab;

    private Transform rings;
    void Start()
    {
        if(SceneManager.GetActiveScene().buildIndex == 1) {
            stats.Reset();
            rings = GameObject.Find("RingsG").transform;
            rings.GetChild(0).GetComponent<RingContoller>().SetOn();
            for (int i = 0; i < rings.childCount; i++)
            {
                RingContoller ringObject = rings.GetChild(i).GetComponent<RingContoller>();
                ringObject.id = i;
            }
            GenerateTeseracts();
            StartCoroutine(Game());
        }    
    }
    public void RingCrossed(int id)
    {
        stats.rings--;
        if (id < rings.childCount - 1)
        {
            rings.GetChild(id + 1).GetComponent<RingContoller>().SetOn();
        }
    }
    public Vector3 ConvertEuler(Vector3 angle)
    {
        for (int i = 0; i < 3; i++)
        {
            if (angle[i] > 180) angle[i] -= 360;
        }

        return angle;
    }
    public void GenerateTeseracts()
    {
        for (int i = 0; i < stats.maxTeseracts; i++)
        {
            float baseT = Mathf.Pow(i / (float)(stats.maxTeseracts - 1), 1.5f);
            float noise = (Mathf.PerlinNoise(i * 0.3f, 0f) - 0.5f) * 0.05f;
            noise = Mathf.Clamp(noise, -1f / stats.maxTeseracts * 0.5f, 1f / stats.maxTeseracts * 0.5f);

            float t = baseT + noise;

            Vector3 position = spline.EvaluatePosition(t);
            Vector3 forward = spline.EvaluateTangent(t);

            GameObject teseract = Instantiate(teseractPrefab, position, Quaternion.LookRotation(forward));
            teseract.transform.parent = transform;
        }
    }
    IEnumerator Game()
    {
        while (stats.time > 0 && !stats.win)
        {
            yield return new WaitForSeconds(1f);
            stats.time--;
            if (stats.rings == 0) {
                stats.win = true;
            }
        }
        stats.score = stats.time * (stats.maxTeseracts - stats.teseracts);
        SceneManager.LoadScene(2);
    }
}
