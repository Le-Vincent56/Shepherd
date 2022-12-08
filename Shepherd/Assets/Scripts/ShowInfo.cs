using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowInfo : MonoBehaviour
{
    private GameObject infoPanel;

    public void Start()
    {
        infoPanel = GameObject.Find("Info Panel");
        infoPanel.SetActive(false);
    }

    public void OnClick()
    {
        // Check if the Info Panel is active
        if (infoPanel.activeInHierarchy)
        {
            // If so, set it to inactive
            infoPanel.SetActive(false);
        } else
        {
            // If not, set it to active
            infoPanel.SetActive(true);
        }
    }
}
