using System.Collections.Generic;
using System;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using System.Linq;
public class Random : Agent
{
    public static List<string> collisiones = new List<string>();
    public static Dictionary<string[], Action> combinaciones = new Dictionary<string[], Action>();
    public int targetFPS = 30;
    public Rigidbody[] rigids;
    public Transform[] hijos;
    public GameObject[] prefabs;
    public GameObject[] prefabsAux;
    public GameObject[] prefabsAux2;
    private GameObject atomoz;
    public Material[] materiales;
    public int velocidadMovimiento = 5;
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
            hijo.gameObject.SetActive(true);
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
        //Application.targetFrameRate = targetFPS;

        Spawner();
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
        GameObject[] objectsToDestroy = GameObject.FindGameObjectsWithTag("Atomo");

        foreach (GameObject obj in objectsToDestroy)
        {
            Destroy(obj);
        }
        SpawnerAux();
        hijos = GetComponentsInChildren<Transform>();
        hijos = System.Array.FindAll(hijos, t => t != transform);
        rigids = GetComponentsInChildren<Rigidbody>();
        if (hijos[0].name == "Proton")
        {
            hijos[0].name = "Up";
            rigids[0].velocity = Vector3.zero;
            rigids[0].angularVelocity = Vector3.zero;
            Renderer renderer0 = hijos[0].GetComponent<Renderer>();
            renderer0.material = materiales[2];
        }

        if (hijos[3].name == "Neutron")
        {
            hijos[3].name = "Up2";
            rigids[3].velocity = Vector3.zero;
            rigids[3].angularVelocity = Vector3.zero;
            Renderer renderer0 = hijos[3].GetComponent<Renderer>();
            renderer0.material = materiales[2];
        }
        combinaciones.Add(new string[] { "Up", "Down", "Up1" }, reward1);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        foreach (Transform hijo in hijos)
        {
            if (hijo.gameObject.activeSelf)
            {
                Vector3 position = hijo.position;

                float normalizedX = (position.x - -4.76f) / (4.76f - -4.76f);
                float normalizedZ = (position.z - -4.76f) / (4.76f - -4.76f);

                normalizedX = (float)Math.Round(normalizedX, 4);
                normalizedZ = (float)Math.Round(normalizedZ, 4);

                sensor.AddObservation(new Vector3(normalizedX, 0f, normalizedZ));
            }
            else
            {
                sensor.AddObservation(new Vector3(0f, 0f, 0f));
            }
        }
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
    private void Switch(int numero)
    {
        if (numero == 1)
        {
            hijos[0].name = "Proton";
            rigids[0].velocity = Vector3.zero;
            rigids[0].angularVelocity = Vector3.zero;
            Renderer renderer0 = hijos[0].GetComponent<Renderer>();
            renderer0.material = materiales[0];
            hijos[1].gameObject.SetActive(false);
            hijos[2].gameObject.SetActive(false);
            for (int i = 0; i < 3; i++)
            {
                Rigidbody tempR = rigids[i];
                Transform tempT = hijos[i];

                rigids[i] = rigids[i + 3];
                hijos[i] = hijos[i + 3];

                rigids[i + 3] = tempR;
                hijos[i + 3] = tempT;
            }
        }
        else if (numero == 2)
        {
            hijos[0].name = "Neutron";
            rigids[0].velocity = Vector3.zero;
            rigids[0].angularVelocity = Vector3.zero;
            Renderer renderer0 = hijos[0].GetComponent<Renderer>();
            renderer0.material = materiales[1];
            hijos[1].gameObject.SetActive(false);
            hijos[2].gameObject.SetActive(false);

            Transform temp = hijos[1];
            hijos[1] = hijos[3];
            hijos[3] = temp;
            temp = hijos[2];
            hijos[2] = hijos[6];
            hijos[6] = temp;

            Rigidbody temp1 = rigids[1];
            rigids[1] = rigids[3];
            rigids[3] = temp1;
            temp1 = rigids[2];
            rigids[2] = rigids[6];
            rigids[6] = temp1;
        }
    }
    public void reward1()
    {
        AddReward(1f);
        collisiones.Clear();
        Debug.Log("reward1");
        Switch(1);
        combinaciones.Remove(combinaciones.FirstOrDefault(x => x.Key.SequenceEqual(new string[] { "Up", "Down", "Up1" })).Key);
        combinaciones.Add(new string[] { "Up2", "Down1", "Down2" }, reward2);
    }
    public void reward2()
    {
        AddReward(1f);
        collisiones.Clear();
        Debug.Log("reward2");
        Switch(2);
        combinaciones.Remove(combinaciones.FirstOrDefault(x => x.Key.SequenceEqual(new string[] { "Up2", "Down1", "Down2" })).Key);
        combinaciones.Add(new string[] { "Proton", "Electron", "Neutron" }, rewardTotal);
    }
    public void rewardTotal()
    {
        SetReward(1f);
        collisiones.Clear();
        Debug.Log("reward3");
        combinaciones.Remove(combinaciones.FirstOrDefault(x => x.Key.SequenceEqual(new string[] { "Proton", "Electron", "Neutron" })).Key);
        hijos[0].gameObject.SetActive(false);
        hijos[1].gameObject.SetActive(false);
        hijos[2].gameObject.SetActive(false);
        Vector3 posicionhijotemp = hijos[0].position;
        if (posicionhijotemp.x < -4f)
        {
            posicionhijotemp.x = -4f;
        }
        if (posicionhijotemp.x > 4.6f)
        {
            posicionhijotemp.x = 4.6f;
        }
        if (posicionhijotemp.z < -4.3f)
        {
            posicionhijotemp.z = -4.3f;
        }
        if (posicionhijotemp.z > 4.3f)
        {
            posicionhijotemp.z = 4.3f;
        }
        posicionhijotemp.y = 6f;
        atomoz = Instantiate(prefabsAux2[0], posicionhijotemp, Quaternion.identity);
        Invoke("Fin", 2f);
    }

    public void Fin()
    {
        Destroy(atomoz);
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
                    AddReward(-0.05f);
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

