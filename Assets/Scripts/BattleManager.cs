using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CBC = CharacterButtonController;
public enum BattleState
{
    Start,
    PlayerTurn,
    EnemyTurn,
    Win,
    Lose
}

public class BattleSystem : MonoBehaviour
{
    public BattleState state;
    public Transform heroPos, dancerPos, birdPos;
    public Transform ghostPos, snakePos, bipedalPos;
    public GameObject GothicHero, DancingGirl, Bird, Ghost, Snake, BipedalUnit;
    public GameObject allySelector, enemySelector;
    public Character activeAlly, activeTarget;
    public CBC heroStatus, dancerStatus, birdStatus; 
    public CBC selectHero, selectDancer, selectBird; 
    public CBC selectGhost, selectSnake, selectBipedal; 
    public Button action1Button, action2Button, action3Button;
    public List<GameObject> allyList;
    public List<GameObject> enemyList;
    public bool hasActed, hasSelected;

    void Start()
    {
        state = BattleState.Start;
        SetupBattle();
    }

    void SetupBattle()
    {
        // Initialize battle setup
        GameObject gothicHero = Instantiate(GothicHero, heroPos);
        heroStatus.character = gothicHero;
        heroStatus.Setup();
        heroStatus.MakeUniteractable();
        selectHero.character = gothicHero;
        selectHero.Setup();
        allyList.Add(gothicHero);

        GameObject dancingGirl = Instantiate(DancingGirl, dancerPos);
        dancerStatus.character = dancingGirl;
        dancerStatus.Setup();
        dancerStatus.MakeUniteractable();
        selectDancer.character = dancingGirl;
        selectDancer.Setup();
        allyList.Add(dancingGirl);

        GameObject bird = Instantiate(Bird, birdPos);
        birdStatus.character = bird;
        birdStatus.Setup();
        birdStatus.MakeUniteractable();
        selectBird.character = bird;
        selectBird.Setup();
        allyList.Add(bird);

        GameObject ghost = Instantiate(Ghost, ghostPos);
        selectGhost.character = ghost;
        selectGhost.Setup();
        enemyList.Add(ghost);

        GameObject snake = Instantiate(Snake, snakePos);
        selectSnake.character = snake;
        selectSnake.Setup();
        enemyList.Add(snake);

        GameObject bipedalUnit = Instantiate(BipedalUnit, bipedalPos);
        selectBipedal.character = bipedalUnit;
        selectBipedal.Setup();
        enemyList.Add(bipedalUnit);

        enemySelector.SetActive(false);
        allySelector.SetActive(false);

        state = BattleState.PlayerTurn;
        StartCoroutine(PlayerTurn());
    }

    IEnumerator PlayerTurn()
    {
        StopCoroutine(EnemyTurn());
        if (state != BattleState.PlayerTurn)
            yield break;

        // Handle player input and actions
        // After the player completes their action, transition to the enemies turn

        // loop through each player character to act
        foreach (GameObject player in allyList)
        {
            activeAlly = player.GetComponent<Character>();
            activeAlly.RestoreEnergy();

            action1Button.GetComponentInChildren<TMP_Text>().text = activeAlly.actions[0].actionName;
            action2Button.GetComponentInChildren<TMP_Text>().text = activeAlly.actions[1].actionName;
            action3Button.GetComponentInChildren<TMP_Text>().text = activeAlly.actions[2].actionName;

            EnableSelection();
            hasActed = false;
            hasSelected = false;

            yield return new WaitUntil(HasActed);
            /*
            put action selection and execution loop here
            player chooses an action and target for the selected ally (ex. index i)
            dialog: "what will {player.characterName} do?"

            after selection:
            dialogue box: "{player.characterName} uses {player.actions[i].actionName}!"
            executing the action:
                if energy cost exceeds remining energy, "{player.characterName} does not have enough energy!"
                don't execute action here

                otherwise,
                if damaging: "{player.characterName} damages {target.characterName} for {player.actions[i].damage}!"
                    - 
                    - remember to loop this if the move is aoe

                if healing: "{player.characterName} heals {target.characterName} for {player.actions[i].damage}!"
                if blocking: "{player.characterName} blocks!"
                player.actions[i].Execute(player, target);

                don't forget to add a yield wait after target

            */
            
        }
        StopCoroutine(PlayerAction());
        state = BattleState.EnemyTurn;
        StartCoroutine(EnemyTurn());
    }

    IEnumerator EnemyTurn()
    {
        StopCoroutine(PlayerTurn());
        if (state != BattleState.EnemyTurn)
            yield break;

        // Enemy AI logic
        foreach (var enemy in enemyList)
        {
            Character enemyCharacter = enemy.GetComponent<Character>();
            enemyCharacter.RestoreEnergy();
            
            int randomIndex = UnityEngine.Random.Range(0, enemyCharacter.actions.Count); // Get a random index
            Action chosenAction = enemyCharacter.actions[randomIndex]; // Choose a random action 

            List<Character> targets = new List<Character>();
            // targeting: single or AoE
            if (chosenAction.aoe)
            {
                foreach(var ally in allyList)
                {
                    targets.Add(ally.GetComponent<Character>());
                }
            } else
            {
                int target = UnityEngine.Random.Range(0, allyList.Count); //choose single target
                targets.Add(allyList[target].GetComponent<Character>());
            }

            chosenAction.Execute(enemyCharacter, targets); // and execute action
            
            // dialogue: "Enemy uses {chosenAction.name} on {targets.characterName}!"
            UpdateStatus();
            yield return new WaitForSeconds(1f);
        }

        yield return new WaitForSeconds(1f); // Delay to simulate enemy action

        // After enemies action, transition back to the player’s turn or check for win/loss
        state = BattleState.PlayerTurn;
        StartCoroutine(PlayerTurn());
    }

    public void SelectAction(int actionIndex) 
    {
        if (state != BattleState.PlayerTurn) {
            return; 
        }

        Action chosenAction = activeAlly.actions[actionIndex];
        if (chosenAction.cost > activeAlly.energy) 
        {
            TriggerDialogue("Not enough energy!");
        }
        else
        {
            StartCoroutine(PlayerAction(actionIndex));
        }
    }

    IEnumerator PlayerAction(int actionIndex = 3) {
        Action chosenAction = activeAlly.actions[actionIndex];
        if (chosenAction == null) {
            yield break;
        }
        List<Character> targets = new List<Character>();

        if (chosenAction.aoe) {
            foreach (GameObject enemy in enemyList) 
            {
                targets.Add(enemy.GetComponent<Character>());
            }
            chosenAction.Execute(activeAlly, targets);
            TriggerDialogue("");
        }
        else if (chosenAction.heal)
        {
            SelectAlly();
            yield return new WaitUntil(HasSelected);
            targets.Add(activeTarget);
            chosenAction.Execute(activeAlly, targets);
            TriggerDialogue("");
        }
        else 
        {
            SelectEnemy();
            yield return new WaitUntil(HasSelected);
            targets.Add(activeTarget);
            chosenAction.Execute(activeAlly, targets);
            TriggerDialogue("");
        }

        UpdateStatus();
        yield return new WaitForSeconds(1.0f);
        hasActed = true;
    }

    public void TriggerDialogue(string message) {
        Debug.Log(message);
    }

    public bool HasActed() {
        return hasActed;
    }

    public bool HasSelected() {
        return hasSelected;
    }

    public void SelectAlly() {
        allySelector.SetActive(true);
        DisableSelection();
    }

    public void SelectEnemy() {
        enemySelector.SetActive(true);
        DisableSelection();
    }

    public void SelectTarget(Character ch) 
    {
        activeTarget = ch;
        hasSelected = true;
        enemySelector.SetActive(false);
        allySelector.SetActive(false);
    }

    public void UpdateStatus() 
    {
        heroStatus.UpdateContent();
        dancerStatus.UpdateContent();
        birdStatus.UpdateContent();

        foreach (var ally in allyList) 
        {
            if (!ally.GetComponent<Character>().isAlive) {
                allyList.Remove(ally);
            }
        }
        if (allyList.Count == 0) {
            state = BattleState.Lose;
            StartCoroutine(PlayerLoss());
            return;
        }

        foreach (var enemy in enemyList) 
        {
            if (!enemy.GetComponent<Character>().isAlive) {
                allyList.Remove(enemy);
            }
        }
        if (enemyList.Count == 0) {
            state = BattleState.Win;
            StartCoroutine(PlayerWin());
        }
    }

    public void DisableSelection() 
    {
        action1Button.interactable = false;
        action2Button.interactable = false;
        action3Button.interactable = false;
    }

    public void EnableSelection() 
    {
        action1Button.interactable = true;
        action2Button.interactable = true;
        action3Button.interactable = true;
    }

    IEnumerator PlayerLoss()
    {
        StopCoroutine(EnemyTurn());
        StopCoroutine(PlayerTurn());
        TriggerDialogue("");
        yield break;
    }
    IEnumerator PlayerWin() 
    {
        StopCoroutine(EnemyTurn());
        StopCoroutine(PlayerTurn());
        TriggerDialogue("");
        yield break;
    }
}
