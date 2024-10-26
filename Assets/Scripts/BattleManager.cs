using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public Transform playPos1, playPos2, playPos3;
    public Transform enemyPos1, enemyPos2, enemyPos3;
    public GameObject GothicHero, DancingGirl, Bird, Ghost, Snake, BipedalUnit;
    public List<GameObject> allies;
    public List<GameObject> enemies;

    void Start()
    {
        state = BattleState.Start;
        SetupBattle();
    }

    void SetupBattle()
    {
        // Initialize battle setup
        GameObject player1 = Instantiate(GothicHero, playPos1);
        allies.Add(player1);

        GameObject player2 = Instantiate(DancingGirl, playPos2);
        allies.Add(player2);

        GameObject player3 = Instantiate(Bird, playPos3);
        allies.Add(player3);

        GameObject enemy1 = Instantiate(Ghost, enemyPos1);
        enemies.Add(enemy1);

        GameObject enemy2 = Instantiate(Snake, enemyPos2);
        enemies.Add(enemy2);

        GameObject enemy3 = Instantiate(BipedalUnit, enemyPos3);
        enemies.Add(enemy3);

        state = BattleState.PlayerTurn;
        StartCoroutine(PlayerTurn());
    }

    IEnumerator PlayerTurn()
    {
        if (state != BattleState.PlayerTurn)
            yield break;

        // Handle player input and actions
        // After the player completes their action, transition to the enemies turn

        // loop through each player character to act
        foreach (var player in allies)
        {
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
        state = BattleState.EnemyTurn;
        StartCoroutine(EnemyTurn());
    }

    IEnumerator EnemyTurn()
    {
        if (state != BattleState.EnemyTurn)
            yield break;

        // Enemy AI logic
        foreach (var enemy in enemies)
        {
            Character enemyCharacter = enemy.GetComponent<Character>();
            
            int randomIndex = UnityEngine.Random.Range(0, enemyCharacter.actions.Count); // Get a random index
            Action chosenAction = enemyCharacter.actions[randomIndex]; // Choose a random action 

            List<Character> targets = new List<Character>();
            // targeting: single or AoE
            if (chosenAction.aoe)
            {
                foreach(var ally in allies)
                {
                    targets.Add(ally.GetComponent<Character>());
                }
            } else
            {
                int target = UnityEngine.Random.Range(0, allies.Count); //choose single target
                targets.Add(allies[target].GetComponent<Character>());
            }

            chosenAction.Execute(enemyCharacter, targets); // and execute action
            
            // dialogue: "Enemy uses {chosenAction.name} on {targets.characterName}!"
            yield return new WaitForSeconds(1f);
        }

        yield return new WaitForSeconds(1f); // Delay to simulate enemy action

        // After enemies action, transition back to the player’s turn or check for win/loss
        state = BattleState.PlayerTurn;
        StartCoroutine(PlayerTurn());
    }
}
