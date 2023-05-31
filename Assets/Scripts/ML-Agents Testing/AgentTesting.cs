using System.Collections.Generic;
using System;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using System.Linq;
public class AgentTesting : Agent
{
    public static List<string> collisiones = new List<string>();
    public static Dictionary<string[], Action> combinaciones = new Dictionary<string[], Action>();
    public int targetFPS = 30;
    public Rigidbody[] rigids;
    public Transform[] hijos;
    public GameObject[] prefabs;
    public int[] spawnNumbers;
    public int velocidadMovimiento = 1;

    Vector3[] spawnPositions = {
    new Vector3(0, 0.70f, 2),
    new Vector3(4, 0.70f, 0),
    new Vector3(3, 0.70f, 0),
    new Vector3(2, 0.70f, 2),
    new Vector3(3, 0.70f, 2),
    new Vector3(4, 0.70f, 2),

    new Vector3(0, 0.70f, 0),
    new Vector3(1, 0.70f, 0),
    new Vector3(2, 0.70f, 0),
    new Vector3(3, 0.70f, 0),
    new Vector3(4, 0.70f, 0),
    new Vector3(0, 0.70f, -2),
    new Vector3(1, 0.70f, -2),
    new Vector3(2, 0.70f, -2),
    new Vector3(4, 0.70f, 0),
    new Vector3(4, 0.70f, -2),

    new Vector3(0, 0.70f, -4),
    new Vector3(1, 0.70f, -4),
    new Vector3(2, 0.70f, -4),
    new Vector3(3, 0.70f, -4),
    new Vector3(4, 0.70f, -4),
};

    public override void Initialize()
    {
        Application.targetFrameRate = targetFPS;

        combinaciones.Add(new string[] { "Proton", "Electron", "Neutron" }, reward);

        int spawnIndex = 0;

        for (int i = 0; i < prefabs.Length; i++)
        {
            int spawnCount = spawnNumbers[i];
            GameObject prefab = prefabs[i];

            for (int j = 0; j < spawnCount; j++)
            {
                if (spawnIndex < spawnPositions.Length)
                {
                    Vector3 spawnPosition = spawnPositions[spawnIndex];
                    GameObject clone = Instantiate(prefab, spawnPosition, Quaternion.identity);

                    clone.transform.SetParent(transform);

                    spawnIndex++;
                }
                else
                {
                    Debug.LogWarning("No hay suficeintes posiciones.");
                    break;
                }
            }
        }
        hijos = GetComponentsInChildren<Transform>();
        hijos = System.Array.FindAll(hijos, t => t != transform);
        rigids = GetComponentsInChildren<Rigidbody>();
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
    public override void OnEpisodeBegin()
    {
        hijos[0].transform.position = new Vector3(3, 0.70f, 0);
        hijos[1].transform.position = new Vector3(0, 0.70f, 2);
        hijos[2].transform.position = new Vector3(4, 0.70f, 0);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        foreach (Transform tr in hijos)
        {
            if (tr.gameObject.CompareTag("Electron"))
            {
                Vector3 distancia = (tr.position - transform.position);
                sensor.AddObservation(distancia.normalized);
            } 
        }
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        Vector3 movimiento = new Vector3(actionBuffers.ContinuousActions[0],
                                         0f,
                                         actionBuffers.ContinuousActions[1]);

        rigids[0].AddForce(movimiento * velocidadMovimiento);
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
    public void reward()
    {
        AddReward(0.5f);
        EndEpisode();
    }
}
