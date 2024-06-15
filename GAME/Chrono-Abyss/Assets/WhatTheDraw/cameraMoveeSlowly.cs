using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraMoveSlowly : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.position -= new Vector3(0.01f, 0, 0);
    }
}
