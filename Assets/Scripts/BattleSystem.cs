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
    public GameObject allySelector, enemySelector, enemyStatus, allyStatus;
    public Character activeAlly, activeTarget;
    public CBC heroStatus, dancerStatus, birdStatus; 
    public CBC selectHero, selectDancer, selectBird; 
    public CBC ghostStatus, snakeStatus, bipedalStatus;
    public CBC selectGhost, selectSnake, selectBipedal; 
    public Button action1Button, action2Button, action3Button;
    public TMP_Text dialogueText, a1Text, a2Text, a3Text;
    public List<GameObject> allyList;
    public List<GameObject> enemyList;
    public bool hasActed, hasSelected, playerActionRunning, awaitingConfirmation, actionConfirmed, actionCancelled;

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
        heroStatus.Setup(false);
        selectHero.character = gothicHero;
        selectHero.Setup(true);
        allyList.Add(gothicHero);

        GameObject dancingGirl = Instantiate(DancingGirl, dancerPos);
        dancerStatus.character = dancingGirl;
        dancerStatus.Setup(false);
        selectDancer.character = dancingGirl;
        selectDancer.Setup(true);
        allyList.Add(dancingGirl);

        GameObject bird = Instantiate(Bird, birdPos);
        birdStatus.character = bird;
        birdStatus.Setup(false);
        selectBird.character = bird;
        selectBird.Setup(true);
        allyList.Add(bird);

        GameObject ghost = Instantiate(Ghost, ghostPos);
        ghostStatus.character = ghost;
        ghostStatus.Setup(false);
        selectGhost.character = ghost;
        selectGhost.Setup(true);
        enemyList.Add(ghost);

        GameObject snake = Instantiate(Snake, snakePos);
        snakeStatus.character = snake;
        snakeStatus.Setup(false);
        selectSnake.character = snake;
        selectSnake.Setup(true);
        enemyList.Add(snake);

        GameObject bipedalUnit = Instantiate(BipedalUnit, bipedalPos);
        bipedalStatus.character = bipedalUnit;
        bipedalStatus.Setup(false);
        selectBipedal.character = bipedalUnit;
        selectBipedal.Setup(true);
        enemyList.Add(bipedalUnit);

        ToggleDisplayedStatus(false);
        DisableTargetSelect();

        dialogueText.text = string.Format("Let's start the battle!");

        state = BattleState.PlayerTurn;
        StartCoroutine(PlayerTurn());
    }

    IEnumerator PlayerTurn()
    {
        if (state != BattleState.PlayerTurn)
            yield break;
        
        StopCoroutine(EnemyTurn());

        foreach (GameObject ally in allyList)
        {
            activeAlly = ally.GetComponent<Character>();
            ToggleDisplayedStatus(false);

            if (activeAlly.health == 0) {
                dialogueText.text = string.Format("{0} is dead!", activeAlly.characterName);
                yield return new WaitForSeconds(1.5f);
                continue;
            }
            if (activeAlly.isPoisoned) {
                activeAlly.PoisonEffect();
                dialogueText.text = string.Format("{0} took poison damage!", activeAlly.characterName);
                yield return new WaitForSeconds(1.5f);
            }
            if (activeAlly.isRecovering) {
                activeAlly.Recover();
                dialogueText.text = string.Format("{0} is recovering!", activeAlly.characterName);
                yield return new WaitForSeconds(1.5f);
            }
            if (activeAlly.health == 0) {
                dialogueText.text = string.Format("{0} is dead!", activeAlly.characterName);
                yield return new WaitForSeconds(1.5f);
                continue;
            }

            activeAlly.RestoreEnergy();
            UpdateStatus();

            a1Text.text = activeAlly.actions[0].actionName;
            a2Text.text = activeAlly.actions[1].actionName;
            a3Text.text = activeAlly.actions[2].actionName;

            yield return new WaitForSeconds(1.5f);
            dialogueText.text = string.Format("It's {0}'s turn! Choose an action!", activeAlly.characterName);

            EnableActionSelection();
            hasActed = false;
            hasSelected = false;
            playerActionRunning = false;
            actionConfirmed = false; 
            awaitingConfirmation = false;

            yield return new WaitUntil(HasActed);
            StopCoroutine(PlayerAction());
            playerActionRunning = false;
        }
        if (state == BattleState.PlayerTurn) {
            state = BattleState.EnemyTurn; 
            StartCoroutine(EnemyTurn());
        }
    }

    IEnumerator EnemyTurn()
    {
        StopCoroutine(PlayerTurn());
        if (state != BattleState.EnemyTurn) {
            yield break;
        }

        // Enemy AI logic
        foreach (GameObject enemy in enemyList)
        {
            Character enemyCharacter = enemy.GetComponent<Character>();
            if (enemyCharacter.health == 0) {
                dialogueText.text = string.Format("{0} is dead!", enemyCharacter.characterName);
                yield return new WaitForSeconds(1.5f);
                continue;
            }
            if (enemyCharacter.isPoisoned) {
                enemyCharacter.PoisonEffect();
                dialogueText.text = string.Format("{0} took poison damage!", enemyCharacter.characterName);
                yield return new WaitForSeconds(1.5f);
            }
            if (enemyCharacter.isRecovering) {
                enemyCharacter.Recover();
                dialogueText.text = string.Format("{0} is recovering!", enemyCharacter.characterName);
                yield return new WaitForSeconds(1.5f);
            }
            if (enemyCharacter.health == 0) {
                dialogueText.text = string.Format("{0} is dead!", enemyCharacter.characterName);
                yield return new WaitForSeconds(1.5f);
                continue;
            }

            enemyCharacter.RestoreEnergy();
            UpdateStatus();
            dialogueText.text = string.Format("It's {0}'s turn! It's choosing an action!", enemyCharacter.characterName);
            yield return new WaitForSeconds(2.5f);
            
            bool pickedAction = false;
            Action chosenAction = null;
            while (!pickedAction){
                int randomIndex = Random.Range(0, enemyCharacter.actions.Count);
                chosenAction = enemyCharacter.actions[randomIndex];
                if (chosenAction.cost <= enemyCharacter.energy) {
                    pickedAction = true;
                }
            }
            List<Character> targets = new List<Character>();
            string targetString = string.Format("{0} used {1}", enemyCharacter.characterName, chosenAction.actionName);

            if (chosenAction.aoe && !chosenAction.block)
            {
                targetString += string.Format(" on all enemies to deal {0} damage!", chosenAction.damage);
                foreach(var ally in allyList) {
                    targets.Add(ally.GetComponent<Character>());
                }
            }
            else if (chosenAction.heal)
            {
                int target = Random.Range(0, enemyList.Count); //choose single target
                Character targetEnemy = enemyList[target].GetComponent<Character>();
                targets.Add(targetEnemy);
                targetString += string.Format(" on {0} to heal {1} health!", targetEnemy.characterName, chosenAction.damage);
            }
            else if (chosenAction.block) 
            {
                if (chosenAction.aoe) 
                {
                    targetString += string.Format(" to block all enemies for {0}!", enemyCharacter.defense);
                    foreach(var targetEnemy in enemyList) {
                        targets.Add(targetEnemy.GetComponent<Character>());
                    }
                }
                else 
                {
                    targetString += string.Format(" to block for {0}!", enemyCharacter.defense);
                    targets.Add(enemyCharacter);
                }
            }
            else
            {
                int target = Random.Range(0, allyList.Count); //choose single target
                Character targetedAlly = allyList[target].GetComponent<Character>();
                targets.Add(targetedAlly);
                targetString += string.Format(" on {0} to deal {1} damage!", targetedAlly.characterName, chosenAction.damage);
            }

            chosenAction.Execute(enemyCharacter, targets);
            dialogueText.text = targetString;
            UpdateStatus();
            yield return new WaitForSeconds(2.5f);
        }

        yield return new WaitForSeconds(1f); // Delay to simulate enemy action

        // After enemies action, transition back to the player’s turn or check for win/loss
        if (state == BattleState.EnemyTurn) {
            state = BattleState.PlayerTurn;
            StartCoroutine(PlayerTurn());
        }
    }

    public void SelectAction(int actionIndex) 
    {
        if (state != BattleState.PlayerTurn) {
            return; 
        }

        Action chosenAction = activeAlly.actions[actionIndex];
        if (chosenAction.cost > activeAlly.energy) 
        {
            dialogueText.text = string.Format("Not enough energy!");
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

        playerActionRunning = true;
        actionCancelled = false;
        DisableActionSelection();
        List<Character> targets = new List<Character>();

        if (chosenAction.aoe && !chosenAction.heal && !chosenAction.block) {
            awaitingConfirmation = true;
            dialogueText.text = string.Format("You have chosen to use {0}. Confirm?", chosenAction.actionName);
            yield return new WaitUntil(HasActionConfirmedOrCancelled);
            if (actionCancelled) {
                yield break;
            }

            foreach (GameObject enemy in enemyList) 
            {
                targets.Add(enemy.GetComponent<Character>());
            }
            chosenAction.Execute(activeAlly, targets);

            dialogueText.text = string.Format("{0} used {1} against all enemies for {2} damage!", 
                activeAlly.characterName, chosenAction.actionName, chosenAction.damage);
        }
        else if (chosenAction.heal)
        {
            if (chosenAction.aoe)
            {
                awaitingConfirmation = true;
                dialogueText.text = string.Format("You have chosen to use {0}. Confirm?", chosenAction.actionName);
                yield return new WaitUntil(HasActionConfirmedOrCancelled);
                if (actionCancelled) {
                    yield break;
                }

                foreach (GameObject ally in allyList) 
                {
                    targets.Add(ally.GetComponent<Character>());
                }
                chosenAction.Execute(activeAlly, targets);

                dialogueText.text = string.Format("{0} used {1} to heal all allies for {2} health!", activeAlly.characterName, chosenAction.actionName, chosenAction.damage);
            }
            else 
            {
                EnableTargetSelect(isAlly: true);
                yield return new WaitUntil(HasSelected);
                if (actionCancelled) {
                    yield break;
                }

                awaitingConfirmation = true;
                dialogueText.text = string.Format("You have chosen to use {0} on {1}. Confirm?", 
                    chosenAction.actionName, activeTarget.characterName);
                yield return new WaitUntil(HasActionConfirmedOrCancelled);
                if (actionCancelled) {
                    yield break;
                }

                targets.Add(activeTarget);
                chosenAction.Execute(activeAlly, targets);

                dialogueText.text = string.Format("{0} used {1} to heal {2} for {3} health!", activeAlly.characterName, chosenAction.actionName, activeTarget.characterName, chosenAction.damage);
            }
        }
        else if (chosenAction.block && chosenAction.damage == 0)
        {
            awaitingConfirmation = true;
            dialogueText.text = string.Format("You have chosen to use {0}. Confirm?", chosenAction.actionName);
            yield return new WaitUntil(HasActionConfirmedOrCancelled);
            if (actionCancelled) {
                yield break;
            }

            if (chosenAction.aoe) 
            {
                foreach (GameObject ally in allyList) 
                {
                    targets.Add(ally.GetComponent<Character>());
                }
                chosenAction.Execute(activeAlly, targets);

                dialogueText.text = string.Format("{0} used {1} to shield all allies for {2}!",  activeAlly.characterName, chosenAction.actionName, activeAlly.defense);
            }
            else 
            {
                targets.Add(activeAlly);
                chosenAction.Execute(activeAlly, targets);

                dialogueText.text = string.Format("{0} used {1} to block!",
                    activeAlly.characterName, chosenAction.actionName);
            }
        }
        else
        {
            EnableTargetSelect(isAlly: false);
            yield return new WaitUntil(HasSelected);
            if (actionCancelled) {
                yield break;
            }

            awaitingConfirmation = true;
            dialogueText.text = string.Format("You have chosen to use {0} on {1}. Confirm?", 
                chosenAction.actionName, activeTarget.characterName);
            yield return new WaitUntil(HasActionConfirmedOrCancelled);
            if (actionCancelled) {
                yield break;
            }

            targets.Add(activeTarget);
            chosenAction.Execute(activeAlly, targets);

            dialogueText.text = string.Format("{0} used {1} to attack {2} for {3} damage!",
                activeAlly.characterName, chosenAction.actionName, activeTarget.characterName, chosenAction.damage);
        }

        if (!(chosenAction.damage == 0 || chosenAction.heal)) {
            ToggleDisplayedStatus(true);
        }

        UpdateStatus();
        yield return new WaitForSeconds(2.5f);
        hasActed = true;
    }

    public bool HasActed() {
        return hasActed;
    }

    public bool HasSelected() {
        return hasSelected;
    }

    public bool HasActionConfirmedOrCancelled() {
        if (actionConfirmed || actionCancelled) {
            return true;
        }
        else {
            return false;
        }
    }

    public void EnableTargetSelect(bool isAlly) {
        dialogueText.text = string.Format("Select a target!");
        if (isAlly) {
            allySelector.SetActive(true);
        }
        else {
            enemySelector.SetActive(true);
        }
        DisableActionSelection();
    }

    public void DisableTargetSelect() {
        enemySelector.SetActive(false);
        allySelector.SetActive(false);
    }

    public void AssignTarget(Character ch) 
    {
        activeTarget = ch;
        hasSelected = true;
        DisableTargetSelect();
    }

    public void UpdateStatus() 
    {
        heroStatus.UpdateContent();
        dancerStatus.UpdateContent();
        birdStatus.UpdateContent();
        ghostStatus.UpdateContent();
        snakeStatus.UpdateContent();
        bipedalStatus.UpdateContent();

        int deadAllies = 0;
        foreach (GameObject ally in allyList) 
        {
            if (ally.gameObject.GetComponentInChildren<Character>().health == 0) 
            {
                deadAllies += 1;
                ally.GetComponent<Renderer>().enabled = false;
            }
            else {
                ally.GetComponent<Renderer>().enabled = true;
            }
        }
        if (deadAllies == allyList.Count) 
        {
            state = BattleState.Lose;
            StartCoroutine(PlayerLoss());
            return;
        }

        int deadEnemies = 0;
        foreach (GameObject enemy in enemyList) 
        {
            if (enemy.GetComponent<Character>().health == 0) 
            {
                deadEnemies += 1;
                enemy.GetComponent<Renderer>().enabled = false;
            }
            else{
                enemy.GetComponent<Renderer>().enabled = true;
            }
        }
        if (deadEnemies == enemyList.Count) 
        {
            state = BattleState.Win;
            StartCoroutine(PlayerWin());
        }
    }

    public void DisableActionSelection() 
    {
        action1Button.interactable = false;
        action2Button.interactable = false;
        action3Button.interactable = false;
    }

    public void EnableActionSelection() 
    {
        action1Button.interactable = true;
        action2Button.interactable = true;
        action3Button.interactable = true;
    }

    public void ConfirmSelection() 
    {
        if (state == BattleState.PlayerTurn && playerActionRunning && awaitingConfirmation && !actionConfirmed) {
        actionConfirmed = true;
        }
    }

    public void CancelSelection()
    {
        if (state == BattleState.PlayerTurn && playerActionRunning && !actionConfirmed) 
        {
            StopCoroutine(PlayerAction());
            playerActionRunning = false;
            awaitingConfirmation = false;
            hasSelected = false;
            DisableTargetSelect();
            actionCancelled = true;
            EnableActionSelection();
            dialogueText.text = string.Format("You cancelled your action. Choose again!");
        }
    }

    public void ToggleDisplayedStatus(bool displayEnemy) 
    {
        if (displayEnemy) 
        {
            allyStatus.SetActive(false);
            enemyStatus.SetActive(true);
        }
        else 
        {
            allyStatus.SetActive(true);
            enemyStatus.SetActive(false);
        }
    }

    IEnumerator PlayerLoss()
    {
        DisableActionSelection();
        StopCoroutine(EnemyTurn());
        StopCoroutine(PlayerTurn());
        dialogueText.text = string.Format("You lost! Better luck next time!");
        yield break;
    }
    IEnumerator PlayerWin() 
    {
        DisableActionSelection();
        StopCoroutine(EnemyTurn());
        StopCoroutine(PlayerTurn());
        dialogueText.text = string.Format("You won! Good job!");
        yield break;
    }
}
