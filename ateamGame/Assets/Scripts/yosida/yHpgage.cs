using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class yHpgage : MonoBehaviour {

    GameObject parent;
    GameObject particle;
    Image hpGage, redGage;

    [SerializeField,Header("PlayerHP専用")]
    int hp = 150;
    int maxHP;
    [SerializeField,Header("HPのゲージが減る時間")]
    float hpGageSpeed = 0.01f;
    [SerializeField, Header("HPのゲージのfillAmountが減っていく値")]
    float hpGageDecrease = 0.01f;

    [SerializeField,Header("赤ゲージが減る時間")]
    float redGageSpeed = 0.01f;
    [SerializeField, Header("赤ゲージのfillAmountが減っていく値")]
    float redGageDecrease = 0.01f;


    yEnemyManager enemyManager;
    yWaveManagement waveManagement;

    // Use this for initialization
    void Start () {
        particle = Resources.Load("yResources/particle") as GameObject;
        //子オブジェクト取得
        hpGage = transform.FindChild("hpGage").gameObject.GetComponent<Image>();
        redGage = transform.FindChild("redGage").gameObject.GetComponent<Image>();
        //親オブジェクト取得
        parent = transform.root.gameObject;

        waveManagement = GameObject.Find("Wave").GetComponent<yWaveManagement>();

        //要はEnemyの時
        if (parent.name != "Canvas")
        {
            //Enemyの時、親オブジェクトのスクリプトを取得
            enemyManager = parent.GetComponent<yEnemyManager>();
            hp = enemyManager.EnemyHP;
        }
        maxHP = hp;
    }

    // Update is called once per frame
    void Update () {
        //デバッグ用
        if (Input.GetMouseButtonDown(0))
        {
            PlayerDamage(30);
        }
        if (Input.GetMouseButtonDown(1))
        {
            EnemyDamage(10);
        }
    }

    public void PlayerDamage(int x)
    {
        if (parent.tag != "enemy")
        {
            StopCoroutine("DamageCoroutine");
            StopCoroutine("ComboEnd");
            StartCoroutine("DamageCoroutine", x);
        }
    }

    public void EnemyDamage(int x)
    {
        if (parent.tag == "enemy")
        {
            StopCoroutine("DamageCoroutine");
            StopCoroutine("ComboEnd");
            StartCoroutine("DamageCoroutine", x);
        }
    }


    private IEnumerator DamageCoroutine(int x)
    {
        float remaining = ((float)hp - x) / maxHP;
        hp -= x;
        while (true)
        {
            if (hpGage.fillAmount <= 0)
            {
                if(parent.tag == "enemy")
                    parent.tag = "deathEnemy";
                GameObject _particle = Instantiate(particle,parent.transform.position,Quaternion.identity) as GameObject;
                Destroy(_particle, 1.0f);
                yield return StartCoroutine("ComboEnd");
                break;
            }
            //HPgageが少しずつ減っていく
            if (hpGage.fillAmount > remaining)
            {
                hpGage.fillAmount -= hpGageDecrease;
            }
            else
            {
                yield return StartCoroutine("ComboEnd");
                break;
            }
            yield return new WaitForSeconds(hpGageSpeed);
        }
        yield break;
    }

    IEnumerator ComboEnd()
    {
        float remaining = redGage.fillAmount;

        while (true)
        {
            if (hpGage.fillAmount < redGage.fillAmount)
            {
                redGage.fillAmount -= redGageDecrease;
            }
            else
            {
                redGage.fillAmount = hpGage.fillAmount;
                break;
            }

            if(redGage.fillAmount <= 0)
            {
                if (parent.tag == "deathEnemy")
                {
                    waveManagement.enemyNumber[waveManagement.WaveNumber - 1]--;
                    Destroy(parent.gameObject);
                }

            }
            yield return new WaitForSeconds(redGageSpeed);
        }
        yield break;
    }

}
