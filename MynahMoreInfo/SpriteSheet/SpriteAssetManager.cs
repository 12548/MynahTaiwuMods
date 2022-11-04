using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore;

namespace MynahMoreInfo.SpriteSheet;

/// <summary>
/// 剑圣大佬提供的TMP_SpriteAsset生成框架
/// </summary>
public static class SpriteAssetManager
{
    public static void Init()
    {
        var keys = new[]
        {
            "sp_icon_renwutexing_0",
            "sp_icon_renwutexing_1",
            "sp_icon_renwutexing_2",
            "sp_icon_renwutexing_3",
            "sp_icon_renwutexing_4",
            "sp_icon_renwutexing_5",
            "sp_icon_renwutexing_6",
            "sp_icon_renwutexing_7",
            "sp_icon_renwutexing_8",
            "sp_icon_renwutexing_9",
            "sp_icon_renwutexing_10",
            "sp_icon_renwutexing_11",
        };

        var sprites = new List<Sprite>();
        foreach (var key in keys)
        {
            AtlasInfo.Instance.GetSprite(key, sprite =>
            {
                sprites.Add(sprite);
                if (sprites.Count == keys.Length)
                {
                    CreateAndRegisterSpriteAsset("mmiSprites", sprites);
                }
            });
        }
    }

    private static TMP_SpriteAsset CreateAndRegisterSpriteAsset(string assetName, List<Sprite> sprites)
    {
        var textPack = new Texture2D[sprites.Count];
        for (var i = 0; i < sprites.Count; i++)
        {
            textPack[i] = DuplicateTexture(sprites[i]);
        }

        var totalTex = new Texture2D(2, 2);
        var rects = totalTex.PackTextures(textPack, 0);
        var spriteAsset = ScriptableObject.CreateInstance<TMP_SpriteAsset>();
        spriteAsset.spriteSheet = totalTex;
        spriteAsset.material = new Material(Shader.Find("TextMeshPro/Sprite"));
        spriteAsset.material.SetTexture(ShaderUtilities.ID_MainTex, spriteAsset.spriteSheet);
        var spriteInfoList = sprites.Select((t, i) => CreateSpriteInfo(i, totalTex, rects[i], t.name)).ToList();

        spriteAsset.spriteInfoList = spriteInfoList;
        UpgradeSpriteAssetEx(spriteAsset);
        spriteAsset.name = assetName;
        spriteAsset.hashCode = TMP_TextUtilities.GetSimpleHashCode(spriteAsset.name);

        var mSpriteAssetReferenceLookup =
            AccessTools.Field(typeof(MaterialReferenceManager), "m_SpriteAssetReferenceLookup")
                .GetValue(MaterialReferenceManager.instance) as Dictionary<int, TMP_SpriteAsset>;
        var mFontMaterialReferenceLookup =
            AccessTools.Field(typeof(MaterialReferenceManager), "m_FontMaterialReferenceLookup")
                .GetValue(MaterialReferenceManager.instance) as Dictionary<int, Material>;
        if (mSpriteAssetReferenceLookup != null) mSpriteAssetReferenceLookup[spriteAsset.hashCode] = spriteAsset;
        if (mFontMaterialReferenceLookup != null)
            mFontMaterialReferenceLookup[spriteAsset.hashCode] = spriteAsset.material;

        return spriteAsset;
    }

    /// <summary>
    /// 创建图片信息
    /// </summary>
    private static TMP_Sprite CreateSpriteInfo(int id, Texture2D totalTex, Rect uvRect, string iconName)
    {
        var info = new TMP_Sprite();
        var rect = TransSpriteRect(totalTex, uvRect);
        info.id = id;
        info.name = iconName;
        info.hashCode = TMP_TextUtilities.GetSimpleHashCode(iconName);
        info.x = rect.x;
        info.y = rect.y;
        info.width = rect.width;
        info.height = rect.height;
        info.xOffset = 0;
        info.yOffset = rect.height;
        info.xAdvance = rect.width;
        info.scale = 1;
        info.pivot = new Vector2(0, 0);
        return info;
    }

    /// <summary>
    /// 转换图片矩阵坐标
    /// </summary>
    /// <param name="totalTex"></param>
    /// <param name="uvRect"></param>
    /// <returns></returns>
    private static Rect TransSpriteRect(Texture2D totalTex, Rect uvRect)
    {
        var rect = new Rect();
        rect.x = uvRect.x * totalTex.width;
        rect.y = uvRect.y * totalTex.height;
        rect.width = uvRect.width * totalTex.width;
        rect.height = uvRect.height * totalTex.height;
        return rect;
    }

    /// <summary>
    /// 导出材质
    /// </summary>
    /// <param name="sprite"></param>
    /// <returns></returns>
    private static Texture2D DuplicateTexture(Sprite sprite)
    {
        var source = sprite.texture;
        var renderTex = RenderTexture.GetTemporary(
            source.width,
            source.height,
            0,
            RenderTextureFormat.Default,
            RenderTextureReadWrite.Default);
        Graphics.Blit(source, renderTex);
        var previous = RenderTexture.active;
        RenderTexture.active = renderTex;
        var readableText = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);
        var rect = sprite.textureRect;
        rect.y = source.height - rect.y - rect.height;
        readableText.ReadPixels(rect, 0, 0, false);
        readableText.Apply();
        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(renderTex);
        return readableText;
    }

    private static void UpgradeSpriteAssetEx(TMP_SpriteAsset spriteAsset)
    {
        Traverse.Create(spriteAsset).Field<string>("m_Version").Value = "1.1.0";
        var m_SpriteCharacterTable = Traverse.Create(spriteAsset)
            .Field<List<TMP_SpriteCharacter>>("m_SpriteCharacterTable").Value;
        var m_SpriteGlyphTable = Traverse.Create(spriteAsset)
            .Field<List<TMP_SpriteGlyph>>("m_SpriteGlyphTable").Value;
        var spriteInfoList = spriteAsset.spriteInfoList;
        m_SpriteCharacterTable.Clear();
        m_SpriteGlyphTable.Clear();
        for (var i = 0; i < spriteInfoList.Count; i++)
        {
            var oldSprite = spriteInfoList[i];
            var spriteGlyph = new TMP_SpriteGlyph
            {
                index = (uint)i,
                sprite = oldSprite.sprite,
                metrics = new GlyphMetrics(oldSprite.width, oldSprite.height, oldSprite.xOffset,
                    oldSprite.yOffset, oldSprite.xAdvance),
                glyphRect = new GlyphRect((int)oldSprite.x, (int)oldSprite.y, (int)oldSprite.width,
                    (int)oldSprite.height),
                scale = 1f,
                atlasIndex = 0
            };
            m_SpriteGlyphTable.Add(spriteGlyph);
            
            var spriteName = oldSprite.name;
            if (spriteName.EndsWith("(Clone)"))
            {
                spriteName = spriteName.Substring(0, spriteName.Length - 7);
            }
            
            var spriteCharacter = new TMP_SpriteCharacter
            {
                glyph = spriteGlyph,
                glyphIndex = (uint)i,
                unicode = (uint)((oldSprite.unicode == 0) ? 65534 : oldSprite.unicode),
                name = spriteName,
                scale = oldSprite.scale
            };
            m_SpriteCharacterTable.Add(spriteCharacter);
        }

        spriteAsset.UpdateLookupTables();
    }
}