using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {


   
    bool paused;
    public GameObject player;

    public Canvas menu;
    public GameObject MenuPanel;
    public GameObject AreYouSurePanel;
    public GameObject OptionsPanel;
    CharacterControl charController = new CharacterControl();


    public void NewGame()
    {
        paused = false;
        menu.GetComponent<Canvas>().enabled = false;
        Time.timeScale = 1f;
        paused = false;
        Cursor.visible = false;
    }

    public void Options()
    {

    }

    public void QuitGame()
    {
    
         
    }

    public void PauseAndResume()
    {
        if (paused == true)
        {
            menu.GetComponent<Canvas>().enabled = true;
            Time.timeScale = 0f;
            Cursor.visible = true;
        }
        else
        {
            menu.GetComponent<Canvas>().enabled = false;
            Time.timeScale = 1f;
            Cursor.visible = false;

        }
    }


     void Start()
    {
        menu.GetComponent<Canvas>().enabled = true;
        Time.timeScale = 0f;
    }

    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
            paused = !paused;
        PauseAndResume();

    }

 

    public void SaveGame()
    {

        if (player != null)
        {

        int broken;
        if (charController.moveSettings.broken == true) broken = 1;
        else broken = 0;

        PlayerPrefs.SetFloat("PlayerX", player.transform.position.x);
        PlayerPrefs.SetFloat("PlayerY", player.transform.position.y);
        PlayerPrefs.SetFloat("PlayerZ", player.transform.position.z);
        PlayerPrefs.SetInt("broken", broken);

        Debug.Log("GAME_SAVED");
        }
        else Debug.LogError("You have no player asigned");
    }



    public void LoadGame()
    {

        if (player != null)
        {
            float x = PlayerPrefs.GetFloat("PlayerX");
            float y = PlayerPrefs.GetFloat("PlayerY");
            float z = PlayerPrefs.GetFloat("PlayerZ");
            int broken = PlayerPrefs.GetInt("broken");

            player.transform.position = new Vector3(x, y, z);

          
            if (broken == 1) charController.moveSettings.broken = true;
            else charController.moveSettings.broken = false;
         
            paused = false;

        }
        else Debug.LogError("You have no player asigned");

    }



}
