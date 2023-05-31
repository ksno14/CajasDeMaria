using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;           //Import de las librerias de ML-Agents
using Unity.MLAgents.Actuators;

public class Agente : Agent               //clase publica principal, importa el cotenido de la clase Agent perteneciente a las librerias de Ml-Agent
{
    public int targetFPS = 30;
    void Start()
    {
        Application.targetFrameRate = targetFPS;
    }
    [SerializeField] float velocidadMovimiento = 1.0f;     //variable privada visible y editable en el editor de unity   
    public Rigidbody[] rigids;                                    //array donde se guardaran los RigidBody de los gameobjects
    public Transform[] hijos;

    public GameObject[] prefabs;                           //input de prefabs para spawn
    Vector3[] spawnPositions = {
    new Vector3(0, 0.70f, 2),
    new Vector3(0, 0.80f, 2),
    new Vector3(1, 0.70f, 2),
    new Vector3(2, 0.70f, 2),
    new Vector3(3, 0.70f, 2),
    new Vector3(4, 0.70f, 2),

    new Vector3(0, 0.70f, 0),
    new Vector3(1, 0.70f, 0),
    new Vector3(2, 0.70f, 0),
    new Vector3(3, 0.70f, 0),
    new Vector3(4, 0.70f, 0),                                  //division de la caja manualmente (cambiar)

    new Vector3(0, 0.70f, -2),
    new Vector3(1, 0.70f, -2),
    new Vector3(2, 0.70f, -2),
    new Vector3(3, 0.70f, -2),
    new Vector3(4, 0.70f, -2),

    new Vector3(0, 0.70f, -4),
    new Vector3(1, 0.70f, -4),
    new Vector3(2, 0.70f, -4),
    new Vector3(3, 0.70f, -4),
    new Vector3(4, 0.70f, -4),
};
    public int minSpawn = 1;                                    //minimo numero de particulas a spawnear 
    public int maxSpawn = 5;                                    //maximo numero  de particulas a spawnear 

     public override void OnEpisodeBegin()                                   //acciones a realizar cada episodio de la simulacion
    {
        int numeroSpawns = Random.Range(minSpawn, maxSpawn + 1);            //numero random de spawns

        int Index = 0;

        for (int i = 0; i < numeroSpawns; i++)
        {
            for (int j = 0; j < prefabs.Length; j++)                           //spawn de particulas
            {
                Vector3 spawnPosition = spawnPositions[Index];

                GameObject newObject = Instantiate(prefabs[j], transform.position + spawnPosition, Quaternion.identity);
                newObject.transform.parent = transform;

                Index = (Index + 1) % spawnPositions.Length;
            }
        }
        Transform[] hijosypadre = GetComponentsInChildren<Transform>();             
        Transform[] hijos = System.Array.FindAll(hijosypadre, t => t != transform);
        Rigidbody[] rigids = GetComponentsInChildren<Rigidbody>();
        Invoke("LaunchProjectile", 10f);
    }
    public override void CollectObservations(VectorSensor sensor)               //observaciones-informacion que obtiene el agente en cada episodio    
    {   
        List<float> tagsF = new List<float>();
        List<string> tagsS = new List<string>();


        for (int i = 1; i < hijos.Length; i++)
        {
            sensor.AddObservation(hijos[i].position.x);                         //observacion posicion x de los hijos (particulas spawneadas)
            sensor.AddObservation(hijos[i].position.z);                         //observacion posicion z de los hijos (particulas spawneadas)
            tagsS.Add(hijos[i].tag);   
        }

        foreach (string tag in tagsS)                                           //volver tags numeros para poder darlos como observacion
        {
            float valorConvertido = 0f;
            switch (tag)
            {
                case "Proton":
                    valorConvertido = 1f;
                    break;
                case "Electron":
                    valorConvertido = 2f;
                    break;
                case "Neutron":
                    valorConvertido = 3f;
                    break;
                case "Atomo":
                    valorConvertido = 4f;
                    break;
                default:
                    Debug.LogError("Tag no encontrado" + tag);
                    break;
            }
            tagsF.Add(valorConvertido);
        }
        sensor.AddObservation(tagsF);                                           //observacion tags de los hijos (particulas spawneadas)

        for (int i = 1; i < rigids.Length; i++)
        {
            sensor.AddObservation(rigids[i].velocity.x);                        //observacion velocidad en x de los hijos (particulas spawneadas)
            sensor.AddObservation(rigids[i].velocity.z);                        //observacion velocidad en z de los hijos (particulas spawneadas)
        }
    }

     public override void OnActionReceived(ActionBuffers actionBuffers)           //acciones a realizar con la informacion obtenida y procesada por el cerebro
    {
        Vector3 movimiento = new Vector3(actionBuffers.ContinuousActions[0],
                                         0f,
                                         actionBuffers.ContinuousActions[1]);

        for (int i = 1; i < rigids.Length; i++)
        {
            rigids[i].velocity = movimiento * velocidadMovimiento;                 //mover los hijos   
        }
    }
  
    private List<string> collisions = new List<string>();

    public void OnCollisionEnter(Collision collision)
    {
    collisions.Add(collision.gameObject.name);

    if (collisions.Contains("Electron") && collisions.Contains("Proton")){
        Debug.Log("Reward");

    }
    Debug.Log(collisions);
    }


    public void OnCollisionExit(Collision collision)
    {
    collisions.Remove(collision.gameObject.name);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxis("Horizontal");
        continuousActions[1] = Input.GetAxis("Vertical");
    }

     void LaunchProjectile(){
    foreach (string item in collisions){
        {
            Debug.Log(item);
        }
}
}
}


    