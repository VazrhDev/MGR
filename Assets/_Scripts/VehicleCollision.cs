using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleCollision : MonoBehaviour
{
    private VehicleControl controller;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponentInParent<VehicleControl>();
    }

    
    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("On Collision Enter");

        if (collision.transform.root.GetComponent<VehicleControl>())
        {

            collision.transform.root.GetComponent<VehicleControl>().slip2 = Mathf.Clamp(collision.relativeVelocity.magnitude, 0.0f, 10.0f);

            //controller.myRigidbody.angularVelocity = new Vector3(-controller.myRigidbody.angularVelocity.x * 0.5f, controller.myRigidbody.angularVelocity.y * 0.5f, -controller.myRigidbody.angularVelocity.z * 0.5f);
            //controller.myRigidbody.velocity = new Vector3(controller.myRigidbody.velocity.x, controller.myRigidbody.velocity.y * 0.5f, controller.myRigidbody.velocity.z);


        }

        controller.myRigidbody.angularVelocity = new Vector3(-controller.myRigidbody.angularVelocity.x * 0.5f, controller.myRigidbody.angularVelocity.y * 0.5f, -controller.myRigidbody.angularVelocity.z * 0.5f);
        controller.myRigidbody.velocity = new Vector3(controller.myRigidbody.velocity.x, controller.myRigidbody.velocity.y * 0.5f, controller.myRigidbody.velocity.z);


        Debug.LogError("CAR COLLIDERS COLLISION ENTERED");
    }




    void OnCollisionStay(Collision collision)
    {
        Debug.Log("On Collision Stay");

        if (collision.transform.root.GetComponent<VehicleControl>())
            collision.transform.root.GetComponent<VehicleControl>().slip2 = 5.0f;

    }
}
