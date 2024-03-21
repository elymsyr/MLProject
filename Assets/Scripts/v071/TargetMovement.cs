using UnityEngine;

public class TargetMovement : MonoBehaviour
{
    [SerializeField] private bool isMovementOn = false;
    [SerializeField] [Range(3,10)] public float moveSpeed = 5f;
    private float[] wallBorders = {-43.9f,-22.5f,18.5f,-3.3f};

    void Update()
    {
        if(isMovementOn){
            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");

            Vector3 movement = new Vector3(horizontalInput, 0f, verticalInput) * moveSpeed * Time.deltaTime;

            Vector3 newPosition = transform.position + movement;
            // newPosition.x = Mathf.Clamp(newPosition.x, wallBorders[0], wallBorders[1]);
            // newPosition.z = Mathf.Clamp(newPosition.z, wallBorders[3], wallBorders[2]);

            transform.position = newPosition;
        }

    }
}