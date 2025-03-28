using UnityEngine;

public class HUD : MonoBehaviour
{
    public static HUD Singleton;

    void Awake()
    {
        Singleton = this;
    }
}
