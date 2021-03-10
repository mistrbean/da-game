using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionStorage : MonoBehaviour
{
    public int enemyHealth;

    public void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void SetEnemyHealth()
    {
        enemyHealth = (int)GameObject.Find("EnemyHealthSlider").GetComponent<Slider>().value;
    }
}
