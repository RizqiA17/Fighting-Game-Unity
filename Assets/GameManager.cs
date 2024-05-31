using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private CharController player;
    [SerializeField] private CharController enemy;
    public TextMeshProUGUI countDownText;
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
}
