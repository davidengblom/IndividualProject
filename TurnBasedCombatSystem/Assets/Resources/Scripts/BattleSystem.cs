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
    public Button spell1;
    public Button spell2;
    public Button backButton;

    public GameObject spellButtons;
    public GameObject buttons;

    public GameObject freezeSpell;

    public int waitTime = 2;

    public int cooldownTurns = 0;
    public int spellCooldown = 0; //Make this value modular for all spells instead of just one

    private bool isDead;

    void Start()
    {
        state = BattleState.Start;
        StartCoroutine(BattleSetup());
    }

    private IEnumerator BattleSetup() //Setup the battle. All information about player and opponent. Start player Turn.
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
        if(spellCooldown > 0) //If there is still a cooldown
        {
            spellCooldown--;
            spell2.interactable = false;
        }
        else //If it comes of cooldown
        {
            spell2.interactable = true;
        }
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

        if (cooldownTurns <= 0) //If the enemy has been waiting for the right amount of turns
        {
            enemyUnit.frozen = false;
            enemyUnit.GetComponentInChildren<SpriteRenderer>().color = Color.white;
        }

        yield return new WaitForSeconds(waitTime);
        float ranNumber = Random.Range(1, 100);

        if(!enemyUnit.frozen && cooldownTurns <= 0)
        {
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
        }
        else
        {
            uIText.text = enemyUnit.unitName + " is frozen!";
            if(cooldownTurns != 0)
            {
                cooldownTurns--;
            }
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

    public void Attack() //Called when you click the Attack button
    {
        if(state != BattleState.PlayerTurn)
        {
            return;
        }

        StartCoroutine(PlayerAttack());
    }

    public void Spell2() //Maybe make a seperate script that handles spells
    {
        if(state != BattleState.PlayerTurn)
        {
            return;
        }

        StartCoroutine(FreezeSpell());
    }

    IEnumerator FreezeSpell() //Maybe make a seperate script that handles spells
    {
        spell1.interactable = false;     //
        spell2.interactable = false;     //Bundles these together somehow
        backButton.interactable = false; //

        uIText.text = playerUnit.unitName + " uses Freeze!";

        yield return new WaitForSeconds(waitTime);

        float ranNumber = Random.Range(1, 100);

        if (ranNumber < playerUnit.hitChance) //Check if the attack hit
        {
            GameObject freeze = Instantiate(freezeSpell, enemyUnit.transform);

            yield return new WaitForSeconds(waitTime);

            enemyUnit.Frozen();
            Destroy(freeze);
            cooldownTurns = 1;                //Make this number editable through inspector
            spellCooldown = 2;                //Make this number editable through inspector and make it match the number of rounds
        }

        spellButtons.SetActive(false);        //
        buttons.SetActive(true);              //
        attackButton.interactable = false;    //
        spellButton.interactable = false;     //Bundle these together somehow
        spell1.interactable = true;           //
        spell2.interactable = true;           //
        backButton.interactable = true;       //
        StartCoroutine(EnemyTurn());
    }

    void EnemyAI() //Ability to make small decisions based on values on the player (HP etc)
    {
        //Be able to choose which ability to use depending on values
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
