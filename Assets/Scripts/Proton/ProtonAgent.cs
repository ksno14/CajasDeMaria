using System.Collections.Generic;
using System;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using System.Linq;
public class ProtonAgent : Agent
{
    public static List<string> collisiones = new List<string>();
    public static Dictionary<string[], Action> combinaciones = new Dictionary<string[], Action>();
    public int targetFPS = 30;
    public Rigidbody[] rigids;
    public Transform[] hijos;
    public GameObject[] prefabs;
    public GameObject[] prefabsAux;
    public int velocidadMovimiento = 10;
    private bool enColision12 = false;
    private bool enColision13 = false;
    private bool enColision23 = false;

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
                UnityEngine.Random.Range(-4.2f, 4.2f),
                (1.2f),
                UnityEngine.Random.Range(-4.2f, 4.2f)
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
        combinaciones.Add(new string[] { "Up", "Down", "Up1" }, reward);
        combinaciones.Add(new string[] { "Down", "Down1", "Up" }, reward);
        hijos = GetComponentsInChildren<Transform>();
        hijos = System.Array.FindAll(hijos, t => t != transform);
        rigids = GetComponentsInChildren<Rigidbody>();
    }
    public static void listaAñadir(string nombre)
    {
        collisiones.Add(nombre);
        check();
    }
    public static void listaRemover(string nombre)
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
        sensor.AddObservation(hijos[1].position);
        sensor.AddObservation(hijos[2].position);
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        Vector3 movimiento0 = new Vector3(actionBuffers.ContinuousActions[0],
                                         0f,
                                         actionBuffers.ContinuousActions[1]);

        rigids[0].AddForce(movimiento0 * velocidadMovimiento);

        Vector3 movimiento1 = new Vector3(actionBuffers.ContinuousActions[2],
                                         0f,
                                         actionBuffers.ContinuousActions[3]);

        rigids[1].AddForce(movimiento1 * velocidadMovimiento);

        Vector3 movimiento2 = new Vector3(actionBuffers.ContinuousActions[4],
                                         0f,
                                         actionBuffers.ContinuousActions[5]);

        rigids[2].AddForce(movimiento2 * velocidadMovimiento);
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
        SetReward(1f);
        EndEpisode();
    }
    private void Update()
    {
        VerificarIntersecciones();
        bool colision12 = hijos[0].GetComponent<Collider>().bounds.Intersects(hijos[1].GetComponent<Collider>().bounds);
        bool colision13 = hijos[0].GetComponent<Collider>().bounds.Intersects(hijos[2].GetComponent<Collider>().bounds);
        bool colision23 = hijos[1].GetComponent<Collider>().bounds.Intersects(hijos[2].GetComponent<Collider>().bounds);
        if (colision12)
        {
            if (!enColision12)
            {
                enColision12 = true;
                OnCollisionEnterSimulation(hijos[0].gameObject, hijos[1].gameObject);
            }
        }
        else
        {
            if (enColision12)
            {
                enColision12 = false;
                OnCollisionExitSimulation(hijos[0].gameObject, hijos[1].gameObject);
            }
        }

        if (colision13)
        {
            if (!enColision13)
            {
                enColision13 = true;
                OnCollisionEnterSimulation(hijos[0].gameObject, hijos[2].gameObject);
            }
        }
        else
        {
            if (enColision13)
            {
                enColision13 = false;
                OnCollisionExitSimulation(hijos[0].gameObject, hijos[2].gameObject);
            }
        }

        if (colision23)
        {
            if (!enColision23)
            {
                enColision23 = true;
                OnCollisionEnterSimulation(hijos[1].gameObject, hijos[2].gameObject);
            }
        }
        else
        {
            if (enColision23)
            {
                enColision23 = false;
                OnCollisionExitSimulation(hijos[1].gameObject, hijos[2].gameObject);
            }
        }


    }

    void VerificarIntersecciones()
    {
        foreach (var prefab in prefabsAux)
        {
            var prefabCollider = prefab.GetComponent<Collider>();
            var prefabBounds = prefabCollider.bounds;

            foreach (var hijo in hijos)
            {
                var hijoCollider = hijo.GetComponent<Collider>();
                var hijoBounds = hijoCollider.bounds;

                if (prefabBounds.Intersects(hijoBounds))
                {
                    AddReward(-0.1f);
                }
            }
        }
    }

    private void OnCollisionEnterSimulation(GameObject objeto1, GameObject objeto2)
    {
        listaAñadir(objeto1.name);
        listaAñadir(objeto2.name);
    }

    private void OnCollisionExitSimulation(GameObject objeto1, GameObject objeto2)
    {
        listaRemover(objeto1.name);
        listaRemover(objeto2.name);
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

