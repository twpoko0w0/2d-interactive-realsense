using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEffectController : MonoBehaviour
{

    private float runSpeed = 0.3f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position += new Vector3(runSpeed, 0, 0);
        if (this.transform.position.x > 5.37f) {
            Destroy(this.gameObject);
        }
    }
}
