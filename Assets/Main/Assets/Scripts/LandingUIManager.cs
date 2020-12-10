using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LandingUIManager : MonoBehaviour
{
    public Text coins;
    public Text tourHistoryCoins;
    public Text tourHistoryName;
    public Text tourHistoryTime;
    public GameObject languagePanel;
    public GameObject home;
    public GameObject topBar;
    public GameObject topShadow;
    public GameObject menuButton;
    
    void OnEnable()
    {
        int curr_coins;

        // Check if coins in playerpref
        if (PlayerPrefs.HasKey("PlayerCoins"))
        {
            languagePanel.SetActive(false);
            home.SetActive(true);
            topBar.SetActive(true);
            topShadow.SetActive(true);
            menuButton.SetActive(true);
            
            curr_coins = PlayerPrefs.GetInt("PlayerCoins", -1);
            tourHistoryCoins.text = curr_coins.ToString();
            tourHistoryName.text = "You";
            tourHistoryTime.text = "15:00 min";
        }
        else
        {
            curr_coins = 0;
            PlayerPrefs.SetInt("PlayerCoins", curr_coins);
            PlayerPrefs.Save();
        }

        coins.text = curr_coins.ToString();




    }


}
