using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using System.Linq;
public class Colision : Agent
{
    public static List<string> collisiones = new List<string>();
    public static Dictionary<string[], Action> combinaciones = new Dictionary<string[], Action>();
    public int targetFPS = 30;

    void Start()
    {
        Application.targetFrameRate = targetFPS;
        //combinaciones.Add(new string[] { "Proton", "Electron", "Neutron" }, reward);
        combinaciones.Add(new string[] { "Neutron" }, reward);
        //combinaciones.Add(new string[] { "Electron", "Neutron" }, reward);
    }

    void OnCollisionEnter(Collision collision){
        if(collision.gameObject.name!="Caja"){
        listaAñadir(collision.gameObject.name);
        }
    }

    void OnCollisionExit(Collision collision) {
       if(collision.gameObject.name!="Caja"){
        listaRemover(collision.gameObject.name);
        }
    }
    public void listaAñadir(string nombre)
    {
        if (!collisiones.Contains(nombre))
        {
            collisiones.Add(nombre);
            check();
        }
    }
    public void listaRemover(string nombre)
    {
        collisiones.Remove(nombre);
        check();
    }

    public void reward()
    {
        Debug.Log("Reward");
    }

    private static void check()
    {
        foreach (var combinacion in combinaciones)
        {
            if (combinacion.Key.All(collisiones.Contains))
            {
                combinacion.Value.Invoke();
                break;
            }
        }
    }


    void Update()
    {
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.A))
        {
            imprimir();
        }
    }



    public void imprimir()
    {
        foreach (string elemento in collisiones)
        {
            Debug.Log(elemento);
        }
    }
}