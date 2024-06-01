using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private CharController player;
    [SerializeField] private Health playerHealth;
    [SerializeField] private CharController enemy;
    [SerializeField] private Health enemyHealth;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI gameCondition;
    [SerializeField] private TextMeshProUGUI countDownText;
    [SerializeField] private TextMeshProUGUI totalAttack;
    [SerializeField] private TextMeshProUGUI attackSuccess;
    float countDown;
    float timer = 3;
    //public TextMeshProUGUI rankText;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(CountDownFight());
        countDown = timer;
    }

    // Update is called once per frame
    void Update()
    {
        if (countDown > -2)
        {
            countDown -= Time.deltaTime;
            if (countDown <= 3) countDownText.text = "3";
            if (countDown <= 2) countDownText.text = "2";
            if (countDown <= 1) countDownText.text = "1";
            if (countDown <= 0) countDownText.text = "GO!!!";
            if (countDown <= -0.9f) countDownText.text = "";
        }
    }

    IEnumerator CountDownFight()
    {
        yield return new WaitForSeconds(4f);
        player.enabled = true;
        enemy.enabled = true;
    }

    public void EndGame()
    {
        if (playerHealth.CurrentHealth > enemyHealth.CurrentHealth) gameCondition.text = "<color=green>You Win</color>";
        else if (playerHealth.CurrentHealth < enemyHealth.CurrentHealth) gameCondition.text = "<color=red>You Lose</color>";
        else if (playerHealth.CurrentHealth == enemyHealth.CurrentHealth) gameCondition.text = "<color=yellow>Draw</color>";
        totalAttack.text = "Total Attack = " + player.totalAttack;
        attackSuccess.text = "Attack Success = " + player.attackSuccess;
        gameOverPanel.SetActive(true);
        player.enabled = false;
        enemy.enabled = false;
    }
}
