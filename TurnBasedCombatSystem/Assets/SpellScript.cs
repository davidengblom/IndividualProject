using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpellScript : MonoBehaviour
{
    //Variables
    public string spellName;       //Name of the spell
    public int spellPower;         //Magnitude of the spell (How much damage or healing it does etc)
    public string spellPowerType;  //If it does damage or heals
    public int spellDuration;      //Duration of the spell
    public int spellCooldown;      //Number of turns you have to wait to use the spell again
    public int spellEffectPenalty; //Number of turns the enemy loses as result of the spell

    public Unit target;

    //Components
    public GameObject spellEffect; //Reference to the spell effect

    private BattleSystem battleSystem; //Reference to the BattleSystem script

    private void Start()
    {
        battleSystem = GameObject.Find("BattleSystem").GetComponent<BattleSystem>();
    }

    public void UseSpellName() //Change name to suit the spell (Call this from a button)
    {
        if(battleSystem.state != BattleState.PlayerTurn)
        {
            return;
        }

        StartCoroutine(SpellName()); //If you change the name of the coroutine, change it here too
    }

    private IEnumerator SpellName() //Change SpellName to suit the spell ("Heal" for example)
    {
        foreach(Button button in battleSystem.spellButtonArray) //Disables all buttons under "spellButtons"
        {
            button.interactable = false;
        }

        battleSystem.uIText.text = battleSystem.playerUnit.unitName + " uses " + spellName;

        yield return new WaitForSeconds(battleSystem.waitTime);

        if(battleSystem.ranNumber < battleSystem.playerUnit.hitChance)
        {
            GameObject effect = Instantiate(spellEffect, battleSystem.playerUnit.transform); //Spawn the effect

            yield return new WaitForSeconds(battleSystem.waitTime);

            battleSystem.playerUnit.currentHP += spellPower; //Heal the player (Example)
            battleSystem.playerHUD.SetHP(battleSystem.playerUnit.currentHP);
            Destroy(effect);
            spellCooldown = 1; //Number of turns you have to wait to use the spell again
            spellEffectPenalty = 0; //Number of turns the enemy lost as result of the spell effect
        }
        else
        {
            battleSystem.uIText.text = "Spell Missed!";
        }

        yield return new WaitForSeconds(battleSystem.waitTime);

        //Re-enable buttons
        battleSystem.spellButtons.SetActive(false);
        battleSystem.buttons.SetActive(true);

        foreach(Button button in battleSystem.defaultButtonArray) //Disables all buttons under "buttons"
        {
            button.interactable = false;
        }
        foreach(Button button in battleSystem.spellButtonArray) //Enables all buttons under "spellButtons"
        {
            button.interactable = true;
        }

        StartCoroutine(battleSystem.EnemyTurn()); //Switch to Enemy Turn
    }
}
