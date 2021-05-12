using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class AugmentMenu : MonoBehaviour
{
    public PlayerState playerState;

    [SerializeField] private Image[] icons;
    [SerializeField] private Button[] slotButtons;
    [SerializeField] private GameObject[] augmentChoices; //per slot
    [SerializeField] private AugmentDataContainer augmentData;
    [SerializeField] private GameObject augmentHoverWindow;
    [SerializeField] private TextMeshProUGUI augmentHoverName;
    [SerializeField] private TextMeshProUGUI augmentHoverDescription;

    private void Awake()
    {
        UpdateAugmentIcons();
    }

    public void UpdateAugmentIcons()
    {
        SpriteState spriteState = new SpriteState();
        if (playerState.headAugment != null)
        {
            spriteState.highlightedSprite = playerState.headAugment.highlightedIcon;
            slotButtons[0].spriteState = spriteState;
            if (slotButtons[0].TryGetComponent<AugmentDataContainer>(out AugmentDataContainer augmentData))
            {
                augmentData.UpdateAugmentData(playerState.headAugment);
            }
            else
            {
                slotButtons[0].gameObject.AddComponent<AugmentDataContainer>().UpdateAugmentData(playerState.headAugment);
            }
            icons[0].sprite = playerState.headAugment.icon;
            icons[0].gameObject.SetActive(true);
        }
        if (playerState.rightArmAugment != null)
        {
            spriteState.highlightedSprite = playerState.rightArmAugment.highlightedIcon;
            slotButtons[1].spriteState = spriteState;
            if (slotButtons[1].TryGetComponent<AugmentDataContainer>(out AugmentDataContainer augmentData))
            {
                augmentData.UpdateAugmentData(playerState.rightArmAugment);
            }
            else
            {
                slotButtons[1].gameObject.AddComponent<AugmentDataContainer>().UpdateAugmentData(playerState.rightArmAugment);
            }
            icons[1].sprite = playerState.rightArmAugment.icon;
            icons[1].gameObject.SetActive(true);
        }
        if (playerState.leftArmAugment != null)
        {
            spriteState.highlightedSprite = playerState.leftArmAugment.highlightedIcon;
            slotButtons[2].spriteState = spriteState;
            if (slotButtons[2].TryGetComponent<AugmentDataContainer>(out AugmentDataContainer augmentData))
            {
                augmentData.UpdateAugmentData(playerState.leftArmAugment);
            }
            else
            {
                slotButtons[2].gameObject.AddComponent<AugmentDataContainer>().UpdateAugmentData(playerState.leftArmAugment);
            }
            icons[2].sprite = playerState.leftArmAugment.icon;
            icons[2].gameObject.SetActive(true);
        }
        if (playerState.chestAugment != null)
        {
            spriteState.highlightedSprite = playerState.chestAugment.highlightedIcon;
            slotButtons[3].spriteState = spriteState;
            if (slotButtons[3].TryGetComponent<AugmentDataContainer>(out AugmentDataContainer augmentData))
            {
                augmentData.UpdateAugmentData(playerState.chestAugment);
            }
            else
            {
                slotButtons[3].gameObject.AddComponent<AugmentDataContainer>().UpdateAugmentData(playerState.chestAugment);
            }
            icons[3].sprite = playerState.chestAugment.icon;
            icons[3].gameObject.SetActive(true);
        }
        if (playerState.waistAugment != null)
        {
            spriteState.highlightedSprite = playerState.waistAugment.highlightedIcon;
            slotButtons[4].spriteState = spriteState;
            if (slotButtons[4].TryGetComponent<AugmentDataContainer>(out AugmentDataContainer augmentData))
            {
                augmentData.UpdateAugmentData(playerState.waistAugment);
            }
            else
            {
                slotButtons[4].gameObject.AddComponent<AugmentDataContainer>().UpdateAugmentData(playerState.waistAugment);
            }
            icons[4].sprite = playerState.waistAugment.icon;
            icons[4].gameObject.SetActive(true);
        }
        if (playerState.rightLegAugment != null)
        {
            spriteState.highlightedSprite = playerState.rightLegAugment.highlightedIcon;
            slotButtons[5].spriteState = spriteState;
            if (slotButtons[5].TryGetComponent<AugmentDataContainer>(out AugmentDataContainer augmentData))
            {
                augmentData.UpdateAugmentData(playerState.rightLegAugment);
            }
            else
            {
                slotButtons[5].gameObject.AddComponent<AugmentDataContainer>().UpdateAugmentData(playerState.rightLegAugment);
            }
            icons[5].sprite = playerState.rightLegAugment.icon;
            icons[5].gameObject.SetActive(true);
        }
        if (playerState.leftLegAugment != null)
        {
            spriteState.highlightedSprite = playerState.leftLegAugment.highlightedIcon;
            slotButtons[6].spriteState = spriteState;
            if (slotButtons[6].TryGetComponent<AugmentDataContainer>(out AugmentDataContainer augmentData))
            {
                augmentData.UpdateAugmentData(playerState.leftLegAugment);
            }
            else
            {
                slotButtons[6].gameObject.AddComponent<AugmentDataContainer>().UpdateAugmentData(playerState.leftLegAugment);
            }
            icons[6].sprite = playerState.leftLegAugment.icon;
            icons[6].gameObject.SetActive(true);
        }
    }

    //slot -> 0 = head, 1 = r_arm, 2 = l_arm, 3 = chest, 4 = waist, 5 = r_leg, 6 = l_leg
    public void DisplayAugments(int slot)
    {
        /*switch (slot)
        {
            case 0:
                Debug.Log("Pressed head slot");
                break;
            case 1:
                Debug.Log("Pressed right arm slot");
                break;
            case 2:
                Debug.Log("Pressed left arm slot");
                break;
            case 3:
                Debug.Log("Pressed chest slot");
                break;
            case 4:
                Debug.Log("Pressed waist slot");
                break;
            case 5:
                Debug.Log("Pressed right leg slot");
                break;
            case 6:
                Debug.Log("Pressed left leg slot");
                break;
        }*/

        CloseAllMenus();

        augmentChoices[slot].SetActive(true);
        int j = 0;
        for (int i = 0; i < playerState.collectedAugments.Length; i++)
        {
            if (playerState.collectedAugments[i].slot == slot && j < transform.childCount)
            {
                GameObject augmentListing = augmentChoices[slot].transform.GetChild(j).gameObject;
                Image image = augmentListing.GetComponent<Image>();
                Button button = augmentListing.GetComponent<Button>();
                if (augmentListing.TryGetComponent<AugmentDataContainer>(out AugmentDataContainer augmentData))
                {
                    augmentData.UpdateAugmentData(playerState.collectedAugments[i]);
                }
                else
                {
                    augmentListing.AddComponent(typeof(AugmentDataContainer));
                    augmentListing.GetComponent<AugmentDataContainer>().UpdateAugmentData(playerState.collectedAugments[i]);
                }
                SpriteState spriteState = new SpriteState();
                image.sprite = playerState.collectedAugments[i].icon;
                image.color = Color.white;
                int index = i;
                button.onClick.AddListener(delegate { EquipAugment(playerState.collectedAugments[index], slot); });
                spriteState.highlightedSprite = playerState.collectedAugments[i].highlightedIcon;
                button.spriteState = spriteState;
                j++;
            }
            else if ((slot == 1 || slot == 2) && playerState.collectedAugments[i].slot == 2)
            {
                GameObject augmentListing = augmentChoices[slot].transform.GetChild(j).gameObject;
                Image image = augmentListing.GetComponent<Image>();
                Button button = augmentListing.GetComponent<Button>();
                if (augmentListing.TryGetComponent<AugmentDataContainer>(out AugmentDataContainer augmentData))
                {
                    augmentData.UpdateAugmentData(playerState.collectedAugments[i]);
                }
                else
                {
                    augmentListing.AddComponent(typeof(AugmentDataContainer));
                    augmentListing.GetComponent<AugmentDataContainer>().UpdateAugmentData(playerState.collectedAugments[i]);
                }
                SpriteState spriteState = new SpriteState();
                image.sprite = playerState.collectedAugments[i].icon;
                image.color = Color.white;
                int index = i;
                button.onClick.AddListener(delegate { EquipAugment(playerState.collectedAugments[index], slot); });
                spriteState.highlightedSprite = playerState.collectedAugments[i].highlightedIcon;
                button.spriteState = spriteState;
                j++;
            }
            else if ((slot == 5 || slot == 6) && playerState.collectedAugments[i].slot == 5)
            {
                GameObject augmentListing = augmentChoices[slot].transform.GetChild(j).gameObject;
                Image image = augmentListing.GetComponent<Image>();
                Button button = augmentListing.GetComponent<Button>();
                if (augmentListing.TryGetComponent<AugmentDataContainer>(out AugmentDataContainer augmentData))
                {
                    augmentData.UpdateAugmentData(playerState.collectedAugments[i]);
                }
                else
                {
                    augmentListing.AddComponent(typeof(AugmentDataContainer));
                    augmentListing.GetComponent<AugmentDataContainer>().UpdateAugmentData(playerState.collectedAugments[i]);
                }
                SpriteState spriteState = new SpriteState();
                image.sprite = playerState.collectedAugments[i].icon;
                image.color = Color.white;
                int index = i;
                button.onClick.AddListener(delegate { EquipAugment(playerState.collectedAugments[index], slot); });
                spriteState.highlightedSprite = playerState.collectedAugments[i].highlightedIcon;
                button.spriteState = spriteState;
                j++;
            }
        }
    }

    public void EquipAugment(Augment augment, int slot)
    {
        Debug.Log($"Equipped augment: {augment.name}, in slot: {slot}");
        CloseAllMenus();
        playerState.EquipAugment(augment, slot);
        UpdateAugmentIcons();
    }

    

    public void CloseAllMenus()
    {
        for (int i = 0; i < 7; i++)
        {
            foreach (Transform child in augmentChoices[i].transform)
            {
                child.gameObject.GetComponent<Button>().onClick.RemoveAllListeners();
            }
            augmentChoices[i].SetActive(false);
        }
        HideAugmentData();
    }

    public void DisplayAugmentData(AugmentDataContainer augmentData)
    {
        augmentHoverWindow.transform.position = augmentData.transform.position;
        augmentHoverWindow.transform.position += Vector3.up * 180;
        augmentHoverWindow.SetActive(true);
        if (augmentData.augmentInfo != null)
        {
            augmentHoverName.text = augmentData.augmentInfo.name;
            augmentHoverDescription.text = augmentData.augmentInfo.description;
        }
    }

    public void HideAugmentData()
    {
        augmentHoverWindow.SetActive(false);
    }
}
