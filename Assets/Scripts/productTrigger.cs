using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class productTrigger : MonoBehaviour
{
    GameObject receiverObject;
    private void Start()
    {
        receiverObject = GameObject.FindWithTag("Player");
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Wall"))
        {
            receiverObject.SendMessage("triggerReset");
        }
        if (other.CompareTag("Target"))
        {
            receiverObject.SendMessage("winReset");
        }
    }
}
