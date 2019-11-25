using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BattleManager : MonoBehaviour
{

    public static BattleManager instance;

    private bool battleActive;
    public GameObject battleScene;

    public Transform[] playerPositions;
    public Transform[] enemyPositions;

    public BattleChar[] playerPrefabs;
    public BattleChar[] enemyPrefabs;

    public List<BattleChar> activeBattlers = new List<BattleChar>();

    public int curretTurn;
    public bool turnWaiting;

    public BattleMove[] moveList;

    public GameObject uiButtonsHolder;

    public GameObject enemyAttackEffect;

    public DameNumber theDamageNumber;

    public GameObject targetMenu;
    public BattleTargetButton[] targetButtons;

    public Text[] playerName, playerHP, playerMP;

    public GameObject magicMenu;
    public BattleMagicSelect[] magicButtons;
    public BattleNotification battleNotice;
    public int chanceToFlee = 35;
    public int muertos = 0;
    public string gameOverScene;

    private bool fleeing;
    public int rewardXP;
    public string[] rewardItems;

    public bool cannotFlee;

    // Use this for initialization
    void Start()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {

        //if (Input.GetKeyDown(KeyCode.M))
        //{
        //    BattleStart(new string[] { "EyeBall" }, false);
        //}

        if (battleActive)
        {
            if (turnWaiting)
            {
                if (activeBattlers[curretTurn].isPlayer)
                {
                    uiButtonsHolder.SetActive(true);
                }
                else
                {
                    uiButtonsHolder.SetActive(false);

                    //enemy should attack
                    StartCoroutine(EnemyMoveCo());
                }
            }
        }

    }

    public void BattleStart(string[] enemiesToSpawn, bool setCannotFlee)
    {

        if (!battleActive)
        {
            cannotFlee = setCannotFlee;
            battleActive = true;

            GameManager.instance.battleActive = true;
            transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, transform.position.z);
            battleScene.SetActive(true);

            AudioManager.instance.PlayBGM(0);

            for (int i = 0; i < playerPositions.Length; i++)
            {
                if (GameManager.instance.playerStats[i].gameObject.activeInHierarchy)
                {
                    for (int j = 0; j < playerPrefabs.Length; j++)
                    {
                        if (playerPrefabs[j].charName == GameManager.instance.playerStats[i].charName)
                        {
                            BattleChar newPlayer = Instantiate(playerPrefabs[j], playerPositions[i].position, playerPositions[i].rotation);
                            newPlayer.transform.parent = playerPositions[i];
                            activeBattlers.Add(newPlayer);

                            CharStats thePlayer = GameManager.instance.playerStats[i];
                            activeBattlers[i].currentHP = thePlayer.currentHP;
                            activeBattlers[i].maxHP = thePlayer.maxHP;
                            activeBattlers[i].currentMP = thePlayer.currentMP;
                            activeBattlers[i].maxMP = thePlayer.maxMP;
                            activeBattlers[i].strength = thePlayer.strenght;
                            activeBattlers[i].defence = thePlayer.defence;
                            activeBattlers[i].wpnPower = thePlayer.wpnPwr;
                            activeBattlers[i].armrPower = thePlayer.armrPwr;
                        }
                    }


                }
            }

            for (int i = 0; i < enemiesToSpawn.Length; i++)
            {
                if (enemiesToSpawn[i] != "")
                {
                    for (int j = 0; j < enemyPrefabs.Length; j++)
                    {
                        if (enemyPrefabs[j].charName == enemiesToSpawn[i])
                        {
                            BattleChar newEnemy = Instantiate(enemyPrefabs[j], enemyPositions[i].position, enemyPositions[i].rotation);
                            newEnemy.transform.parent = enemyPositions[i];
                            activeBattlers.Add(newEnemy);
                        }
                    }
                }
            }

            turnWaiting = true;
            curretTurn = Random.Range(0, activeBattlers.Count);
            UpdateUIStats();
        }
    }
    public void NextTurn()
    {
        curretTurn++;
        if (curretTurn >= activeBattlers.Count)
        {
            curretTurn = 0;
        }

        turnWaiting = true;
        UpdateBattle();
        UpdateUIStats();
    }
    public void UpdateBattle()
    {
        bool allEnemiesDead = false;
        bool allPlayersDead = false;
        

        for (int i = 0; i < activeBattlers.Count; i++)
        {
            if (activeBattlers[i].currentHP < 0)
            {
                activeBattlers[i].currentHP = 0;
            }
            if (activeBattlers[i].currentHP == 0)
            {
                //handle dead battler
                if(activeBattlers[i].isPlayer)
                {
                    activeBattlers[i].theSprite.sprite = activeBattlers[i].deadSprite;
                    activeBattlers[i].hasDied = true;
                    muertos++;
                    if(muertos == 1)
                    {
                        allPlayersDead = true;
                    }
                }
                else
                {
                    activeBattlers[i].EnemyFade();
                    activeBattlers[i].hasDied = true;
                }
            }
            else
            {
                if (activeBattlers[i].isPlayer)
                {
                    allEnemiesDead = true;
                    activeBattlers[i].theSprite.sprite = activeBattlers[i].aliveSrpite;
                }
                else
                {
                    allEnemiesDead = false;
                }
            }
        }

        if (allEnemiesDead || allPlayersDead)
        {
            if (allEnemiesDead)
            {
                //end battle in victory
                StartCoroutine(EndBattleCo());
            }
            else
            {
                //end battle in failure
                StartCoroutine(GameOverCo());
            }

            /*battleScene.SetActive(false);
            GameManager.instance.battleActive = false;
            battleActive = false;*/
        }else
        {
            while (activeBattlers[curretTurn].currentHP == 0)
            {
                curretTurn++;
                if (curretTurn >= activeBattlers.Count)
                {
                    curretTurn = 0;
                }
            }
        }
    }

    public IEnumerator EnemyMoveCo()
    {
        turnWaiting = false;
        yield return new WaitForSeconds(1f);
        EnemyAttack();
        yield return new WaitForSeconds(1f);
        NextTurn();
    }

    public void EnemyAttack()
    {
        List<int> players = new List<int>();
        for (int i = 0; i < activeBattlers.Count; i++)
        {
            if (activeBattlers[i].isPlayer && activeBattlers[i].currentHP > 0)
            {
                players.Add(i);
            }
        }
        int selectedTarget = players[Random.Range(0, players.Count)];

        //activeBattlers[selectedTarget].currentHP -= 30;

        int selectAttack = Random.Range(0, activeBattlers[curretTurn].movesAvailable.Length);
        int movePower = 0;
        for (int i = 0; i < moveList.Length; i++)
        {
            if (moveList[i].moveName == activeBattlers[curretTurn].movesAvailable[selectAttack])
            {
                Instantiate(moveList[i].theEffect, activeBattlers[selectedTarget].transform.position, activeBattlers[selectedTarget].transform.rotation);
                movePower = moveList[i].movePower;
            }
        }

        Instantiate(enemyAttackEffect, activeBattlers[curretTurn].transform.position, activeBattlers[curretTurn].transform.rotation);
        DealDamage(selectedTarget, movePower);
    }

    public void DealDamage(int target, int movePower)
    {
        float atkPwr = activeBattlers[curretTurn].strength + activeBattlers[curretTurn].wpnPower;
        float defPwr = activeBattlers[target].defence + activeBattlers[target].armrPower;

        float damageCalc = (atkPwr / defPwr) * movePower * Random.Range(0.9f, 1.1f);
        int damgeToGive = Mathf.RoundToInt(damageCalc);

        //Debug.Log(activeBattlers[curretTurn].charName + "is dealing" + damageCalc + "(" + damgeToGive + ") damage to" + activeBattlers[target].charName);

        activeBattlers[target].currentHP -= damgeToGive;

        Instantiate(theDamageNumber, activeBattlers[target].transform.position, activeBattlers[target].transform.rotation).SetDamage(damgeToGive);

        UpdateUIStats();
    }

    public void UpdateUIStats()
    {
        for (int i = 0; i < playerName.Length; i++)
        {
            if (activeBattlers.Count > i)
            {
                if (activeBattlers[i].isPlayer)
                {
                    BattleChar playerData = activeBattlers[i];

                    playerName[i].gameObject.SetActive(true);
                    playerName[i].text = playerData.charName;
                    playerHP[i].text = Mathf.Clamp(playerData.currentHP, 0, int.MaxValue) + "/" + playerData.maxHP;
                    playerMP[i].text = Mathf.Clamp(playerData.currentMP, 0, int.MaxValue) + "/" + playerData.maxMP;

                }
                else
                {
                    playerName[i].gameObject.SetActive(false);
                }
            }
            else
            {
                playerName[i].gameObject.SetActive(false);
            }

        }
    }

    public void PlayerAttack(string moveName, int selectedTarget)
    {
        //int selectedTarget = 2;

        int movePower = 0;
        for (int i = 0; i < moveList.Length; i++)
        {
            if (moveList[i].moveName == moveName)
            {
                Instantiate(moveList[i].theEffect, activeBattlers[selectedTarget].transform.position, activeBattlers[selectedTarget].transform.rotation);
                movePower = moveList[i].movePower;
            }
        }

        Instantiate(enemyAttackEffect, activeBattlers[curretTurn].transform.position, activeBattlers[curretTurn].transform.rotation);
        DealDamage(selectedTarget, movePower);

        uiButtonsHolder.SetActive(false);
        targetMenu.SetActive(false);
        NextTurn();

    }


    public void OpenTargetMenu(string moveName)
    {
        targetMenu.SetActive(true);

        List<int> Enemies = new List<int>();

        for (int i = 0; i < activeBattlers.Count; i++)
        {
            if (!activeBattlers[i].isPlayer)
            {
                Enemies.Add(i);
            }
        }

        for (int i = 0; i < targetButtons.Length; i++)
        {
            if (Enemies.Count > i && activeBattlers[Enemies[i]].currentHP > 0)
            {
                targetButtons[i].gameObject.SetActive(true);

                targetButtons[i].moveName = moveName;
                targetButtons[i].activeBattlerTarget = Enemies[i];
                targetButtons[i].targetName.text = activeBattlers[Enemies[i]].charName;
            }
            else
            {
                targetButtons[i].gameObject.SetActive(false);
            }
        }

    }
    public void OpenMagicMenu()
    {
        magicMenu.SetActive(true);

        for(int i = 0; i<magicButtons.Length;i++)
        {
            if(activeBattlers[curretTurn].movesAvailable.Length > i)
            {
                magicButtons[i].gameObject.SetActive(true);

                magicButtons[i].spellName = activeBattlers[curretTurn].movesAvailable[i];
                magicButtons[i].nameText.text = magicButtons[i].spellName;

                for(int j =0; j<moveList.Length;j++)
                {
                    if(moveList[j].moveName == magicButtons[i].spellName)
                    {
                        magicButtons[i].spellCost = moveList[j].moveCost;
                        magicButtons[i].costText.text = magicButtons[i].spellCost.ToString();
                    }
                }
            }else
            {
                magicButtons[i].gameObject.SetActive(false);
            }
        }
    }

    public void Flee()
    {
        if (cannotFlee)
        {
            battleNotice.theText.text = "Can not flee this battle!";
            battleNotice.Activate();
        }
        else
        {
            int fleeSuccess = Random.Range(0, 100);
            if (fleeSuccess < chanceToFlee)
            {
                //end the battle
                //battleActive = false;
                //GameManager.instance.battleActive = false;
                //battleScene.SetActive(false);
                fleeing = true;
                StartCoroutine(EndBattleCo());
            }
            else
            {
                NextTurn();
                battleNotice.theText.text = "Couldn't escape!";
                battleNotice.Activate();
            }
        }
    }

    public IEnumerator EndBattleCo()
    {
        battleActive = false;
        uiButtonsHolder.SetActive(false);
        targetMenu.SetActive(false);
        magicMenu.SetActive(false);

        yield return new WaitForSeconds(.5f);

        UIFade.instance.FadeToBlack();

        yield return new WaitForSeconds(1.5f);

        for(int i=0; i<activeBattlers.Count;i++)
        {
            if(activeBattlers[i].isPlayer)
            {
                for(int j=0; j< GameManager.instance.playerStats.Length;j++)
                {
                    if(activeBattlers[i].charName == GameManager.instance.playerStats[j].charName)
                    {
                        GameManager.instance.playerStats[j].currentHP = activeBattlers[i].currentHP;
                        GameManager.instance.playerStats[j].currentMP = activeBattlers[i].currentMP;

                    }
                }
            }
            Destroy(activeBattlers[i].gameObject);
        }

        muertos = 0;
        UIFade.instance.FadeFromBlack();
        battleScene.SetActive(false);
        activeBattlers.Clear();
        curretTurn = 0;
        //GameManager.instance.battleActive = false;
        if(fleeing)
        {
            GameManager.instance.battleActive = false;
            fleeing = false;
        }else
        {
            BattleReward.instance.OpenRewardScreen(rewardXP, rewardItems);
        }

        AudioManager.instance.PlayBGM(FindObjectOfType<CameraController>().musicToPlay);
    }

    public IEnumerator GameOverCo()
    {
        muertos = 0;
        battleActive = false;
        UIFade.instance.FadeToBlack();
        yield return new WaitForSeconds(1.5f);
        battleScene.SetActive(false);
        SceneManager.LoadScene(gameOverScene);
    }
}
