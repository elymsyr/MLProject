using UnityEngine;

public class TargetController : MonoBehaviour
{
    private CreateBoard table;
    private float[] wallBorders;
    public float moveSpeed = 5f;
    private float horizontalInput;
    private float verticalInput;

    void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        Vector3 movement = new Vector3(horizontalInput, 0f, verticalInput) * moveSpeed * Time.deltaTime;
        transform.Translate(movement, Space.World);
    }

    void Awake(){
        table = transform.GetComponent<CreateBoard>();
        
    }

    private void GetBorders(){
        wallBorders = table.getBorders;
        Run();
    }

    private void Run(){
        if(!Input.anyKey){
            horizontalInput = Random.Range(-1,1);
            verticalInput = Random.Range(-1,1);
            Vector3 movement = new Vector3(horizontalInput, 0f, verticalInput) * moveSpeed * Time.deltaTime;
            transform.Translate(movement, Space.World);            
        }

    }
}