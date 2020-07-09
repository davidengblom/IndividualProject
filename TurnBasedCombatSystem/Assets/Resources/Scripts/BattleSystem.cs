using System.Collections;
using System.Collections.Generic;
using System.Resources;
using System.Runtime.ConstrainedExecution;
using UnityEngine;
using UnityEngine.UI;

public enum BattleState { Start, PlayerTurn, EnemyTurn, Won, Lost }

public class BattleSystem : MonoBehaviour
{
    public BattleState state;

    public GameObject playerPrefab;
    public GameObject enemyPrefab;

    public Transform playerStation;
    public Transform enemyStation;

    private Unit playerUnit;
    private Unit enemyUnit;

    public Text uIText;

    public HUDScript playerHUD;
    public HUDScript enemyHUD;

    public Button attackButton;
    public Button spellButton;
    public GameObject spellButtons;
    public GameObject buttons;

    public int waitTime = 2;

    private bool isDead;

    void Start()
    {
        state = BattleState.Start;
        StartCoroutine(BattleSetup());
    }

    private IEnumerator BattleSetup()
    {
        GameObject player = Instantiate(playerPrefab, playerStation); //Spawn in the player at the playerStation
        playerUnit = player.GetComponent<Unit>();

        GameObject enemy = Instantiate(enemyPrefab, enemyStation); //Spawn in the enemy at the enemyStation
        enemyUnit = enemy.GetComponent<Unit>();

        uIText.text = "A wild " + enemyUnit.unitName + " approaches..";

        playerHUD.SetHUD(playerUnit);
        enemyHUD.SetHUD(enemyUnit);

        yield return new WaitForSeconds(waitTime);

        state = BattleState.PlayerTurn;
        PlayerTurn();
    }

    void PlayerTurn()
    {
        uIText.text = "Choose an action..";
        buttons.SetActive(true);
        attackButton.interactable = true; //Bundle these together plz
        spellButton.interactable = true;  //^
    }

    private IEnumerator PlayerAttack()
    {
        attackButton.interactable = false; //Bundle these together plz
        spellButton.interactable = false;  //^

        uIText.text = playerUnit.unitName + " attacks!";

        yield return new WaitForSeconds(waitTime);

        float ranNumber = Random.Range(1, 100);

        if(ranNumber < playerUnit.hitChance) //Check if the attack hit
        {
            if (ranNumber < playerUnit.critChance) //If attack hit, check if it was critical
            {
                enemyUnit.shakeIntensity = 0.7f;
                isDead = enemyUnit.TakeDamage(playerUnit.damage * 2); //Critical = Do double damage
                uIText.text = "Critical Hit!";
                enemyUnit.Shake();
            }
            else
            {
                enemyUnit.shakeIntensity = 0.3f;
                isDead = enemyUnit.TakeDamage(playerUnit.damage);
                uIText.text = "Attack Successful!";
                enemyUnit.Shake();
                
            }
        }
        else //If the attack missed
        {
            uIText.text = "Attack Missed!";
        }

        enemyHUD.SetHP(enemyUnit.currentHP); //Update the enemy's HP

        yield return new WaitForSeconds(waitTime);

        if (isDead) //if the enemy died, end the battle
        {
            state = BattleState.Won;
            EndBattle();
        }
        else //Otherwise continue to the enemy's turn
        {
            state = BattleState.EnemyTurn;
            StartCoroutine(EnemyTurn());
        }
    }

    IEnumerator EnemyTurn()
    {
        uIText.text = enemyUnit.unitName + " attacks!";

        yield return new WaitForSeconds(waitTime);

        float ranNumber = Random.Range(1, 100);
        if (ranNumber < enemyUnit.hitChance) //Check if the attack hit
        {
            if (ranNumber < enemyUnit.critChance) //If attack hit, check if it was critical
            {
                playerUnit.shakeIntensity = 0.7f;
                isDead = playerUnit.TakeDamage(enemyUnit.damage * 2); //Crit = Do double damage
                uIText.text = "Critical Hit!";
                playerUnit.Shake();
            }
            else
            {
                playerUnit.shakeIntensity = 0.3f;
                isDead = playerUnit.TakeDamage(enemyUnit.damage);
                uIText.text = "Attack Successful!";
                playerUnit.Shake();
            }
        }
        else //If the attack missed
        {
            uIText.text = "Attack Missed!";
        }

        playerHUD.SetHP(playerUnit.currentHP); //Update the player's HP

        yield return new WaitForSeconds(waitTime);

        if(isDead) //If the player died, lose the game
        {
            state = BattleState.Lost;
            EndBattle();
        }
        else //Otherwise, continue to the player's turn
        {
            state = BattleState.PlayerTurn;
            PlayerTurn();
        }
    }

    void EndBattle()
    {
        if(state == BattleState.Won)
        {
            uIText.text = "You won!";
        }
        else if(state == BattleState.Lost)
        {
            uIText.text = "You lost!";
        }
    }

    public void Attack()
    {
        if(state != BattleState.PlayerTurn)
        {
            return;
        }

        StartCoroutine(PlayerAttack());
    }

    void EnemyAI() //Ability to make small decisions based on values on the player (HP etc)
    {
        //Insert good intelligence here plz
    }

    public void SpellMenu()
    {
        buttons.SetActive(false);
        spellButtons.SetActive(true);

        
    }

    public void BackOutOfSpellMenu()
    {
        spellButtons.SetActive(false);
        buttons.SetActive(true);
    }
}
