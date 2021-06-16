using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Common
{
    public class AudioAssetItem : LoadBundle
    {
        public enum AudioType { None, UI, BGM, Effect }

        AudioType m_AudioType = AudioType.None;
        string m_AudioName;
        public AudioClip AudioObject;
        public AudioAssetItem(AudioAssetItem.AudioType audioType, string name)
        {
            m_AudioType = audioType;
            m_AudioName = name;
        }
        public override string FullPath
        {
            get
            {
                switch (m_AudioType)
                {
                    case AudioType.UI:
                    case AudioType.BGM:
                    case AudioType.Effect:
                        return $"{PathConfig.AudioAbsFolder}/{m_AudioType}/{m_AudioName}.u3d";
                    case AudioType.None:
                    default:
                        return $"{PathConfig.AudioAbsFolder}/{m_AudioName}.u3d";
                }
            }
        }

        protected override void Dispose()
        {
            AudioObject = null;
        }

        protected override void OnLoadComplele(AssetRequest req)
        {
            AudioObject = req.Load(m_AudioName, typeof(AudioClip)) as AudioClip;
        }
    }
}

