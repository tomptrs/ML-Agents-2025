using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class SearchAgent : Agent
{

    [SerializeField]
    private float maxSpeed = 5f;

    [SerializeField]
    private Transform targetPosition;

   

    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.localPosition.y < -1)
        {
           // SetReward(-1f);
            EndEpisode();
        }
    }

    private void InitializeEnvironment()
    {
        
        float randomX = Random.Range(-4.5f, 4.5f);  
        float randomZ = Random.Range(-4.5f, 4.5f);
        targetPosition.localPosition = new Vector3(randomX, 0.5f, randomZ);
        transform.localPosition = new Vector3(0, 0.5f, 0);
    }

    public override void OnEpisodeBegin()
    {
        InitializeEnvironment();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(targetPosition.localPosition);

    }

    public override void OnActionReceived(ActionBuffers actions)
    {
       var inputX = Mathf.Clamp(actions.ContinuousActions[0], -1f, 1f);
       var inputZ = Mathf.Clamp(actions.ContinuousActions[1], -1f, 1f);  
        
       Vector3 move = new Vector3(inputX, 0, inputZ) * maxSpeed * Time.deltaTime;
        transform.Translate(move);
    }  

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;

        continuousActionsOut[0] = Input.GetAxis("Horizontal");
        continuousActionsOut[1] = Input.GetAxis("Vertical");
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            SetReward(1f);
            EndEpisode();
        }
    }
}
