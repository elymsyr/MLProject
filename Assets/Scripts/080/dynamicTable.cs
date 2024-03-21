using UnityEngine;
using Unity.MLAgents.Actuators;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using UnityEngine.U2D;
using System;

public class dynamicTable : Agent
{
    private float[] wallBorders;
    private CreateBoard table;
    private int rows;
    private int columns;
    private float directionFactor = 1.2f;
    private float heightFactor = 0.3f;
    public Transform[,] boxesArray;
    public Transform[,] boxesStartLoc;
    private GameObject product;
    private GameObject target;
    private int actionCount = 0;
    private int actionLimit = 800;
    private int gameCount = 0;
    private int size = 6;
    [Range(0f,10f)] public float MoveSpeed = 8f;
    private Transform[] activeArray;
    private Rigidbody productRigidbody;

    void Awake()
    {
        table = transform.GetComponent<CreateBoard>();
        rows = table.rows;
        columns = table.columns;
        foreach (Transform child in table.transform)
        {
            Destroy(child.gameObject);
        }
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        if (actionLimit < actionCount)
        {
            AddReward(-1f);
            EndEpisode();
        }
        actionCount++;
        int index = 0;
        int movingPartsIndex = 0;

        GetActiveArray();

        foreach (Transform child in boxesArray)
        {
            if (child != null)
            {
                if (Array.IndexOf(activeArray, child) != -1){
                    float newYPosition = child.localPosition.y + actions.ContinuousActions[movingPartsIndex] * MoveSpeed * Time.deltaTime;
                    newYPosition = Mathf.Clamp(newYPosition, 0f, 3.5f);
                    child.localPosition = new Vector3(child.localPosition.x, newYPosition, child.localPosition.z);
                    movingPartsIndex++;
                }
                else
                {
                    int i = index / rows;
                    int j = index % columns;
                    child.transform.localPosition = boxesStartLoc[i, j].transform.localPosition;
                }
                index++;
            }
            else{
                Debug.Log("Null child founded!");
            }
        }

        var directionPoint = Vector3.Dot(productRigidbody.velocity.normalized, (target.transform.localPosition - product.transform.localPosition).normalized);
        float reward = 0.001f * (directionPoint*directionFactor + (product.transform.localPosition.y-40)*heightFactor);
        AddReward(reward);

        // if (showUI)
        // {
        //     updateUI();
        // }
    }

    public override void OnEpisodeBegin()
    {
        CreateEnvironment();
        actionCount = 0;
        ++gameCount;
        activeArray = new Transform[size*size];
        GetActiveArray();
    }    
    
    public void triggerReset(){
        AddReward(-1f);
        ClearEnvionment();
        EndEpisode();
    }
    
    public void winReset(){
        // win++;
        AddReward(1f);
        ClearEnvionment();
        EndEpisode();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        foreach (Transform child in activeArray)
        {
            if (child != null)
            {
                sensor.AddObservation(child.localPosition.y);
            }
            else
            {
                sensor.AddObservation(0);
            }
        }
        var scale = table.productScale;
        sensor.AddObservation(product.transform.localPosition);
        sensor.AddObservation(target.transform.localPosition);
        sensor.AddObservation(targetCloseness());
        sensor.AddObservation(wallBorders[0]);
        sensor.AddObservation(wallBorders[1]);
        sensor.AddObservation(wallBorders[2]);
        sensor.AddObservation(wallBorders[3]);
        sensor.AddObservation(scale);
        // sensor.AddObservation(0);
    }

    private void GetActiveArray()
    {
        int[] centerPoint = FindClosestTransform();

        int startX = centerPoint[0] - size / 2;
        int startY = centerPoint[1] - size / 2;
        int index = 0;
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                if(IsIndexValid(startX + i, startY + j) && boxesArray[startX + i, startY + j] != null){
                    activeArray[index] = boxesArray[startX + i, startY + j];
                }
                else{
                    activeArray[index] = null;
                }
                index++;
            }
        }
    }
    
    private bool IsIndexValid(int rowIndex, int colIndex)
    {
        return rowIndex >= 0 && rowIndex < rows && colIndex >= 0 && colIndex < columns;
    }    
    
    private int[] FindClosestTransform()
    {
        int[] closestPosition = new int[2];
        float closestDistance = Mathf.Infinity;

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                Transform transform = boxesArray[i, j];
                if (transform != null)
                {
                    float distance = GetDistanceToChild(transform);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestPosition[0] = i;
                        closestPosition[1] = j;
                    }
                }
            }
        }

        return closestPosition;
    }
    
    private float targetCloseness()
    {
        return Vector3.Distance(product.transform.localPosition, target.transform.localPosition);
    }
    
    private float GetDistanceToChild(Transform child)
    {
        float distance = Vector3.Distance(gameObject.transform.InverseTransformPoint(new Vector3(child.position.x, 0f, child.position.z)), gameObject.transform.InverseTransformPoint(new Vector3(product.transform.position.x, 0f, product.transform.position.z)));
        return distance;
    }
    
    private void CreateEnvironment(){
        Vector3 boardSize = table.CreateBoxes();
        table.CreateWalls(boardSize);
        wallBorders = table.getBorders;
        boxesArray = table.getPieces;
        boxesStartLoc = boxesArray;
        table.LoadPrefabs();
        product = table.getProduct;
        target = table.getTarget;
        productRigidbody = product.GetComponent<Rigidbody>();
    }

    private void ClearEnvionment(){
        table.ClearEnvironment();
        foreach (Transform child in table.transform)
        {
            Destroy(child.gameObject);
        }
        wallBorders = null;
        boxesArray = null;
        boxesArray = null;
        boxesStartLoc = null;
        activeArray = null;
        productRigidbody = null;

    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        for (int i = 0; i < size*size; i++)
        {
            continuousActions[i] = UnityEngine.Random.Range(-1f, 1f);
        }       
    }

}
