using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraMoveeSlowly : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        gameObject.transform.position -= new Vector3(0.01f, 0, 0);
    }
}
