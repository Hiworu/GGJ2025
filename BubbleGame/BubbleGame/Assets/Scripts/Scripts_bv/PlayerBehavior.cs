using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehavior : MonoBehaviour
{

    private new Camera _camera;
    [SerializeField] private int ammo = 0;
    
    private void Update()
    {
        _camera = Camera.main;
        
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 playerDirection = _camera.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(camera.transform.position, playerDirection, out hit))
            {
                var target = hit.collider.gameObject;
                //if (target == bubble || topping)
                //{DragObject()}
                
                //if (target == straw)
                //{SelectStraw();}
                //if (ammo > 0)
                //{ShootBoba()}
            }
        }

        //drop
        if (Input.GetMouseButtonUp(0))
        {
            //DropObject()
        }

        //charge straw
        if (Input.GetMouseButtonDown(1))
        {
            ammo++;
            if (ammo > 1)
            {
                Debug.Log("Max Ammo");
                return;
            }
        }
    }

    private void ShootBoba()
    {
        //instantiates a bubble and shoots it in front of player
        //if hits a gameObject CompareTag("seagull") invoke?. action
        ammo--;
        
    }
}
