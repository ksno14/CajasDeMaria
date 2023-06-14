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
    public int velocidadMovimiento = 7;

    public void Spawner()
    {
        foreach (GameObject prefab in prefabs)
        {
            GameObject spawnedObject = Instantiate(prefab);
            spawnedObject.name = prefab.name;
            spawnedObject.transform.SetParent(transform);
        }
    }
    public void SpawnerAux()
    {
        foreach (Transform hijo in hijos)
        {
            Vector3 randomPosition = new Vector3(
                UnityEngine.Random.Range(4.5f,13f),
                (1.3f),
                UnityEngine.Random.Range(-7, 1.5f)
            );
            hijo.position = randomPosition;
        }

        foreach (Rigidbody rigid in rigids)
        {
            rigid.velocity = Vector3.zero;
            rigid.angularVelocity = Vector3.zero;
        }
    }
    public override void Initialize()
    {
        Application.targetFrameRate = targetFPS;

        Spawner();
        combinaciones.Add(new string[] { "Proton", "Electron", "Neutron" }, reward);
        combinaciones.Add(new string[] { "Electron", "Neutron" }, reward);
        hijos = GetComponentsInChildren<Transform>();
        hijos = System.Array.FindAll(hijos, t => t != transform);
        rigids = GetComponentsInChildren<Rigidbody>();
        collisiones.Add("Electron");
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name != "Suelo" && collision.gameObject.name != "Bordes")
        {
            listaAñadir(collision.gameObject.name);
        }
        else if (collision.gameObject.name == "Bordes")
        {
            AddReward(-0.1f);
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.name != "Suelo" && collision.gameObject.name != "Bordes")
        {
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
    public override void OnEpisodeBegin()
    {
        SpawnerAux();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(hijos[0].position);
        sensor.AddObservation(hijos[2].position);
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
        AddReward(1f);
        EndEpisode();
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        float horizontalAxis = Input.GetAxis("Horizontal");
        float verticalAxis = Input.GetAxis("Vertical");

        var actions = actionsOut.ContinuousActions;
        actions[0] = horizontalAxis;
        actions[1] = verticalAxis;
    }
}
