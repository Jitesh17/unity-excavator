// using System.Collections;
// using System.Collections.Generic;
using UnityEngine;

public class Bucket : MonoBehaviour
{
    public ExcavatorAgent agent;
    public void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Stone"))
        {
            agent.countInBucket++;
            agent.countChangeInBucket++;
            // Debug.Log($"Bucket enter: {agent.countInBucket}");
        }
    }
    public void OnTriggerExit(Collider collider)
    {
        if (collider.CompareTag("Stone"))
        {
            agent.countInBucket--;
            agent.countChangeInBucket--;
            // Debug.Log($"Bucket exit: {agent.countInBucket}");
        }
    }
}
