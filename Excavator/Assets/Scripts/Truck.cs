using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Truck : MonoBehaviour
{
    public ExcavatorAgent agent;
    public void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Stone"))
        {
            agent.countInTruck++;
            agent.countChangeInTruck++;
            // Debug.Log("enter");
            Debug.Log($"Truck:: Enter: {agent.countChangeInTruck}, Total: {agent.countInTruck}");
        }
    }
    public void OnTriggerExit(Collider collider)
    {
        if (collider.CompareTag("Stone"))
        {
            agent.countInTruck--;
            agent.countChangeInTruck--;
            // Debug.Log("exit");
            Debug.Log($"Truck:: Exit: {agent.countChangeInTruck}, Total: {agent.countInTruck}");
        }
    }
}
