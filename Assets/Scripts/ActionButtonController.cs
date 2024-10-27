using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ActionButtonController : MonoBehaviour
{
    public int actionIndex;
    public BattleSystem battleManager;

    public void TrasmitAction() {
        battleManager.SelectAction(actionIndex);
    }
}
