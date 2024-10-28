using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CharacterButtonController : MonoBehaviour
{
    public GameObject character;
    public BattleSystem battleManager;
    public Character characterScript;
    public Button thisButton;
    public TMP_Text nameText, hpText, energyText;
    public Image hpImg, energyImg;
    public float hpMax, hpCurrent, energyMax, energyCurrent;
    public bool isInteractable;

    private void OnEnable() {
        if (character != null) {
        UpdateContent();
        }
    }
    
    public void Setup(bool interactable) 
    {
        characterScript = character.GetComponent<Character>();
        nameText.text = characterScript.characterName;

        hpMax = characterScript.maxHealth;
        hpCurrent = hpMax;
        hpImg.fillAmount = hpCurrent/hpMax;
        hpText.text = string.Format("{0}/{1}", hpCurrent, hpMax);

        energyMax = characterScript.maxEnergy;
        energyCurrent = energyMax;
        energyImg.fillAmount = energyCurrent/energyMax;
        energyText.text = string.Format("{0}/{1}", energyCurrent, energyMax);

        if (!interactable) {
            thisButton.interactable = false;
        }
        isInteractable = interactable;
    }
    public void UpdateContent() 
    {
        hpCurrent = characterScript.health;
        hpImg.fillAmount = hpCurrent/hpMax;
        hpText.text = string.Format("{0}/{1}", hpCurrent, hpMax);
        if (characterScript.health == 0) {
            thisButton.interactable = false;
        }
        else if (characterScript.health > 0 && isInteractable) {
            thisButton.interactable = true;
        }

        energyCurrent = characterScript.energy;
        energyImg.fillAmount = energyCurrent/energyMax;
        energyText.text = string.Format("{0}/{1}", energyCurrent, energyMax);
    }

    public void TransmitCharacter()
    {
        battleManager.AssignTarget(characterScript);
    }
}
