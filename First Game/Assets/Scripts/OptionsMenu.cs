using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OptionsMenu : MonoBehaviour
{
    private Slider enemySlider;
    private Slider sensitivitySlider;
    private TextMeshProUGUI sensitivityText;

    // Start is called before the first frame update
    void Start()
    {
        enemySlider = GameObject.Find("EnemyHealthSlider").GetComponent<Slider>();
        sensitivitySlider = GameObject.Find("SensitivitySlider").GetComponent<Slider>();
        sensitivityText = GameObject.Find("SensitivityValue").GetComponent<TextMeshProUGUI>();
        enemySlider.value = PlayerPrefs.GetInt("enemyHealth");
        sensitivitySlider.value = PlayerPrefs.GetFloat("playerSens");
        sensitivityText.text = sensitivitySlider.value.ToString("#.##");
    }


    public void SetText()
    {
        sensitivityText.text = sensitivitySlider.value.ToString("#.##");
    }

    public void SavePrefs()
    {
        PlayerPrefs.SetInt("enemyHealth", (int)enemySlider.value);
        PlayerPrefs.SetFloat("playerSens", sensitivitySlider.value);
        PlayerPrefs.Save();
    }

}
