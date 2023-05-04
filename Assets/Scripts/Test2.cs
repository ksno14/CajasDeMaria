using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test2 : MonoBehaviour
{
    [SerializeField] float velocidadMovimiento = 3.0f;
    public Transform[] hijosypadre;
    public Rigidbody[] rigids;                                    
    public Transform[] hijos;                                   
    public GameObject[] prefabs;
    void Start()
    {
        GameObject newObject = Instantiate(prefabs[0], new Vector3(0, 0.70f, 2), Quaternion.identity);
        newObject.transform.SetParent(transform);
        GameObject newObject2 = Instantiate(prefabs[1], new Vector3(1, 0.70f, -4), Quaternion.identity);
        newObject2.transform.SetParent(transform);
        hijosypadre = GetComponentsInChildren<Transform>();             
        hijos = System.Array.FindAll(hijosypadre, t => t != transform);
        rigids = GetComponentsInChildren<Rigidbody>();
    }
     void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 movimiento = new Vector3(horizontal, 0f, vertical) * velocidadMovimiento;
        rigids[0].velocity = movimiento;
    }

    void OnCollisionEnter(Collision colision)
    {
        Debug.Log("Reward");
    }

    
}
