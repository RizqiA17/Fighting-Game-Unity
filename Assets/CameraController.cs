using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform player1;
    [SerializeField] private Transform player2;
    private Camera mainCamera;
    private float defaultCamSize;
    private float camSize;
    private float playerDistance;
    private float xPosition;
    private float maxXDistance;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = GetComponent<Camera>();
        defaultCamSize = mainCamera.orthographicSize;
        maxXDistance = mainCamera.aspect * 9f;
    }
    // Update is called once per frame
    void Update()
    {
        CameraAdjustment();
    }

    void CameraAdjustment()
    {
        // Set Camera Size
        mainCamera.orthographicSize = defaultCamSize + camSize;

        // Set Camera Position from Player Position
        playerDistance = Vector2.Distance(player1.position, player2.position);
        xPosition = playerDistance / 2;

        if (player1.position.x < player2.position.x)
        {
            transform.position = new Vector3(player1.position.x + xPosition, camSize, transform.position.z);
        }
        else
        {
            transform.position = new Vector3(player2.position.x + xPosition, camSize, transform.position.z);
        }

        if (playerDistance > maxXDistance)
        {
            camSize = (playerDistance - maxXDistance) / 3;
        }
        else if (playerDistance <= maxXDistance)
        {
            camSize = 0;
        }
    }
}
