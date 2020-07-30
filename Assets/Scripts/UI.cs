using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    public GameObject vision;
    public GameObject slider;
    bool showRays;

    public GameObject linesFolder;

    vision visionScript;
    visionCorners visionCornersScript;

    void Start()
    {
        visionScript = vision.GetComponent<vision>();
        visionCornersScript = vision.GetComponent<visionCorners>();

        showRays = false;
    }

    public void toggleClicked()
    {
        if(!showRays)
        {
            showRays = true;
            visionScript.drawRays = true;
            visionCornersScript.drawRays = true;
        }
        else
        {
            showRays = false;
            visionScript.drawRays = false;
            visionCornersScript.drawRays = false;
        }
    }

    public void sliderChanged()
    {
        for (int i = 0; i < linesFolder.transform.childCount; i++)
        {
            Destroy(linesFolder.transform.GetChild(i).gameObject);
        }

       if (slider.GetComponent<Slider>().value == 1)
       {
            visionScript.enabled = true;
            visionCornersScript.enabled = false;
       }
       else
       {
            visionScript.enabled = false;
            visionCornersScript.enabled = true;
       }
    }
}
