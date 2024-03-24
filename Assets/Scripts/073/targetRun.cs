using UnityEngine;
using Unity.MLAgents.Actuators;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;

public class AgentRun : Agent
{
    private float[] wallBorders;
    private CreateBoard table;
    private GameObject product;
    [Range(0f,10f)] public float MoveSpeed = 8f;

    void Awake()
    {    
        table = transform.GetComponent<CreateBoard>();
        product = table.getProduct;
        wallBorders = table.getBorders;
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        AddReward(0.1f);
        int x = actionBuffers.DiscreteActions[0]-1;
        int z = actionBuffers.DiscreteActions[1]-1;
        Vector3 newPosition = transform.position + new Vector3(x, 0f, z);
        newPosition.x = Mathf.Clamp(newPosition.x, wallBorders[0], wallBorders[1]);
        newPosition.z = Mathf.Clamp(newPosition.z, wallBorders[2], wallBorders[3]);
        transform.localPosition = newPosition;
    }

    public override void OnEpisodeBegin()
    {
        wallBorders = table.getBorders;
    }

    private void Reset(){
        EndEpisode();
    }


    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(product.transform.localPosition);
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(targetCloseness());
        sensor.AddObservation(wallBorders[0]);
        sensor.AddObservation(wallBorders[1]);
        sensor.AddObservation(wallBorders[2]);
        sensor.AddObservation(wallBorders[3]);
        sensor.AddObservation(table.productScale);
    }
    
    private float targetCloseness()
    {
        return Vector3.Distance(product.transform.localPosition, transform.localPosition);
    }


    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActionsOut = actionsOut.DiscreteActions;
        discreteActionsOut.Clear();
        discreteActionsOut[0] = Random.Range(0, 3);
        discreteActionsOut[1] = Random.Range(0, 3);
    }

}
