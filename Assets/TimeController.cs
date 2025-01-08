using TMPro;
using UnityEngine;

public class Director : MonoBehaviour
{
    float time = 0.0f;
    GameObject timerText;
    GameObject lap1Text;
    GameObject lap2Text;
    GameObject lap3Text;
    int lapCount = 0;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        timerText = GameObject.Find("Time");
        lap1Text = GameObject.Find("Lap1");
        lap2Text = GameObject.Find("Lap2");
        lap3Text = GameObject.Find("Lap3");
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        timerText.GetComponent<TextMeshProUGUI>().text = "Time: " + time.ToString("F2");
        if (Input.GetKeyDown(KeyCode.E))
        {
            lapCount++;
            if (lapCount == 1)
            {
                lap1Text.GetComponent<TextMeshProUGUI>().text = "lap1: " + time.ToString("000.00");
            }
            else if (lapCount == 2)
            {
                lap2Text.GetComponent<TextMeshProUGUI>().text = "lap2: " + time.ToString("000.00");
            }
            else
            {
                lap3Text.GetComponent<TextMeshProUGUI>().text = "lap3: " + time.ToString("000.00");
            }


        }
    }
}