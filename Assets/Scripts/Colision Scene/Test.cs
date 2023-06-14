using UnityEngine;

public class Test : MonoBehaviour
{
    public int targetFPS = 30;
    void Start()
    {
        Application.targetFrameRate = targetFPS;
    }
    private void Update()
    {
        Transform hijo1 = transform.GetChild(0);
        Transform hijo2 = transform.GetChild(1);

        if (hijo1.GetComponent<Collider>().bounds.Intersects(hijo2.GetComponent<Collider>().bounds))
        {
            Debug.Log("Colisión detectada entre hijo1 y hijo2");
        }
    }
}
