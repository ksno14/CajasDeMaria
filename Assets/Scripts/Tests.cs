using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;          
using Unity.MLAgents.Actuators;

public class Tests : Agent         
{
    [SerializeField] float velocidadMovimiento = 1.0f;
    Rigidbody[] rigids;                                    
    public GameObject[] prefabs;   
     public override void OnEpisodeBegin()                                   
    {
        GameObject newObject = Instantiate(prefabs[0],new Vector3(2, 0.70f, 2), Quaternion.identity);
        newObject.transform.parent = transform;
        GameObject newObject2 = Instantiate(prefabs[1], new Vector3(2, 0.70f, 2), Quaternion.identity);
        newObject2.transform.parent = transform;
        GameObject newObject3 = Instantiate(prefabs[2], new Vector3(2, 0.70f, 2), Quaternion.identity);
        newObject3.transform.parent = transform;
        Invoke("LaunchProjectile", 10f);
    }
    public override void CollectObservations(VectorSensor sensor)                
    {   
    
    }

     public override void OnActionReceived(ActionBuffers actionBuffers)          
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


    