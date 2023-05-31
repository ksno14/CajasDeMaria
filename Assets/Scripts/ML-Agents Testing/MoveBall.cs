using UnityEngine;

public class MoveBall : MonoBehaviour
{
    public int targetFPS = 30;
    void Start()
    {
        Application.targetFrameRate = targetFPS;
    }
    public float velocidad = 5f;
    
    void Update()
    {
        float movimientoHorizontal = Input.GetAxis("Horizontal");
        float movimientoVertical = Input.GetAxis("Vertical");
        
        Vector3 movimiento = new Vector3(movimientoHorizontal, 0f, movimientoVertical) * velocidad * Time.deltaTime;
        
        transform.Translate(movimiento);
    }
}