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

    private void OnEnable() {
        UpdateContent();
    }
    
    public void Setup() 
    {
        characterScript = character.GetComponent<Character>();
        nameText.text = characterScript.characterName;

        hpMax = characterScript.maxHealth;
        hpCurrent = hpMax;
        hpImg.fillAmount = hpCurrent/hpMax;
        hpText.text = String.Format("{0}/{1}", hpCurrent, hpMax);

        energyMax = characterScript.maxEnergy;
        energyCurrent = energyMax;
        energyImg.fillAmount = energyCurrent/energyMax;
        energyText.text = String.Format("{0}/{1}", energyCurrent, energyMax);

    }
    public void UpdateContent() 
    {
        hpCurrent = characterScript.health;
        hpImg.fillAmount = hpCurrent/hpMax;
        hpText.text = String.Format("{0}/{1}", hpCurrent, hpMax);
        if (!characterScript.isAlive) {
            thisButton.interactable = false;
            return;
        }

        energyCurrent = characterScript.energy;
        energyImg.fillAmount = energyCurrent/energyMax;
        energyText.text = String.Format("{0}/{1}", energyCurrent, energyMax);
    }

    public void MakeUniteractable() {
        thisButton.interactable = false;
    }

    public void TransmitCharacter()
    {
        battleManager.SelectTarget(characterScript);
    }
}
