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
    public Spells spells;
    public Battle_Manager BM;
    public Battle_Manager_Functions BM_Funcs;
    public Combo_Manager combo_Manager;
    public Animation_Handler animHandler;
    public Battle_Manager_IEnumerators BM_Enums;
    public bool spellReportFinished;
    public bool enemySpellReportFinished;
    public int maxMessages;
    public List<Message> messageList;
    public GameObject chatPanel;
    public GameObject textObject;
    public GameObject statusAilment;
    public List<StatusAilment> statusAilmentList;

    // Start is called before the first frame update
    void Start()
    {
        maxMessages = 25;
        messageList = new List<Message>();        
        BM = GetComponent<Battle_Manager>();
        BM_Funcs = GetComponent<Battle_Manager_Functions>();
        BM_Enums = GetComponent<Battle_Manager_IEnumerators>();
        combo_Manager = GetComponent<Combo_Manager>();
        animHandler = GetComponent<Animation_Handler>();
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

                if (BM.activePlayer.activeSpell.isSupport == true)
                {
                    BM_Funcs.setPlayerOrEnemyTargetFromID(null, null, BM.activePlayer);
                    SendMessagesToCombatLog(
                        BM.activePlayer.name + " starts casting " + BM.activePlayer.activeSpell.name + " on " + BM.supportTarget.name + ".");
                }                    
                else
                {
                    BM_Funcs.setPlayerOrEnemyTargetFromID(BM.activePlayer, null);
                    SendMessagesToCombatLog(
                    BM.activePlayer.name + " starts casting " + BM.activePlayer.activeSpell.name + " on " + BM.playerTarget.EnemyName + ".");
                }
                
                break;                
            case "PlayerFinishCast":
                                
                while(spellReportFinished == false)
                {
                    spells.CastSpell(BM.activePlayer.activeSpell.methodID);
                    
                    spellReportFinished = true;
                }                

                break;
            case "Weapon Skill":

                StartCoroutine(delayWSMessageReports());

                IEnumerator delayWSMessageReports()
                {                    
                    BM_Funcs.setPlayerOrEnemyTargetFromID(BM.activePlayer, null);

                    float WSrandom = Random.Range(1, 101);
                    float WSoutcome = BM.activePlayer.Accuracy + WSrandom;

                    SendMessagesToCombatLog(
                            BM.activePlayer.name + "'s hit score is " + WSoutcome + " (" + WSrandom + " + " + BM.activePlayer.Accuracy + " acc)" +
                            " vs " + BM.playerTarget.EnemyName + "'s evasion of " + BM.playerTarget.Evasion + ".");

                    if (WSoutcome > BM.playerTarget.Evasion)
                    {
                        BM.activePlayer.tpTotal += 10 + BM.activePlayer.storeTP;

                        SendMessagesToCombatLog(
                        BM.activePlayer.name + " hits the enemy!");
                        CreateDamagePopUp(BM.playerTarget.battleSprite.transform.position, BM.activePlayer.Attack.ToString(), Color.white);                        
                    }
                    else
                    {
                        SendMessagesToCombatLog(
                        BM.activePlayer.name + " misses the enemy...");
                        CreateDamagePopUp(BM.playerTarget.battleSprite.transform.position, "Miss!", Color.white);
                    }
                    
                    yield return new WaitForSeconds(BM.activePlayer.selectedWeaponSkill.wsMultiAttackReportTimer);

                    if (BM.activePlayer.selectedWeaponSkill.spentAttacks > 1)
                    {
                        BM.activePlayer.selectedWeaponSkill.spentAttacks = BM.activePlayer.selectedWeaponSkill.spentAttacks - 1;
                        StartCoroutine(delayWSMessageReports());
                    }
                    else
                    {
                        StopCoroutine(delayWSMessageReports());
                    }
                }

                break;
            case "SkillChain":

                BM_Funcs.setPlayerOrEnemyTargetFromID(BM.activePlayer, null);                

                SendMessagesToCombatLog(
                        BM.activePlayer.name + " closes a SKILLCHAIN to create " + combo_Manager.skillChainToCreate.name + "!");
                CreateDamagePopUp(BM.playerTarget.battleSprite.transform.position, combo_Manager.skillChainToCreate.name + "!", Color.blue);                
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
                    CreateStatusAilment(BM.enemyTarget.battleSprite, 12, "poison");
                    animHandler.animationController(BM.enemyTarget, "IsCritical");
                    BM.enemyTarget.constantAnimationStates.Add("IsCritical");
                    enemySpellReportFinished = true;
                }

                break;
            default:
                break;
        }
    }

    //This deals with wait coroutines and animations
    public void resolveAction(string Command)
    {
        switch (Command)
        {
            case "Attack":

                BM_Funcs.setPlayerOrEnemyTargetFromID(BM.activePlayer, null);
                StartCoroutine(waitForAttackAnimation());
                animHandler.animationController(BM.activePlayer, "IsAttacking");
                animHandler.enemyAnimationController(BM.playerTarget, "TakeDamage");

                IEnumerator waitForAttackAnimation()
                {
                    yield return new WaitForSeconds(1f);
                    animHandler.enemyAnimationController(BM.playerTarget);
                    BM.activePlayer.speedTotal -= 100f;
                    resolveAction(default);
                }                               

                break;
            case "Magic":
                if (BM.activePlayer.activeSpell.isSupport == true)
                    BM_Funcs.setPlayerOrEnemyTargetFromID(null, null, BM.activePlayer);                
                else
                    BM_Funcs.setPlayerOrEnemyTargetFromID(BM.activePlayer, null);

                reportOutcome("PlayerStartCast");
                animHandler.animationController(BM.activePlayer, "IsChanting");
                BM.activePlayer.constantAnimationStates.Add("IsChanting");
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
                animHandler.animationController(BM.activePlayer, "IsFastBlade");
                StartCoroutine(waitForWeaponSkillAnimation());
                animHandler.enemyAnimationController(BM.playerTarget, "TakeDamage");

                IEnumerator waitForWeaponSkillAnimation()
                {
                    yield return new WaitForSeconds(BM.activePlayer.selectedWeaponSkill.wsAnimTimer);                                        
                    animHandler.enemyAnimationController(BM.playerTarget);
                    animHandler.animationController(BM.activePlayer, "Ready");
                    BM.activePlayer.speedTotal -= 100f;
                    BM.activePlayer.tpTotal = 0;

                    if (BM.activePlayer.selectedWeaponSkill.willCreateSkillchain == true)
                    {
                        
                        reportOutcome("SkillChain");
                        resolveAction("Skillchain");                        
                    }
                    else
                    {                        
                        resolveAction(default);
                    }
                }

                break;
            case "Skillchain":
                StopAllCoroutines();

                StartCoroutine(waitForSkillChainAnimation());
                animHandler.enemyAnimationController(BM.playerTarget, "TakeDamage");

                IEnumerator waitForSkillChainAnimation()
                {                                        
                    yield return new WaitForSeconds(1.5f);                    
                    animHandler.enemyAnimationController(BM.playerTarget);
                    BM.activePlayer.selectedWeaponSkill.willCreateSkillchain = false;
                    resolveAction(default);
                    StopCoroutine(waitForSkillChainAnimation());
                }

                break;
            case "Cast":
                
                StartCoroutine(waitForCastAnimation());
                
                animHandler.animationController(BM.activePlayer, "IsCasting");
                BM.activePlayer.constantAnimationStates.Remove("IsChanting");

                IEnumerator waitForCastAnimation()
                {                    
                    yield return new WaitForSeconds(1f);
                    for (int i = 0; i < BM.EnemiesInBattle.Count; i++)
                    {
                        animHandler.enemyAnimationController(BM.EnemiesInBattle[i]);
                    }
                    for (int i = 0; i < BM.PlayersInBattle.Count; i++)
                    {
                        animHandler.animationController(BM.PlayersInBattle[i]);    
                    }                    
                    BM.activePlayer.isCastingSpell = false;
                    BM.activePlayer.activeSpell = null;
                    BM.activePlayer.castSpeedTotal = 0f;
                    BM.activePlayer.playerOptions.Remove("Cast");
                    spellReportFinished = false;
                    resolveAction(default);
                }
                               
                break;
            default:
                StopAllCoroutines();                
                BM_Funcs.standIdle(BM.activePlayer);
                animHandler.animationController(BM.activePlayer);
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
    public DamagePopUp CreateDamagePopUp(Vector3 position, string damageAmount, Color color, string fontsize = null)
    {
        Transform damagePopupTransform = Instantiate(floatingDamage.transform, position, Quaternion.identity);

        DamagePopUp damagePopUp = damagePopupTransform.GetComponent<DamagePopUp>();
        damagePopUp.Setup(damageAmount, color);

        return damagePopUp;
    }

    //Create a status ailment 
    public void CreateStatusAilment(GameObject target, int timeRemaining, string type)
    {        
        GameObject statusAilmentGameObject = Instantiate(statusAilment, target.transform);

        StatusAilment statusAilmentToAdd = statusAilmentGameObject.GetComponentInChildren<StatusAilment>();

        statusAilmentList.Add(statusAilmentToAdd);
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
