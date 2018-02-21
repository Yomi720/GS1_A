using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class yBandFade : MonoBehaviour {

    Image band;
    Text bandText;

    [SerializeField,Header("FadeInとFadeOutのフレームカウント")]
    int frameCount = 5;
    int maxWave = 0;

    [SerializeField,Header("FadeInしたあとのFadeOutする時間")]
    float fadeOutTime = 0.5f;
    float alpha = 0;

    string startText;//最初の文字

    bool flgFade = true;
    bool flgFadeIn = true;
    bool flgFadeOut = false;

    yWaveManagement waveManagement;
    yTime time;

    public bool FlgFadeIn
    {
        set { flgFadeIn = value; }
        get { return flgFadeIn; }
    }

    // Use this for initialization
    void Start () {
        //子オブジェクト取得
        band = transform.FindChild("band").GetComponent<Image>();
        bandText = transform.FindChild("bandText").GetComponent<Text>();

        waveManagement = GameObject.Find("Wave").GetComponent<yWaveManagement>();
        time = GameObject.Find("Time").GetComponent<yTime>();

        band.color = new Color(band.color.r, band.color.g, band.color.b, 0);
        bandText.color = new Color(bandText.color.r, bandText.color.g, bandText.color.b, 0);

        maxWave = waveManagement.WaveMax;
        startText = "START Wave " + "1 / " + maxWave;
        bandText.text = startText;
    }

    // Update is called once per frame
    void Update () {
        if (flgFadeIn)
        {
            if (Time.frameCount % frameCount == 0)
                BandFadeIn();
        }

        if (flgFadeOut)
        {
            if (Time.frameCount % frameCount == 0)
                BandFadeOut();
        }

        if (flgFade && alpha >= 1.0f)
            Invoke("Fade", fadeOutTime);

    }

    private void BandFadeIn()
    {
        time.FlgTime = false;//時間が止まる
        alpha += 0.1f;
        band.color = new Color(band.color.r, band.color.g, band.color.b, alpha);
        bandText.color = new Color(bandText.color.r, bandText.color.g, bandText.color.b, alpha);
        if (alpha >= 1.0f)
        {
            if (bandText.text != startText)//初回以外テキスト更新
            {
                bandText.text = "Wave " + waveManagement.WaveNumber + " / " + maxWave;
                flgFade = true;
            }
        }
    }

    private void BandFadeOut()
    {
        alpha -= 0.1f;
        band.color = new Color(band.color.r, band.color.g, band.color.b, alpha);
        bandText.color = new Color(bandText.color.r, bandText.color.g, bandText.color.b, alpha);
        if (alpha <= 0.0f)
        {
            flgFadeOut = false;
            bandText.text = "Wave " + waveManagement.WaveNumber + " / " + maxWave;
            time.FlgTime = true;//そして動き出す
        }
    }

    private void Fade()
    {
        flgFade = false;
        flgFadeIn = false;
        flgFadeOut = true;
    }
}
