using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    float moveSpeed = 2f;

    // Start is called before the first frame update
    void Start()
    {
        transform.rotation = Quaternion.Euler(60, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift))
            moveSpeed = 8;
        else
            moveSpeed = 4;

        float vertInput = Input.GetAxis("Vertical");
        float horizInput = Input.GetAxis("Horizontal");

        transform.Translate(new Vector3(0, 2, 1).normalized * Time.deltaTime * vertInput * moveSpeed);
        transform.Translate(Vector3.right * Time.deltaTime * horizInput * moveSpeed);
    }
}
