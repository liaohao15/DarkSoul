using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TesterJoystick : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //print(Input.GetAxis("Horizontal"));
        //print(Input.GetAxis("Vertical"));
        //print("Jup:" + Input.GetAxis("Jup"));
        //print("Jright:" + Input.GetAxis("Jright"));
        //print("Dright:" + Input.GetAxis("Dright"));
        //print("btn3:" + Input.GetButton("btn3"));
        //print("padV:" + Input.GetAxis("padV"));
        print("RT:" + Input.GetButton("RT"));
    }
}
