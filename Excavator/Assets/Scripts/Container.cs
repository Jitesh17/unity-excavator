using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Container : MonoBehaviour
{
    public ExcavatorAgent agent;
    public void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Stone"))
        {
            agent.countInContainer++;
            // agent.countChangeInContainer++;
            // Debug.Log($"Container enter: {agent.countInContainer}");
        }
    }
    public void OnTriggerExit(Collider collider)
    {
        if (collider.CompareTag("Stone"))
        {
            agent.countInContainer--;
            // agent.countChangeInContainer--;
            // Debug.Log($"Container exit: {agent.countInContainer}");
        }
    }
}
