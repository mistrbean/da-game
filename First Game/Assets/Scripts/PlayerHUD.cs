using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHUD : MonoBehaviour
{
    [SerializeField] public PlayerState playerState;
    [SerializeField] public Transform cam;

    //pick-up prompt
    public GameObject promptPickup;
    public GameObject lookingAt;

    private Slider energySlider;
    private TextMeshProUGUI energyValue;

    [SerializeField] private Slider[] dashSliders;

    private void Awake()
    {
        energySlider = GameObject.Find("EnergySlider").GetComponent<Slider>();
        energyValue = GameObject.Find("EnergyValue").GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        if (LookingAtEquippable(out GameObject item))
        {
            CheckPrompt(true);
            lookingAt = item;
        }
        else
        {
            CheckPrompt(false);
            lookingAt = null;
        }

        UpdateEnergySlider();
        if (playerState.dashCount < playerState.maxDashCount)
        {
            UpdateDashElements();
        }
    }

    //set pick-up prompt visibility to check
    public void CheckPrompt(bool check)
    {
        if (promptPickup.activeSelf != check) promptPickup.SetActive(check);
    }

    public bool PromptVisible()
    {
        return promptPickup.activeSelf;
    }

    private bool LookingAtEquippable(out GameObject item)
    {
        //only collide with objects in "Item" layer 10
        int layerMask = 1 << 10;

        Ray ray = new Ray(cam.position, cam.TransformDirection(Vector3.forward));

        if (Physics.Raycast(ray, out RaycastHit hit, 5.0f, layerMask))
        {
            //Debug.DrawRay(cam.position, cam.TransformDirection(Vector3.forward) * hit.distance, Color.yellow, 1, true);
            item = hit.collider.gameObject;
            if (item == playerState.equippedWeapon) return false;
            return true;
        }
        else
        {
            item = null;
            return false;
        }
    }

    public void UpdateEnergySlider()
    {
        energySlider.value = playerState.energy;
    }

    public void SetEnergyValueText()
    {
        energyValue.text = energySlider.value.ToString();
    }

    public void UpdateDashElements()
    {
        for (int i = 0; i < playerState.dashChargeCooldowns.Length; i++)
        {
            dashSliders[i].value = playerState.dashChargeCooldowns[i];
        }
        
    }

}
