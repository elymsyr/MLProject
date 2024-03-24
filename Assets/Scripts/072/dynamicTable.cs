using UnityEngine;
using Unity.MLAgents.Actuators;
using Unity.MLAgents;
using TMPro;
using Unity.MLAgents.Sensors;
using System;

public class dynamicTable : Agent
{
    [SerializeField] private bool showUI = false;
    [SerializeField] private bool randomTableSize = false;
    [SerializeField] private GameObject text;
    private TextMeshPro ui;
    private float directionPoint = 0;
    private int win = 0;
    private float[] wallBorders;
    private CreateBoard table;
    private int rows;
    private int columns;
    private float directionFactor = 1.3f;
    private float heightFactor = 0.5f;
    private float distanceFactor = 0.11f;
    public Transform[,] boxesArray;
    public Vector3[,] boxesLoc;
    private GameObject product;
    private GameObject target;
    private int actionCount = 0;
    private int actionLimit = 700;
    private int gameCount = -1;
    private int size = 6;
    [Range(0f,10f)] public float MoveSpeed = 8f;
    private Transform[] activeArray;
    private Rigidbody productRigidbody;
    private float startDist; 

    void Awake()
    {
        if (text != null)
        {
            ui = text.GetComponent<TextMeshPro>();
        }        
        table = transform.GetComponent<CreateBoard>();
        rows = table.rows;
        columns = table.columns;
        foreach (Transform child in table.transform)
        {
            Destroy(child.gameObject);
        }
        table.CreateEnv();
        product = table.getProduct;
        target = table.getTarget;        
        if (product == null){Debug.Log("Product NULL");}
        if (target == null){Debug.Log("Target NULL");}
        productRigidbody = product.GetComponent<Rigidbody>();
        boxesArray = table.getPieces;
        rows = table.rows;
        columns = table.columns;
        wallBorders = table.getBorders;
        productCollision productClass = product.GetComponent<productCollision>();
        productClass.Initialize(table.wallsArray[0],table.wallsArray[1],table.wallsArray[2],table.wallsArray[3],target,gameObject);
        boxesLoc = new Vector3[rows,columns];     
        for(int i=0; i<rows;i++){
            for(int j=0;j<columns;j++){
                boxesLoc[i,j]=boxesArray[i,j].transform.localPosition;
            }
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
                    child.transform.localPosition = boxesLoc[i, j];
                }
                index++;
            }
            else{
                Debug.Log("Null child founded!");
            }
        }
        directionPoint = Vector3.Dot(productRigidbody.velocity.normalized, (target.transform.localPosition - product.transform.localPosition).normalized);
        float reward = productRigidbody.velocity.magnitude * 0.001f * (directionPoint*directionFactor + (product.transform.localPosition.y-7.5f)*heightFactor + (startDist-targetCloseness())*distanceFactor);
        AddReward(reward);

        if (showUI)
        {
            updateUI();
        }
    }

    public override void OnEpisodeBegin()
    {
        if(!randomTableSize){
            table.ObjectPos();
        }
        else{
            table.ResetEnv();
            rows = table.rows;
            columns = table.columns;
            wallBorders = null;
            boxesArray = new Transform[rows,columns];
            boxesLoc = new Vector3[rows,columns];
            wallBorders = table.getBorders;
            boxesArray = table.getPieces;
            productCollision productClass = product.GetComponent<productCollision>();
            productClass.Initialize(table.wallsArray[0],table.wallsArray[1],table.wallsArray[2],table.wallsArray[3],target,gameObject);
            for(int i=0; i<rows;i++){
                for(int j=0;j<columns;j++){
                    boxesLoc[i,j]=boxesArray[i,j].transform.localPosition;
                }
            }                     
        }
        product.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
        productRigidbody.velocity = Vector3.zero;
        actionCount = 0;
        gameCount++;
        activeArray = new Transform[size*size];
        GetActiveArray();
        startDist = targetCloseness();
    }
    
    public void triggerReset(){
        AddReward(-1f);
        EndEpisode();
    }
    
    public void winReset(){
        win++;
        AddReward(1f);
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
        sensor.AddObservation(product.transform.localPosition);
        sensor.AddObservation(target.transform.localPosition);
        sensor.AddObservation(targetCloseness());
        sensor.AddObservation(wallBorders[0]);
        sensor.AddObservation(wallBorders[1]);
        sensor.AddObservation(wallBorders[2]);
        sensor.AddObservation(wallBorders[3]);
        sensor.AddObservation(table.productScale);
    }

    private void updateUI()
    {
        ui.text = "Product States\nBoard Size: "+rows+"x"+columns+"\nDirection: "+directionPoint+"\nSpeed: "+productRigidbody.velocity.magnitude+"\nPosition: "+product.transform.localPosition+"\nDistance to Target: "+targetCloseness()+"\nReward: "+GetCumulativeReward()+"\nAction Count: "+actionCount+"\nGame Count: "+gameCount+"\nWin Count: "+win+"\nActive Parts Map: \n"+ActiveMap();
    }

    private string ActiveMap(){
        string arrayString = "";
        for (int i = 0; i < size*size; i++)
        {
            if (activeArray[i] != null){arrayString += "O ";}
            else{arrayString += "X ";}
            if ((i+1)%size == 0){arrayString += "\n";}
        }
        return arrayString;       
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

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        for (int i = 0; i < size*size; i++)
        {
            continuousActions[i] = UnityEngine.Random.Range(-1f, 1f);
        }       
    }

}
