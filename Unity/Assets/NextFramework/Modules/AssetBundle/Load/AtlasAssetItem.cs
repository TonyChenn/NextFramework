using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

namespace Common
{
    public class AtlasAssetItem : LoadBundle
    {
        string m_AtlasName;
        public SpriteAtlas AtlasObject;

        public AtlasAssetItem(string atlasName)
        {
            this.m_AtlasName = atlasName.TrimEnd('/');
        }

        public override string FullPath
        {
            get
            {
                return $"{PathConfig.UIAtlasAbsPath}/{m_AtlasName}.u3d";
            }
        }

        protected override void Dispose()
        {
            AtlasObject = null;
        }

        protected override void OnLoadComplele(AssetRequest req)
        {
            AtlasObject = req.Load(m_AtlasName,typeof(SpriteAtlas)) as SpriteAtlas;
        }
    }
}

