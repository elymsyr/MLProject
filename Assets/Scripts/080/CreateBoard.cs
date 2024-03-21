using UnityEngine;

public class CreateBoard : MonoBehaviour
{
    [SerializeField] [Range(20,50)] public int rows = 25;
    [SerializeField] [Range(20,50)] public int columns = 25;
    private float gap = 0.2f;
    public Material boxMaterial;
    public Material coverMaterial;
    public Material wallMaterial;
    private GameObject pieces;
    private GameObject[] wallsArray;
    private Transform[,] boxesArray;
    private float[] wallBorders;
    public GameObject[] getWalls => wallsArray;
    public float[] getBorders => wallBorders ; 
    public Transform[,] getPieces => boxesArray;
    [SerializeField] private GameObject productPrefab;
    private GameObject product;
    public GameObject getProduct => product;
    [SerializeField] private GameObject targetPrefab;
    private GameObject target;
    public GameObject getTarget => target;
    private float scale = 4.3f;
    public float productScale => scale;

    public Vector3 CreateBoxes()
    {

        // Pieces
        pieces = new GameObject("Pieces");
        pieces.transform.parent = transform;    

        Vector3 boxSize = new Vector3(1, 4, 1);
        float totalWidth = rows * (boxSize.x + gap);
        float totalHeight = columns * (boxSize.z + gap);

        Vector3 startPos = transform.position - new Vector3(totalWidth / 2f, 0f, totalHeight / 2f);
        Vector3 boardSize = new Vector3(totalWidth, boxSize.y, totalHeight);

        boxesArray = new Transform[rows, columns];

        for (int x = 0; x < rows; x++)
        {
            for (int z = 0; z < columns; z++)
            {
                Vector3 position = startPos + new Vector3(x * (boxSize.x + gap), 0f, z * (boxSize.z + gap));
                GameObject box = GameObject.CreatePrimitive(PrimitiveType.Cube);
                box.name = "Box";
                box.transform.position = position;
                box.transform.localScale = boxSize;
                box.transform.parent = pieces.transform;

                if (boxMaterial != null)
                {
                    Renderer renderer = box.GetComponent<Renderer>();
                    if (renderer != null)
                    {
                        renderer.material = boxMaterial;
                    }
                }

                boxesArray[x, z] = box.transform;
            }
        }

        Vector3 coverSize = new Vector3(totalWidth+1f, 4, totalHeight+1f);
        GameObject cover = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cover.name = "Board";
        cover.transform.parent = transform;
        cover.transform.position = startPos + new Vector3(totalWidth / 2f, -0.3f, totalHeight / 2f);
        cover.transform.localScale = coverSize;
        Vector3 boardPosition = cover.transform.localPosition;
        boardPosition.x -= gap*3;
        boardPosition.z -= gap*3;
        cover.transform.localPosition = boardPosition;

        if (coverMaterial != null)
        {
            Renderer coverRenderer = cover.GetComponent<Renderer>();
            if (coverRenderer != null)
            {
                coverRenderer.material = coverMaterial;
            }
        }

        return boardSize;
    }

    public void CreateWalls(Vector3 boardSize)
    {
        wallsArray = new GameObject[4];
        wallBorders = new float[4];
        float wallThickness = 0.2f;
        float wallHeight = 10f;

        Vector3[] wallPositions = {
            new Vector3(boardSize.x / 2f, wallHeight / 2f, 0f),
            new Vector3(-boardSize.x / 2f, wallHeight / 2f, 0f),
            new Vector3(0f, wallHeight / 2f, boardSize.z / 2f),
            new Vector3(0f, wallHeight / 2f, -boardSize.z / 2f)
        };

        Quaternion[] wallRotations = {
            Quaternion.Euler(0f, 0f, 0f),
            Quaternion.Euler(0f, 0f, 0f),
            Quaternion.Euler(0f, 90f, 0f),
            Quaternion.Euler(0f, 90f, 0f)
        };

        for (int i = 0; i < wallPositions.Length; i++)
        {
            Vector3 position = wallPositions[i];
            Quaternion rotation = wallRotations[i];

            GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wall.transform.parent = transform;
            Collider wallCollider = wall.GetComponent<BoxCollider>();
            wallCollider.isTrigger = true;
            wall.transform.localPosition = position;
            wall.transform.localScale = new Vector3(wallThickness, wallHeight, boardSize.z+2.5f);
            wall.transform.rotation = rotation;
            wall.name = "Wall"+(i+1);
            Vector3 wallPosition = wall.transform.localPosition;
            if(i==0){
                wallPosition.x += 1f;
                wallPosition.z -= 0.5f;
            }
            else if(i==1){
                wallPosition.x -= 2f;
                wallPosition.z -= 0.5f;
            }
            else if(i==2){
                wallPosition.z += 1f;
                wallPosition.x -= 0.5f;
                
            }
            else{
                wallPosition.z -= 2f;
                wallPosition.x -= 0.5f;
            }
            wall.transform.localPosition = wallPosition;
            if (i > 1){wallBorders[i] = wall.transform.localPosition.z;}
            else{wallBorders[i] = wall.transform.localPosition.x;}
            if (coverMaterial != null)
            {
                Renderer renderer = wall.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material = wallMaterial;
                }
            }
            wallsArray[i] = wall;
           
        }

    }

    public void LoadPrefabs(){
        GameObject product = Instantiate(productPrefab);

        product.transform.parent = transform;
        product.name = "Product";
        product.transform.localRotation = Quaternion.identity;

        GameObject target = Instantiate(targetPrefab);
        target.transform.parent = transform;
        target.name = "Target";
        target.transform.localRotation = Quaternion.identity;

        Vector3 target_start;
        Vector3 product_start;
        do {
            target_start = randomPos();
            product_start = randomPos();
        } while (Vector3.Distance(target_start, product_start) < 10f);
      
        target.transform.localPosition = target_start;
        product.transform.localPosition = product_start;

        productCollision productClass = product.AddComponent<productCollision>();
        productClass.Initialize(wallsArray[0],wallsArray[1],wallsArray[2],wallsArray[3],target,gameObject);
    }

    private Vector3 randomPos(){
        return new Vector3(Random.Range(wallBorders[0]-(scale/2)-0.5f, wallBorders[1]+(scale/2)+0.5f), 10f, Random.Range(wallBorders[2]-(scale/2)-0.5f, wallBorders[3]+(scale/2)+0.5f));
    }    

    public void ClearEnvironment()
    {
        foreach (Transform child in boxesArray)
        {
            Destroy(child.gameObject);
        }
        foreach (GameObject child in wallsArray)
        {
            Destroy(child.gameObject);
        }
        wallsArray = null;
        wallBorders = null;
        boxesArray = null;
        target = null;
        pieces = null;
    }
}
