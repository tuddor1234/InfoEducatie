using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Objectives : MonoBehaviour {

    public GameObject flyingStuff;
    public string[] ObjectiveHelp = new string[8];
    public Text ObjectiveTXT;

    int index = 1;
    bool taskCompleted;

    void Start()
    {
        taskCompleted = false;

    }


    void Update()
    {
        if (taskCompleted)
        {
            index++;
            taskCompleted = false;
        }

        switch (index)
        {
            case 1:
                Objective1Complete();
                break;

            case 2:
                Objecitve2Complete();
                break;

            case 3:
                break;

            case 4:
                break;


        }



    }

    public void Objective1Complete()
    {
        ObjectiveTXT.text = ObjectiveHelp[index - 1];
        float x = Mathf.Abs(transform.position.x - flyingStuff.transform.position.x);
        float y = Mathf.Abs(transform.position.y - flyingStuff.transform.position.y);
        if (x < 5 && y < 5)
        {
            // ANIMATIE!
            this.GetComponent<CharacterControl>().moveSettings.broken = false;
            taskCompleted = true;

        }
    }

    public void Objecitve2Complete()
    {
        if (Input.GetButtonDown("ChangeView"))
        {
            taskCompleted = true;
            Debug.Log("TASK DONE!");
        }

    }

    public void Objecitve3Complete()
    {

    }

    public void Objecitve4Complete()
    {

    }

}
