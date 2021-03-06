﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class yHpgage : MonoBehaviour {

    GameObject parent;
    Image hpGage, redGage;

    [SerializeField,Header("ここから設定するならPlayer専用")]
    int hp = 150;
    int maxHP;

    yEnemyManager enemyManager;
    yWaveManagement waveManagement;

    // Use this for initialization
    void Start () {
        //子オブジェクト取得
        hpGage = transform.FindChild("hpGage").gameObject.GetComponent<Image>();
        redGage = transform.FindChild("redGage").gameObject.GetComponent<Image>();
        //親オブジェクト取得
        parent = transform.root.gameObject;

        try
        {
            waveManagement = GameObject.Find("Wave").GetComponent<yWaveManagement>();
        }
        catch(Exception e)
        {
            print("見つからない");
        }
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
            EnemyDamage(20);
        }
    }

    public void PlayerDamage(int x)
    {
        if (parent.name == "Canvas")
        {
            StopCoroutine("DamageCoroutine");
            StopCoroutine("ComboEnd");
            StartCoroutine("DamageCoroutine", x);
        }
    }

    public void EnemyDamage(int x)
    {
        if (parent.name != "Canvas")
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
                yield return StartCoroutine("ComboEnd");
                break;
            }
            //HPgageが少しずつ減っていく
            if (hpGage.fillAmount > remaining)
            {
                hpGage.fillAmount -= 0.01f;
            }
            else
            {
                yield return StartCoroutine("ComboEnd");
                break;
            }
            yield return new WaitForSeconds(0.01f);
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
                redGage.fillAmount -= 0.01f;
            }
            else
            {
                redGage.fillAmount = hpGage.fillAmount;
                break;
            }

            if(redGage.fillAmount == 0)
            {
                if (parent.name != "Canvas")
                {
                    waveManagement.enemyNumber[waveManagement.WaveNumber - 1]--;
                    Destroy(parent.gameObject);
                }

            }
            yield return new WaitForSeconds(0.01f);
        }
        yield break;
    }

}
