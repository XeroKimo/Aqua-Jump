using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public new Camera camera;

    public Transform trackedObject;
    public Vector3 offset;

    public Rect bounds
    {
        get
        {
            Rect rect = new Rect();
            rect.size = new Vector2(camera.orthographicSize * camera.aspect * 2, camera.orthographicSize * 2);
            rect.center = camera.transform.position;
            return rect;
        }
    }


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void LateUpdate()
    {
        if(trackedObject)
        {
            camera.transform.position = new Vector3(0, trackedObject.position.y, 0) + offset;
        }
    }
}
