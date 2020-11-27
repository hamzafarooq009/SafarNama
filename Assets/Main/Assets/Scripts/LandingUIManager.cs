using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LandingUIManager : MonoBehaviour
{
    public Text coins;
    void OnEnable()
    {
        int curr_coins;

        // Check if coins in playerpref
        if (PlayerPrefs.HasKey("PlayerCoins"))
        {
            curr_coins = PlayerPrefs.GetInt("PlayerCoins", -1);
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
