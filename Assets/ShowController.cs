using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowController : MonoBehaviour
{
    Camera cam;
    public Text text;
    Controller ControllerObserve;
    void Start()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(1))
        {
            var pos = Input.mousePosition;
            Ray ray = cam.ScreenPointToRay(pos);
            if(Physics.Raycast(ray, out var info))
            {


                if (info.collider.gameObject.TryGetComponent<Controller>(out ControllerObserve))
                    StartCoroutine(WriteText());

            }
            
            
        }
    }

    private IEnumerator WriteText()
    {
        while(true)
        {
            yield return new WaitForSeconds(0.1f);
            text.text = ControllerObserve.Observe();
        }
    }
}
