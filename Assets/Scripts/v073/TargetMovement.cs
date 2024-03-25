using UnityEngine;
public class TargetMovement2 : MonoBehaviour
{
    private bool isMovementOn = false;
    private CreateBoard2 table;
    private float[] wallBorders;
    private float X = 1;
    private float Z = 1;
    private GameObject product;
    private bool startrun = false;

    private void AwakeMe(){
        table = transform.parent.GetComponent<CreateBoard2>();
        product = table.getProduct;
    }

    void Update()
    {
        if (isMovementOn && startrun)
        {
            float distance = 0;
            if(product!=null){distance = Vector3.Distance(product.transform.localPosition, transform.localPosition);}
            if (Input.anyKey)
            {
                float horizontalInput = Input.GetAxis("Horizontal");
                float verticalInput = Input.GetAxis("Vertical");
                float newXPosition = transform.localPosition.x + horizontalInput * table.TargetMoveSpeed * Time.deltaTime;
                newXPosition = Mathf.Clamp(newXPosition, wallBorders[1], wallBorders[0]);                
                float newZPosition = transform.localPosition.z + verticalInput * table.TargetMoveSpeed * Time.deltaTime;
                newZPosition = Mathf.Clamp(newZPosition, wallBorders[3], wallBorders[2]);
                transform.localPosition = new Vector3(newXPosition, transform.localPosition.y, newZPosition);
            }
            else
            {
                float horizontalInput = Random.Range(0f, 2f);
                float verticalInput = Random.Range(0f, 2f);
                float newXPosition = transform.localPosition.x + horizontalInput * table.TargetMoveSpeed * Time.deltaTime * X;
                if(newXPosition > wallBorders[0]-5f || newXPosition < wallBorders[1]+5f){X=-X;}
                float newZPosition = transform.localPosition.z + verticalInput * table.TargetMoveSpeed * Time.deltaTime * Z;
                if(newZPosition > wallBorders[2]-5f || newZPosition < wallBorders[3]+5f){Z=-Z;}
                transform.localPosition = new Vector3(newXPosition, transform.localPosition.y, newZPosition);
                if (Vector3.Distance(product.transform.localPosition, transform.localPosition) < distance){
                    float randomX = Random.Range(0f, 3f);
                    float randomZ = Random.Range(0f, 3f);
                    if (randomX < 0.2f){X = -X;}
                    if (randomZ < 0.2f){Z = -Z;}
                }
            }
        }
    }

    private void GetBorders(){
        wallBorders = table.getBorders;
        startrun = true;
        isMovementOn = table.TargetRun;
    }
}