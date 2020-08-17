using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public GameObject player;
    public float cameraHeight = 50.0f;
    public float cameraDistance = 10.0f;

    void Update()
    {
        Vector3 pos = player.transform.position;
        pos.y += cameraHeight;
        pos.z += -cameraDistance;
        transform.position = pos;
    }
}