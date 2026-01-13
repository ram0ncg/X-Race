using UnityEngine;

[CreateAssetMenu(fileName = "GameStats", menuName = "Scriptable Objects/GameStats")]
public class GameStats : ScriptableObject
{
    public bool win;
    public int maxTeseracts;
    public int teseracts;
    public float maxTime;
    public float time;
    public int maxRings;
    public int rings;

    public float score;
    public void Reset()
    {
        score = 0f;
        win = false;
        maxTeseracts = 50;
        teseracts = maxTeseracts;
        maxTime = 100f;
        time = maxTime;
        maxRings = 10;
        rings = maxRings;
    }
}
