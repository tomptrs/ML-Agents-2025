using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoredGoal : MonoBehaviour
{
    // Start is called before the first frame update
    public SearchAgentPush agent;  //


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Zone"))
        {
            agent.ScoredGoal();
            print("collide");
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        // Touched goal.
        if (other.gameObject.CompareTag("Zone"))
        {
            agent.ScoredGoal();
            print("trigger");
        }

    }
}
