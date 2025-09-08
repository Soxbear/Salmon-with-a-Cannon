using UnityEngine;

public class Managers : MonoBehaviour
{
    public static Managers Singleton
    {
        get
        {
            if (instance == null)
            {

            }
            return instance;
        }
    }

    private static Managers instance;

    public LayermaskReference LayermaskReference;

    private void Awake()
    {
        instance = this;
    }
}
