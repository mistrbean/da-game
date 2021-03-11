using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    private Slider enemySlider;
    private Slider sensitivitySlider;

    // Start is called before the first frame update
    void Start()
    {
        enemySlider = GameObject.Find("EnemyHealthSlider").GetComponent<Slider>();
        sensitivitySlider = GameObject.Find("SensitivitySlider").GetComponent<Slider>();
        enemySlider.value = PlayerPrefs.GetInt("enemyHealth");
        sensitivitySlider.value = PlayerPrefs.GetFloat("playerSens");
    }

    public void SavePrefs()
    {
        PlayerPrefs.SetInt("enemyHealth", (int)enemySlider.value);
        PlayerPrefs.SetFloat("playerSens", sensitivitySlider.value);
        PlayerPrefs.Save();
    }

}
