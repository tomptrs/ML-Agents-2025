using Google.Protobuf.WellKnownTypes;
using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class SearchAgentPush : Agent
{

    [SerializeField]
    private float maxSpeed = 5f;

    [SerializeField]
    private GameObject target;

    [SerializeField]
    private Transform zoneTransform;

    private Transform targetPosition;

    private bool hasCollidedWithEnemy = false;

    private float lastDistanceToTarget=0;



    // Start is called before the first frame update
    void Start()
    {
        targetPosition = target.transform;
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    private void InitializeEnvironment()
    {
        
        float randomX = Random.Range(-4.5f, 4.5f);  
        float randomZ = Random.Range(-4.5f, 4.5f);
        targetPosition.localPosition = new Vector3(randomX, 0.5f, randomZ);
        transform.localPosition = new Vector3(-8, 0.5f, 0);
        lastDistanceToTarget = Vector3.Distance(targetPosition.transform.localPosition, zoneTransform.transform.localPosition); ;
        //target.SetActive(true);
        //hasCollidedWithEnemy = false;
    }

    public override void OnEpisodeBegin()
    {
        InitializeEnvironment();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        
        //sensor.AddObservation(hasCollidedWithEnemy);
        sensor.AddObservation(zoneTransform.localPosition);
        sensor.AddObservation(zoneTransform.localScale);

        sensor.AddObservation(targetPosition.localPosition);
        // Distance to green zone
        Vector3 distanceToZone = zoneTransform.localPosition - targetPosition.localPosition;
        sensor.AddObservation(distanceToZone);

    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        //straf de agent voor niet snel naar zijn doel te gaan met een minimum penalty
       AddReward(-1 / MaxStep);

       var inputX = Mathf.Clamp(actions.ContinuousActions[0], -1f, 1f);
       var inputZ = Mathf.Clamp(actions.ContinuousActions[1], -1f, 1f);  
        
       Vector3 move = new Vector3(inputX, 0, inputZ) * maxSpeed * Time.deltaTime;
        transform.Translate(move);

        // Penalty given each step to encourage agent to finish task quickly.
        //AddReward(-1f / MaxStep);
        if (transform.localPosition.y < -1)
        {
            SetReward(-1f);
            EndEpisode();
        }
        if (targetPosition.localPosition.y < -1)
        {
            SetReward(-1f);
            EndEpisode();
        }
        float distanceToGreenZone = Vector3.Distance(targetPosition.transform.localPosition, zoneTransform.transform.localPosition);
        float epsilon = 0.01f;  // Small tolerance value
        if (distanceToGreenZone < lastDistanceToTarget)
        { 
            float maxDistance = Vector3.Distance(new Vector3(-4.5f, 0.5f, 4.5f), zoneTransform.transform.localPosition);
            //AddReward(0.05f);
            float normalizedValue = (1-Mathf.Clamp((distanceToGreenZone) / (maxDistance), 0f, 1f))/10;
            AddReward(normalizedValue);
        }
        //else if((distanceToGreenZone > lastDistanceToTarget))
        //{
        //    AddReward(-0.05f);
        //}
        lastDistanceToTarget= distanceToGreenZone;
    }  

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;

        continuousActionsOut[0] = Input.GetAxis("Horizontal");
        continuousActionsOut[1] = Input.GetAxis("Vertical");
    }
    public void ScoredGoal()
    {
        AddReward(1f);
        EndEpisode();
    }
     private void OnCollisionEnter(Collision collision)
     {
        /* if (collision.gameObject.CompareTag("Enemy"))
         {
             AddReward(0.3f);
             collision.gameObject.SetActive(false);
             hasCollidedWithEnemy = true;

         }*/
     }

     /*private void OnTriggerEnter(Collider other)
     {
         if (other.gameObject.CompareTag("Target"))
         {
             if (hasCollidedWithEnemy)
             {
                 AddReward(0.7f);
                 EndEpisode();
             }
             else
             {
                 AddReward(-0.1f);

             }

         }

     }*/
}
