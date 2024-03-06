using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class PlayerMovement : Agent
{
    [SerializeField] private Transform target;

    public float speed = 10f;
    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    void Update()
    {
        Debug.Log((Vector3)transform.position);
        Debug.Log((Vector3)target.position);
        float yMove = Input.GetAxisRaw("Vertical");
        rb.velocity = new Vector3(0f, yMove, 0f) * speed;
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation((Vector3)transform.position);
        sensor.AddObservation((Vector3)target.position);

    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveY = actions.ContinuousActions[0];
        float movementSpeed = 5f;
        transform.localPosition += new Vector3(0f, moveY) * Time.deltaTime * movementSpeed;
    }

        public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxisRaw("Vertical");
    }
}
