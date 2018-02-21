using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class yWaveManagement : MonoBehaviour {
    enum topRow : int { ID = 0, Stage, Wave, Pos, Time ,HP};

    [SerializeField,Header("csvのIDにSprite.name入れなきゃ動かないよ")]
    SpriteRenderer[] enemyType;

    Vector3[] enemyPos;

    int i = 0, j = 0;
    int stageNumber = 1;
    int waveNumber = 1;
    int maxWave = 0;

    [System.NonSerialized]
    public int[] enemyNumber;//Wave毎ごとの敵の数、死んだら減っていく
    int[] enemyHP;
    int number;//そのWave毎ごとに出現する敵の数、出現したら減っていく
    int wholeNumber;//全ての敵の数

    float[] enemyAppearanceTime;//敵が出てくる時間
    float time = 0;

    bool flgNumber = true;
    bool flgBoss = false;//trueでボスが出現


    string[] enemyID;
    yCsvRender csv;
    yEnemyManager enemyManager;
    yBandFade bandFade;
    yTime _time;

    public int WaveNumber
    {
        set { waveNumber = value; }
        get { return waveNumber; }
    }
    public int WaveMax
    {
        set { maxWave = value; }
        get { return maxWave; }
    }
    // Use this for initialization
    void Start() {
        csv = GameObject.Find("Reference").GetComponent<yCsvRender>();
        bandFade = GameObject.Find("Canvas/Band").GetComponent<yBandFade>();
        _time = GameObject.Find("Time").GetComponent<yTime>();

        MaxWave((int)topRow.Wave);//最大Wave取得
        EnemyNumber((int)topRow.Wave);//WaveごとのEnemyの数
        EnemyTime((int)topRow.Time);//Enemyが出てくる時間
        EnemyPos((int)topRow.Pos);//Enemyが出てくる場所
        EnemyID((int)topRow.ID);//どのEnemyか
        EnemyHP((int)topRow.HP);//EnemyのHP
        number = enemyNumber[0];
        for (int i = 0; i < maxWave; i++)
            wholeNumber += enemyNumber[i];
        print(wholeNumber);
        StartCoroutine("BossAppearance");

    }

    // Update is called once per frame
    void Update()
    {
        if(_time.FlgTime)
            time += Time.deltaTime;


        if (waveNumber < maxWave)//ボス戦前のWave数まで
        {
            if (flgNumber && j < maxWave - 1)
            {
                if (time >= enemyAppearanceTime[i])//時間になったら生成
                {
                    while (true)
                    {
                        for(int k = 0;k < enemyType.Length; k++)
                        {
                            if(enemyID[i] == enemyType[k].name)
                            {
                                SpriteRenderer enemy;
                                enemy = Instantiate(enemyType[k], enemyPos[i], Quaternion.identity) as SpriteRenderer;
                                enemy.name = enemyID[i] + number;
                                enemyManager = enemy.GetComponent<yEnemyManager>();
                                enemyManager.EnemyHP = enemyHP[i];
                                break;
                            }
                        }

                        i++;
                        number--;

                        if (i >= wholeNumber)//全ての敵が出現し終えたら(1～3Wave)
                            break;
                        else if (enemyAppearanceTime[i - 1] == enemyAppearanceTime[i])//今作ったものと次作る秒数が一緒っだったらもう一度
                            continue;
                        else
                            break;
                    }
                }
            }
            if (number <= 0)//そのWaveに出てくる敵の出現がなくなったら
                flgNumber = false;

            if (enemyNumber[j] <= 0 && j < maxWave - 1)//そのWaveの敵が全て死んだら
            {
                waveNumber++;
                time = 0;
                if (j < maxWave - 2)
                {
                    j++;
                    bandFade.FlgFadeIn = true;
                }
                else
                    flgBoss = true;
                number = enemyNumber[j];
                flgNumber = true;
            }
        }

        //if (Input.GetKeyDown(KeyCode.A))
        //    yMusicManager.instance.MusicSound((int)yMusicManager.musicChip.bgm1);
        //if (Input.GetKeyDown(KeyCode.B))
        //    Sound((int)soundClip.se1);
        //if (Input.GetKeyDown(KeyCode.C))
        //    Sound((int)soundClip.se2);
        //if (Input.GetKeyDown(KeyCode.D))
        //    Sound((int)soundClip.se3);
        //if (Input.GetKeyDown(KeyCode.Space))
        //    MusicEffectStop();


    }

    private void EnemyNumber(int x)//Wave毎ごとの敵の数
    {
        enemyNumber = new int[maxWave];

        for (int y = 1;y < csv.wave.Count; y++)
        {
            if (int.Parse(csv.wave[y][(int)topRow.Stage]) == stageNumber)//ステージの確認
            {
                enemyNumber[int.Parse(csv.wave[y][x]) - 1] += 1;
            }
        }
    }

    private void EnemyTime(int x)//Wave毎ごとの敵の出てくる時間
    {
        int i = 0;
        for (int k = 0; k < maxWave; k++)
            number += enemyNumber[k];
        enemyAppearanceTime = new float[number];

        for (int y = 1; y < csv.wave.Count; y++)
        {
            if (int.Parse(csv.wave[y][(int)topRow.Stage]) == stageNumber)//ステージの確認
            {
                enemyAppearanceTime[i] = float.Parse(csv.wave[y][(int)topRow.Time]);
                i++;
            }
        }
    }

    private void EnemyPos(int x)//敵の位置の初期化
    {
        int i = 0;
        for (int k = 0; k < maxWave; k++)
            number += enemyNumber[k];
        enemyPos = new Vector3[number];

        for (int y = 1; y < csv.wave.Count; y++)
        {
            if (int.Parse(csv.wave[y][(int)topRow.Stage]) == stageNumber)//ステージの確認
            {
                Vector3 cameraPos = Camera.main.transform.position;
                float rangeX = Random.Range(-2, 3);
                float rangeY = Random.Range(-5, 5);
                switch (csv.wave[y][x])
                {
                    case "左":
                        enemyPos[i] = new Vector3(cameraPos.x - 5.0f, cameraPos.y,0);
                        i++;
                        break;
                    case "右":
                        enemyPos[i] = new Vector3(cameraPos.x + 5.0f, cameraPos.y, 0);
                        i++;
                        break;
                    default:
                        string[] pos = csv.wave[y][x].Split('/');
                        enemyPos[i] = new Vector3(float.Parse(pos[0]), float.Parse(pos[1]), 0);
                        i++;
                        break;
                }
            }
        }
    }

    private void EnemyID(int x)//敵のIDの初期化
    {
        int i = 0;
        for (int k = 0; k < maxWave; k++)
            number += enemyNumber[k];

        enemyID = new string[number];

        for (int y = 1; y < csv.wave.Count; y++)
        {
            if (int.Parse(csv.wave[y][(int)topRow.Stage]) == stageNumber)//ステージの確認
            {
                enemyID[i] = csv.wave[y][x];
                i++;
            }
        }
    }

    private void EnemyHP(int x)//敵のHP
    {
        int i = 0;
        for (int k = 0; k < maxWave; k++)
            number += enemyNumber[k];
        enemyHP = new int[number];

        for (int y = 1; y < csv.wave.Count; y++)
        {
            if (int.Parse(csv.wave[y][(int)topRow.Stage]) == stageNumber)//ステージの確認
            {
                enemyHP[i] = int.Parse(csv.wave[y][x]);
                i++;
            }
        }

    }

    private void MaxWave(int x)
    {
        for (int y = 1; y < csv.wave.Count; y++)
        {
            if (int.Parse(csv.wave[y][(int)topRow.Stage]) == stageNumber)//ステージの確認
            {
                if(maxWave < int.Parse(csv.wave[y][x]))
                {
                    maxWave = int.Parse(csv.wave[y][x]);
                }
            }
        }
    }

    IEnumerator BossAppearance()
    {
        yield return new WaitUntil(() => flgBoss);
        yield return new WaitForSeconds(1.0f);

        yield return StartCoroutine("BossPerformance");//ボスが出てくる演出

        yield return new WaitForSeconds(enemyAppearanceTime[i]);//ボス出現
        SpriteRenderer boss;
        for (int k = 0; k < enemyType.Length; k++)
        {
            if (enemyID[i] == enemyType[k].name)
            {
                boss = Instantiate(enemyType[k], enemyPos[i], Quaternion.identity) as SpriteRenderer;
                boss.name = enemyID[i] + number;
                enemyManager = boss.GetComponent<yEnemyManager>();
                enemyManager.EnemyHP = enemyHP[i];

                break;
            }
        }

        yield return StartCoroutine("BossPerformanceText");//ボス出現のテキスト
        yield break;
    }

    IEnumerator BossPerformance()
    {
        Image bossPerformance = GameObject.Find("Performance/Image").GetComponent<Image>();
        int i = 0;
        while (i < 3)
        {
            float alpha = 146.0f / 255.0f;
            bossPerformance.color = new Color(1, 0, 0, alpha);
            yield return new WaitForSeconds(0.3f);
            while (alpha >= 0)
            {
                alpha -= 0.1f;
                bossPerformance.color = new Color(1, 0, 0, alpha);
                yield return new WaitForSeconds(0.08f);
            }
            yield return new WaitForSeconds(0.5f);
            bossPerformance.color = new Color(1, 0, 0, 0);
            yield return new WaitForSeconds(0.03f);
            i++;
        }
        yield break;
    }

    IEnumerator BossPerformanceText()
    {
        Text[] text = new Text[6];
        for (int i = 0; i < text.Length; i++)
        {
            int j = i + 1;
            text[i] = GameObject.Find("Performance/Text" + j).GetComponent<Text>();
            text[i].color = new Color(0, 0, 0, 1);
        }
        yield return new WaitForSeconds(1.0f);

        for (int i = 0; i < text.Length; i++)
            text[i].color = new Color(0, 0, 0, 0);
        yield break;
    }
}
