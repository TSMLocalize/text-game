using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.EventSystems;
using TMPro;

[System.Serializable]
public class Action_Handler : MonoBehaviour
{
    public Battle_Manager BM;
    public Battle_Manager_Functions BM_Funcs;
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
    }

    [System.Serializable]
    public class Message
    {
        public string text;
        public TextMeshProUGUI textObject;
    }

    //GAMEPLAY FUNCTIONS

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
                    BM_Funcs.createFloatingText(BM.playerTarget.battleSprite.transform.position, BM.activePlayer.Attack.ToString());
                }
                else
                {
                    SendMessagesToCombatLog(
                    BM.activePlayer.name + " misses the enemy...");
                    BM_Funcs.createFloatingText(BM.playerTarget.battleSprite.transform.position, "Miss!");
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
                    BM_Funcs.createFloatingText(BM.enemyTarget.battleSprite.transform.position, BM.activeEnemy.Attack.ToString());

                }
                else
                {
                    SendMessagesToCombatLog(
                    BM.activeEnemy.EnemyName + " misses " + BM.enemyTarget.name + "!");
                    BM_Funcs.createFloatingText(BM.enemyTarget.battleSprite.transform.position, "Miss!");
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
