using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class yWaveManagement : MonoBehaviour {
    enum topRow : int { ID = 0, Stage, Wave, Pos, Time ,HP};

    [SerializeField,Header("csvのIDにSprite.name入れなきゃ動かないよ")]
    SpriteRenderer[] enemyType;
    [SerializeField]
    SpriteRenderer bossType;

    Vector3[] enemyPos;

    int i = 0, j = 0;
    int stageNumber = 1;
    int waveNumber = 1;

    [System.NonSerialized]
    public int[] enemyNumber = new int[3];//Wave毎ごとの敵の数、死んだら減っていく
    int[] enemyHP;
    float[] enemyAppearanceTime;//敵が出てくる時間
    int number;//そのWave毎ごとに出現する敵の数、出現したら減っていく
    float time = 0;
    int wholeNumber;

    bool flgNumber = true;
    bool flgBoss = false;


    string[] enemyID;
    yCsvRender csv;
    yEnemyManager enemyManager;

    public int WaveNumber
    {
        set { waveNumber = value; }
        get { return waveNumber; }
    }
    // Use this for initialization
    void Start() {
        csv = GameObject.Find("Reference").GetComponent<yCsvRender>();

        EnemyNumber((int)topRow.Wave);

        EnemyTime((int)topRow.Time);
        EnemyPos((int)topRow.Pos);
        EnemyID((int)topRow.ID);
        EnemyHP((int)topRow.HP);
        number = enemyNumber[0];
        wholeNumber = enemyNumber[0] + enemyNumber[1] + enemyNumber[2];
        print(wholeNumber);
        StartCoroutine("BossAppearance");

    }

    // Update is called once per frame
    void Update()
    {
        time+=Time.deltaTime;
        print(flgNumber);

        if (waveNumber < 3)//ボス戦前のWave数まで
        {
            if (flgNumber && j < 2)
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
                                enemy = Instantiate(enemyType[0], enemyPos[i], Quaternion.identity) as SpriteRenderer;
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

            if (enemyNumber[j] <= 0 && j < 2)//そのWaveの敵が全て死んだら
            {
                waveNumber++;
                time = 0;
                if (j < 1)
                    j++;
                else
                    flgBoss = true;
                number = enemyNumber[j];
                flgNumber = true;
            }
        }

        //if (Input.GetKeyDown(KeyCode.A))
        //    yMusicManager.instance.MusicSound((int)yMusicManager.musicChip.bgm1);

    }

    private void EnemyNumber(int x)//Wave毎ごとの敵の数
    {
        for(int y = 1;y < csv.wave.Count; y++)
        {
            if (int.Parse(csv.wave[y][(int)topRow.Stage]) == stageNumber)//ステージの確認
            {
                switch (int.Parse(csv.wave[y][x]))
                {
                    case 1:
                        enemyNumber[0] += 1;
                        break;
                    case 2:
                        enemyNumber[1] += 1;
                        break;
                    case 3:
                        enemyNumber[2] += 1;
                        break;
                }
            }
        }
    }

    private void EnemyTime(int x)//Wave毎ごとの敵の出てくる時間
    {
        int i = 0;
        int number = enemyNumber[0] + enemyNumber[1] + enemyNumber[2];
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
        int number = enemyNumber[0] + enemyNumber[1] + enemyNumber[2];
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
        int number = enemyNumber[0] + enemyNumber[1] + enemyNumber[2];

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
        int number = enemyNumber[0] + enemyNumber[1] + enemyNumber[2];
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

    IEnumerator BossAppearance()
    {
        yield return new WaitUntil(() => flgBoss);
        yield return new WaitForSeconds(1.0f);

        yield return StartCoroutine("BossPerformance");
        yield return StartCoroutine("BossPerformanceText");
        yield return new WaitForSeconds(1.0f);

        SpriteRenderer boss;
        boss = Instantiate(this.bossType, enemyPos[i], Quaternion.identity) as SpriteRenderer;
        boss.name = enemyID[i] + number;

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
