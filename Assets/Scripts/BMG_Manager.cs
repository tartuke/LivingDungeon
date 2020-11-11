using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BMG_Manager : MonoBehaviour
{

    public Transform player;

    public AudioSource BGM;
    public List<AudioClip> bgm_list;
    public Vector2 timeBetweenMusic;

    public AudioSource FX;
    public List<AudioClip> fx_list;
    public AudioClip heartBeat;

    private StatControler sC;
    HealthStat health;


    AudioSource SlimeFX;
    public AudioClip SlimeAudio;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Sound Manager Setup");
        sC = player.GetComponent<StatControler>();
        BGM.volume = 0.05f;
        FX.volume = 0.005f;
        StartCoroutine(BGM_Manager());

        /**/
        GameObject SlimeAS = new GameObject();
        SlimeAS.transform.parent = transform;
        SlimeAS.AddComponent<AudioSource>();
        SlimeFX = SlimeAS.GetComponent<AudioSource>();
        SlimeFX.loop = true;
        SlimeFX.volume = 0.1f;
        SlimeFX.clip = SlimeAudio;
    }

    // Update is called once per frame
    void Update()
    {
        // Make sure the Audio Souce follows the Player
        transform.position = player.position;

        if (health == null)
        {
            health = sC.GetStatOfType(StatType.HealthStat) as HealthStat;
            if(health.PrecentValue <= 0.3)
            {
                FX.clip = heartBeat;
                FX.Play();
                StartCoroutine(StartFade(100, 0.025f, FX));
                FX.loop = true;
            }
            else if(FX.isPlaying)
            {
                StartCoroutine(StartFade(50, 0.0f, FX));
                FX.Stop();
            }
        }
      
        Debug.Log(string.Format("Slime Status: {0}", GameObject.Find("slime_VFP(Clone)")));
        if (GameObject.Find("slime_VFP(Clone)") && !SlimeFX.isPlaying)
        {
            SlimeFX.Play();
            Debug.Log("Slime Playing");
        }
        else if(!GameObject.Find("slime_VFP(Clone)") && SlimeFX.isPlaying)
        {
            SlimeFX.Stop();
            Debug.Log("Slime Stop Playing");
        }
    }

    IEnumerator BGM_Manager()
    {
        while (true)
        {
            Debug.Log("BGM Looper");
            if (BGM.isPlaying)
            {
                yield return new WaitForSeconds(0.5f);
            }

            else
            {
                // If not playing we start the the wait
                Debug.Log(Time.time);
                yield return StartCoroutine(Waiter());
                Debug.Log("Done");
                Debug.Log(Time.time);

                BGM.clip = bgm_list[Random.Range(0, bgm_list.Count)];
                BGM.Play();
                StartCoroutine(StartFade(500, 0.15f, BGM));
            }
            
        }
    }

    IEnumerator Waiter()
    {
        var wait_time = Random.Range(timeBetweenMusic.x, timeBetweenMusic.y);
        yield return new WaitForSeconds(wait_time);
    }
    IEnumerator StartFade(float duration, float targetVolume, AudioSource AS)
    {
        float currentTime = 0;
        float startVol = AS.volume;

        while(currentTime < duration)
        {
            currentTime += Time.deltaTime;
            AS.volume = Mathf.Lerp(startVol, targetVolume, currentTime / duration);
            yield return null;
        }

        yield break;
    }

    public IEnumerator PlayOneShotAudio(int audioIndex)
    {
        GameObject temp = new GameObject();
        AudioSource AS;
        Debug.Log(string.Format("Playing audio: {0}", fx_list[audioIndex]));

        temp.transform.parent = transform;
        temp.AddComponent<AudioSource>();
        AS = temp.GetComponent<AudioSource>();
        AS.clip = fx_list[audioIndex];
        AS.PlayOneShot(AS.clip, 0.2f);
        Debug.Log(string.Format("Audio Still Playing? {0}", AS.isPlaying));

        while (AS.isPlaying)
            yield return null;

        Destroy(temp);
        yield return null;
    }
}
