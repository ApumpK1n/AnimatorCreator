using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


namespace BuildAnimation.Parser
{
    public class SpriteFrame
    {
        public static SpriteMetaData CreateWithFrameDict(FrameDataDict frameDataDict, Texture2D bigTexture)
        {
            SpriteMetaData spriteMetaData = new SpriteMetaData();
            int sampleWidth = frameDataDict.width;
            int sampleHeight = frameDataDict.height;

            //旋转时，宽高调换
            if (frameDataDict.rotated)
            {
                sampleWidth = frameDataDict.height;
                sampleHeight = frameDataDict.width;
            }

            // 起始位置
            int startPosX = frameDataDict.x;
            int startPosY = bigTexture.height - (frameDataDict.y + sampleHeight);

            spriteMetaData.rect = new Rect(startPosX, startPosY, sampleWidth, sampleHeight);
            spriteMetaData.pivot = new Vector2(0.5f, 0.5f);
            spriteMetaData.name = frameDataDict.name;

            return spriteMetaData;
        }
    }

}
