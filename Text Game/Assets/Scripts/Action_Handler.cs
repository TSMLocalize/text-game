using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.EventSystems;
using TMPro;

/** WELCOME TO THE ACTION HANDLER SCRIPT
 * This script calculates the maths and outcomes of selected actions, and creates combat log messages and floating text for them.
 */

[System.Serializable]
public class Action_Handler : MonoBehaviour
{
    public GameObject floatingDamage;
    public Battle_Manager BM;
    public Battle_Manager_Functions BM_Funcs;
    public Battle_Manager_IEnumerators BM_Enums;
    public bool spellReportFinished;
    public bool enemySpellReportFinished;
    public int maxMessages;
    public List<Message> messageList;
    public GameObject chatPanel;
    public GameObject textObject;

    // Start is called before the first frame update
    void Start()
    {
        maxMessages = 25;
        messageList = new List<Message>();
        BM = GetComponent<Battle_Manager>();
        BM_Funcs = GetComponent<Battle_Manager_Functions>();
        BM_Enums = GetComponent<Battle_Manager_IEnumerators>();
    }

    [System.Serializable]
    public class Message
    {
        public string text;
        public TextMeshProUGUI textObject;
    }

    //This deals with the combat text, maths and floating text
    public void reportOutcome(string report)
    {
        switch (report)
        {
            case "PlayerAttack":

                BM_Funcs.setPlayerOrEnemyTargetFromID(BM.activePlayer, null);

                float random = Random.Range(1, 101);
                float outcome = BM.activePlayer.Accuracy + random;

                SendMessagesToCombatLog(
                        BM.activePlayer.name + "'s hit score is " + outcome + " (" + random + " + " + BM.activePlayer.Accuracy + " acc)" +
                        " vs " + BM.playerTarget.EnemyName + "'s evasion of " + BM.playerTarget.Evasion + ".");

                if (outcome > BM.playerTarget.Evasion)
                {
                    BM.activePlayer.tpTotal += 10 + BM.activePlayer.storeTP;

                    SendMessagesToCombatLog(
                    BM.activePlayer.name + " hits the enemy!");                    
                    CreateDamagePopUp(BM.playerTarget.battleSprite.transform.position, BM.activePlayer.Attack.ToString(), Color.white);
                    CreateDamagePopUp(BM.activePlayer.battleSprite.transform.position, BM.activePlayer.storeTP.ToString() + " TP", Color.yellow);
                }
                else
                {
                    SendMessagesToCombatLog(
                    BM.activePlayer.name + " misses the enemy...");
                    CreateDamagePopUp(BM.playerTarget.battleSprite.transform.position, "Miss!", Color.white);
                }
                break;
            case "PlayerWait":
                SendMessagesToCombatLog(
                    BM.activePlayer.name + " waits.");
                break;
            case "PlayerStartCast":

                BM_Funcs.setPlayerOrEnemyTargetFromID(BM.activePlayer, null);

                SendMessagesToCombatLog(
                    BM.activePlayer.name + " starts casting " + BM.activePlayer.activeSpell.name + " on " + BM.playerTarget.EnemyName + ".");
                break;
            case "PlayerFinishCast":
                while (spellReportFinished == false)
                {
                    BM_Funcs.setPlayerOrEnemyTargetFromID(BM.activePlayer, null);

                    SendMessagesToCombatLog(
                        BM.activePlayer.name + " casts " + BM.activePlayer.activeSpell.name + " on the " + BM.playerTarget.EnemyName + "!");
                    spellReportFinished = true;
                }
                break;
            case "SkillChain":

                BM_Funcs.setPlayerOrEnemyTargetFromID(BM.activePlayer, null);                

                SendMessagesToCombatLog(
                        BM.activePlayer.name + " closes a SKILLCHAIN to create Scission!");
                CreateDamagePopUp(BM.activePlayer.battleSprite.transform.position, "Scission!", Color.blue);                
                break;
            case "EnemyAttack":

                BM_Funcs.setPlayerOrEnemyTargetFromID(null, BM.activeEnemy);

                float enemyRandom = Random.Range(1, 101);
                float enemyOutcome = BM.activeEnemy.Accuracy + enemyRandom;

                SendMessagesToCombatLog(
                        BM.activeEnemy.EnemyName + "'s hit score is " + enemyOutcome + " (" + enemyRandom + " + " + BM.activeEnemy.Accuracy + " acc)" +
                        " vs " + BM.enemyTarget.name + "'s evasion of " + BM.enemyTarget.Evasion + ".");

                if (enemyOutcome > BM.enemyTarget.Evasion)
                {
                    SendMessagesToCombatLog(
                    BM.activeEnemy.EnemyName + " hits " + BM.enemyTarget.name + "...");
                    CreateDamagePopUp(BM.enemyTarget.battleSprite.transform.position, BM.activeEnemy.Attack.ToString(), Color.white);

                }
                else
                {
                    SendMessagesToCombatLog(
                    BM.activeEnemy.EnemyName + " misses " + BM.enemyTarget.name + "!");
                    CreateDamagePopUp(BM.enemyTarget.battleSprite.transform.position, "Miss!", Color.white);
                }
                break;
            case "EnemyStartCast":

                BM_Funcs.setPlayerOrEnemyTargetFromID(null, BM.activeEnemy);

                SendMessagesToCombatLog(
                    BM.activeEnemy.EnemyName + " starts casting " + BM.activeEnemy.activeSpell.name + " on " + BM.enemyTarget.name + ".");
                break;
            case "EnemyFinishCast":
                while (enemySpellReportFinished == false)
                {
                    BM_Funcs.setPlayerOrEnemyTargetFromID(null, BM.activeEnemy);

                    SendMessagesToCombatLog(
                        BM.activeEnemy.EnemyName + " casts " + BM.activeEnemy.activeSpell.name + " on " + BM.enemyTarget.name + "!");
                    enemySpellReportFinished = true;
                }

                break;
            default:
                break;
        }
    }

    //This deals with the coroutine and animation
    public void resolveAction(string Command)
    {
        switch (Command)
        {
            case "Attack":
                BM_Funcs.setPlayerOrEnemyTargetFromID(BM.activePlayer, null);
                BM.attackAnimCoroutineIsPaused = false;
                StartCoroutine(BM_Enums.waitForAttackAnimation());
                BM_Funcs.animationController(BM.activePlayer, "IsAttacking");
                BM_Funcs.enemyAnimationController(BM.playerTarget, "TakeDamage");

                if (BM.attackAnimIsDone == true)
                {
                    BM.attackAnimCoroutineIsPaused = true;
                    BM_Funcs.enemyAnimationController(BM.playerTarget);
                    BM.activePlayer.speedTotal -= 100f;
                    resolveAction(default);
                }

                break;
            case "Magic":
                BM_Funcs.setPlayerOrEnemyTargetFromID(BM.activePlayer, null);
                reportOutcome("PlayerStartCast");
                BM_Funcs.animationController(BM.activePlayer, "IsChanting");
                BM.activePlayer.constantAnimationState = "IsChanting";
                BM.activePlayer.hasConstantAnimationState = true;
                BM.activePlayer.playerCastBar.SetActive(true);
                BM.activePlayer.castSpeedTotal = BM.activePlayer.activeSpell.castTime;
                BM.activePlayer.speedTotal -= 100f;
                resolveAction(default);
                break;
            case "Wait":
                reportOutcome("PlayerWait");
                BM.activePlayer.speedTotal = (100f - BM.activePlayer.speed);
                resolveAction(default);
                break;
            case "Change Row":
                if (BM.isSwitchingWithOtherPlayer)
                {
                    BM.playerToSwitchRowWith.speedTotal -= 100f;
                    BM.isSwitchingWithOtherPlayer = false;
                }
                BM.playerToSwitchRowWith = null;
                BM.activePlayer.speedTotal -= 100f;
                resolveAction(default);
                break;
            case "Weapon Skill":                
                
                BM_Funcs.setPlayerOrEnemyTargetFromID(BM.activePlayer, null);
                BM.WSAnimCoroutineIsPaused = false;
                StartCoroutine(BM_Enums.waitForWeaponSkillAnimation(1.8f));
                BM_Funcs.enemyAnimationController(BM.playerTarget, "TakeDamage");

                if (BM.WSAnimIsDone == true)
                {
                    BM.WSAnimIsDone = false;
                    BM.WSAnimCoroutineIsPaused = true;
                    reportOutcome("PlayerAttack");
                    BM_Funcs.enemyAnimationController(BM.playerTarget);
                    BM.activePlayer.speedTotal -= 100f;
                    BM.activePlayer.tpTotal = 0;
                    if (BM.activePlayer.selectedWeaponSkill.willCreateSkillchain == true)
                    {
                        StopAllCoroutines();
                        reportOutcome("SkillChain");
                        StartCoroutine(BM_Enums.waitForSkillChainAnimation(1.5f));
                        BM.SCAnimCoroutineIsPaused = false;
                        resolveAction("Skillchain");
                        break;
                    }
                    else
                    {
                        resolveAction(default);
                        break;
                    }                    
                }

                break;
            case "Skillchain":
                
                BM.activePlayer.battleSprite.GetComponent<Animator>().SetBool("IsFastBlade", false);
                BM.activePlayer.battleSprite.GetComponent<Animator>().SetBool("IsReady", true);
                BM_Funcs.setPlayerOrEnemyTargetFromID(BM.activePlayer, null);                                
                BM_Funcs.enemyAnimationController(BM.playerTarget, "TakeDamage");

                if (BM.SCAnimIsDone == true)
                {
                    BM.SCAnimIsDone = false;
                    BM.SCAnimCoroutineIsPaused = true;                    
                    BM_Funcs.enemyAnimationController(BM.playerTarget);                                       
                    BM.activePlayer.selectedWeaponSkill.willCreateSkillchain = false;
                    resolveAction(default);
                }

                break;
            case "Cast":
                BM_Funcs.setPlayerOrEnemyTargetFromID(BM.activePlayer, null);
                BM.castAnimCoroutineIsPaused = false;
                StartCoroutine(BM_Enums.waitForCastAnimation());
                BM_Funcs.animationController(BM.activePlayer, "IsCasting");

                if (BM.castAnimIsDone)
                {
                    spellReportFinished = false;
                    BM.castAnimCoroutineIsPaused = true;
                    BM.activePlayer.constantAnimationState = null;
                    BM.activePlayer.hasConstantAnimationState = false;
                    BM.activePlayer.isCastingSpell = false;
                    BM.activePlayer.castSpeedTotal = 0f;
                    BM.activePlayer.playerOptions.Remove("Cast");
                    resolveAction(default);
                }
                break;
            default:
                StopAllCoroutines();
                BM.WSAnimIsDone = false;
                BM.WSAnimCoroutineIsPaused = true;
                BM.SCAnimIsDone = false;
                BM.SCAnimCoroutineIsPaused = true;
                BM_Funcs.standIdle(BM.activePlayer);
                BM_Funcs.animationController(BM.activePlayer);
                BM.activePlayer.playerPanel.GetComponent<Image>().color = BM.defaultColor;
                BM_Funcs.resetChoicePanel();
                BM.ActionPanel.SetActive(false);
                BM.OptionPanel.SetActive(false);
                BM.ActivePlayers.Remove(BM.activePlayer);
                BM.activePlayer = null;
                BM.selectedCommand = null;

                if (BM.ActivePlayers.Count == 0)
                {
                    BM.returningStarting = true;
                    BM.startRoutinesGoingAgain = true;
                    BM_Funcs.redirectAction();
                }
                else
                {
                    BM.battleStates = Battle_Manager.BattleStates.SELECT_PLAYER;
                }
                break;
        }
    }

    // Create a Damage PopUp
    public DamagePopUp CreateDamagePopUp(Vector3 position, string damageAmount, Color color)
    {
        Transform damagePopupTransform = Instantiate(floatingDamage.transform, position, Quaternion.identity);

        DamagePopUp damagePopUp = damagePopupTransform.GetComponent<DamagePopUp>();
        damagePopUp.Setup(damageAmount, color);

        return damagePopUp;
    }

    public void SendMessagesToCombatLog(string text)
    {
        if (messageList.Count >= maxMessages)
        {
            Destroy(messageList[0].textObject.gameObject);
            messageList.Remove(messageList[0]);
        }

        Message newMessage = new Message();
        newMessage.text = text;
        GameObject newText = Instantiate(textObject, chatPanel.transform);
        newMessage.textObject = newText.GetComponent<TextMeshProUGUI>();
        newMessage.textObject.text = newMessage.text;
        messageList.Add(newMessage);
    }
}
