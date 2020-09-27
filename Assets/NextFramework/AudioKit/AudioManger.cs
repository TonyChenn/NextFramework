using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManger
{
    AudioSource AS_UI;
    AudioSource AS_BGM;
    AudioSource AS_Effect;

    GameObject bgmObject;
    GameObject uiObject;
    GameObject effectObject;

    const float defVolume = 0.8f;

    static List<AudioSource> uiASList = new List<AudioSource>();
    static List<AudioSource> effectASList = new List<AudioSource>();
    static Dictionary<string, AudioClip> audioClipDict = new Dictionary<string, AudioClip>();

    static AudioManger _instance = null;
    public static AudioManger Singlton
    {
        get
        {
            if (_instance == null)
                _instance = new AudioManger();
            return _instance;
        }
    }

    private AudioManger()
    {
        Transform audioTrans = GameObject.Find("AudioObject").transform;
        //BGM
        bgmObject = new GameObject();
        bgmObject.transform.SetParent(audioTrans);
        bgmObject.name = "BgmSound";
        AS_BGM = bgmObject.AddComponent<AudioSource>();
        AS_BGM.loop = false;
        AS_BGM.playOnAwake = false;
        AS_BGM.volume = GetAudioVolumeData(AudioType.BGM);
        //UI
        uiObject = new GameObject();
        uiObject.transform.SetParent(audioTrans);
        uiObject.name = "UISound";
        AS_UI = uiObject.AddComponent<AudioSource>();
        AS_UI.loop = false;
        AS_UI.playOnAwake = false;
        AS_UI.volume = GetAudioVolumeData(AudioType.UI);
        uiASList.Add(AS_UI);
        //Effect
        effectObject = new GameObject();
        effectObject.transform.SetParent(audioTrans);
        effectObject.name = "UIEffect";
        AS_Effect = effectObject.AddComponent<AudioSource>();
        AS_Effect.loop = false;
        AS_Effect.playOnAwake = false;
        AS_Effect.volume = GetAudioVolumeData(AudioType.Effect);
        effectASList.Add(AS_Effect);
    }

    AudioSource CreateNewUISource()
    {
        AudioSource _as = uiObject.AddComponent<AudioSource>();
        _as.loop = false;
        _as.playOnAwake = false;
        _as.volume = AS_UI.volume;
        uiASList.Add(_as);

        return _as;
    }
    AudioSource CreateNewEffectSource()
    {
        AudioSource _as = effectObject.AddComponent<AudioSource>();
        _as.loop = false;
        _as.playOnAwake = false;
        _as.volume = AS_Effect.volume;
        uiASList.Add(_as);

        return _as;
    }


    #region 保存本地音量，读取本地音量
    public float GetAudioVolumeData(AudioType type)
    {
        if(type==AudioType.BGM)
        {
            if (PlayerPrefs.HasKey("bgmVolume"))
                return PlayerPrefs.GetFloat("bgmVolume");
            else
                return defVolume;
        }
        else if (type == AudioType.UI)
        {
            if (PlayerPrefs.HasKey("uiVolume"))
                return PlayerPrefs.GetFloat("uiVolume");
            else
                return defVolume;
        }
        else if(type==AudioType.Effect)
        {
            if (PlayerPrefs.HasKey("effectVolume"))
                return PlayerPrefs.GetFloat("effectVolume");
            else
                return defVolume;
        }
        return 1f;
    }
    public void SetAudioVolumeData(AudioType type,float volume)
    {
        if (type == AudioType.BGM)
            PlayerPrefs.SetFloat("bgmVolume", volume);
        else if (type == AudioType.UI)
            PlayerPrefs.SetFloat("uiVolume", volume);
        else if(type==AudioType.Effect)
            PlayerPrefs.SetFloat("effectVolume", volume);

        SetAudioSourceVolume(type, volume);
    }
    #endregion

    void SetAudioSourceVolume(AudioType type,float volume)
    {
        if(type==AudioType.BGM)
        {
            AS_BGM.volume = Mathf.Clamp01(volume);
        }
        else if(type == AudioType.UI)
        {
            for (int i = 0; i < uiASList.Count; i++)
                uiASList[i].volume = Mathf.Clamp01(volume);
        }
        else if (type == AudioType.Effect)
        {
            for (int i = 0; i < effectASList.Count; i++)
                effectASList[i].volume = Mathf.Clamp01(volume);
        }
    }

    double trackBGMStartTime = 0f;
    public void PlayAudioClip(AudioType type, AudioClip clip)
    {
        AudioSource source = null;
        if(type==AudioType.BGM)
        {
            trackBGMStartTime = AudioSettings.dspTime + 1;
            source = AS_BGM;
        }
        else if(type==AudioType.UI)
        {
            for (int i = 0; i < uiASList.Count; i++)
            {
                if (!uiASList[i].isPlaying)
                {
                    source = uiASList[i];
                    break;
                }
            }
            if (source == null)
                source = CreateNewUISource();
        }
        else if(type==AudioType.Effect)
        {
            for (int i = 0; i < effectASList.Count; i++)
            {
                if (!effectASList[i].isPlaying)
                {
                    source = effectASList[i];
                    break;
                }
            }
            if (source == null)
                source = CreateNewEffectSource();
        }
        if(source!=null)
        {
            source.Stop();
            source.time = 0;
            source.clip = clip;
            if (type == AudioType.BGM)
                source.PlayScheduled(trackBGMStartTime);
            else
                source.Play();
        }
    }
    public void OnPlayUIButtonSound(UI_SoundType type)
    {
        AudioClip clip = null;
        if(type==UI_SoundType.common)
        {
            if(audioClipDict.ContainsKey("CommonClickSound"))
            {
                clip = audioClipDict["CommonClickSound"];
            }
            else
            {
                clip = Resources.Load("Audio/UI/Button") as AudioClip;
            }
        }
        if (clip != null)
            Singlton.PlayAudioClip(AudioType.UI, clip);
    }

    public void StopAudio(AudioType type)
    {
        if(type==AudioType.BGM)
        {
            AS_BGM.time = 0;
            AS_BGM.Stop();
            AS_BGM.Pause();
        }
        else if(type==AudioType.UI)
        {
            for (int i = 0; i < uiASList.Count; i++)
            {
                uiASList[i].time = 0;
                uiASList[i].Stop();
            }
        }
        else if(type==AudioType.Effect)
        {
            for (int i = 0; i < effectASList.Count; i++)
            {
                effectASList[i].time = 0;
                effectASList[i].Stop();
            }
        }
    }

    public void StopAllAudio()
    {
        StopAudio(AudioType.BGM);
        StopAudio(AudioType.UI);
        StopAudio(AudioType.Effect);
    }
    public void PauseBGM()
    {
        if (AS_BGM.isPlaying)
            AS_BGM.Pause();
    }
    public void ReplayBGM()
    {
        if (!AS_BGM.isPlaying)
            AS_BGM.Play();
    }

    /// <summary>
    /// 获取背景音乐时长
    /// </summary>
    public float GetBGMTime()
    {
        if (AS_BGM.isPlaying)
            return (float)(AudioSettings.dspTime - trackBGMStartTime);
        else
            return AS_BGM.time;
    }
    /// <summary>
    /// 设置当前播放位置
    /// </summary>
    public void SetBGMTime(float time)
    {
        AS_BGM.timeSamples = (int)(AS_BGM.clip.frequency * time);
        trackBGMStartTime = AudioSettings.dspTime - time;
    }

}
public enum AudioType
{
    BGM,
    UI,
    Effect,
}
public enum UI_SoundType
{
    common,
}
