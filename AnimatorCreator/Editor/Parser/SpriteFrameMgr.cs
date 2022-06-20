using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using System.IO;

namespace BuildAnimation.Parser
{
    public class SpriteFrameMgr
    {
        public static Dictionary<string, FrameDataDict> frameInfos = new Dictionary<string, FrameDataDict>();

        public static string AddSpriteFrameWithPlist(string plist)
        {
            PlistDictionary dPlist = new PlistDictionary();
            dPlist.LoadWithFile(plist);
            return LoadWithFrameDict(dPlist);
        }

        private static PlistMetaData ParseMetaData(PlistDictionary dMetaData)
        {
            PlistMetaData metaData = new PlistMetaData();
            metaData.format = (int)dMetaData["format"];
            metaData.realTextureFileName = dMetaData["realTextureFileName"] as string;
            metaData.size = PlistDictionary.ParseVector2(dMetaData["size"] as string);
            metaData.smartpdate = dMetaData["smartupdate"] as string;
            metaData.textureFileName = dMetaData["textureFileName"] as string;
            return metaData;
        }

        private static List<FrameDataDict> ParseFrames(PlistDictionary dFrames, int format)
        {
            List<FrameDataDict> frames = new List<FrameDataDict>();
            foreach(KeyValuePair<string, object> kv in dFrames)
            {
                if (kv.Value is PlistDictionary)
                {
                    FrameDataDict frameDataDict = new FrameDataDict();
                    frameDataDict.name = kv.Key;
                    PlistDictionary frameDict = kv.Value as PlistDictionary;
                    if (format == 2)
                    {
                        RectInt frame = PlistDictionary.ParseRectInt(frameDict["frame"] as string);
                        frameDataDict.x = frame.x;
                        frameDataDict.y = frame.y;
                        frameDataDict.width = frame.width;
                        frameDataDict.height = frame.height;
                    }
                    else
                    {
                        RectInt frame = PlistDictionary.ParseRectInt(frameDict["textureRect"] as string);
                        frameDataDict.x = frame.x;
                        frameDataDict.y = frame.y;
                        frameDataDict.width = frame.width;
                        frameDataDict.height = frame.height;
                    }

                    if (format == 2)
                    {
                        Vector2 offset = PlistDictionary.ParseVector2(frameDict["offset"] as string);
                        frameDataDict.offsetHeight = (int)offset.y;
                        frameDataDict.offsetWidth = (int)offset.x;
                    }
                    else
                    {
                        Vector2 offset = PlistDictionary.ParseVector2(frameDict["spriteOffset"] as string);
                        frameDataDict.offsetHeight = (int)offset.y;
                        frameDataDict.offsetWidth = (int)offset.x;
                    }
                    if (format == 2)
                    {
                        frameDataDict.rotated = (bool)frameDict["rotated"];
                    }
                    else
                    {
                        frameDataDict.rotated = (bool)frameDict["textureRotated"];
                    }
                    if (format == 2)
                    {
                        Vector2 size = PlistDictionary.ParseVector2(frameDict["sourceSize"] as string);
                        frameDataDict.sourceSizeWidth = (int)size.x;
                        frameDataDict.sourceSizeHeight = (int)size.y;
                    }
                    else
                    {
                        Vector2 size = PlistDictionary.ParseVector2(frameDict["spriteSourceSize"] as string);
                        frameDataDict.sourceSizeWidth = (int)size.x;
                        frameDataDict.sourceSizeHeight = (int)size.y;
                    }

                    frames.Add(frameDataDict);
                }
            }
            return frames;
        }

        private static string LoadWithFrameDict(PlistDictionary dPlist)
        {
            //parse metaData
            var meta = dPlist["metadata"] as PlistDictionary;
            PlistMetaData metaData = ParseMetaData(meta);

            //parse frames
            List<FrameDataDict> frames = ParseFrames(dPlist["frames"] as PlistDictionary, metaData.format);

            string name = metaData.realTextureFileName.Substring(0, metaData.realTextureFileName.Length - 4);
            DirectoryInfo directory = new DirectoryInfo(BuildAnimation.PlistPath + "/png");

            Debug.Log("name:" + name);
            string path = BuildAnimation.DataPathToAssetPath(directory.FullName + "/" + name + ".png");

            //load png
            Texture2D bigTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            if (bigTexture == null)
            {
                Debug.LogError("LoadTexture2D, failed!" + path);
                return null;
            }

            List<SpriteMetaData> lstSprite = new List<SpriteMetaData>();
            TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
            foreach(FrameDataDict frameDataDict in frames)
            {
                frameInfos[frameDataDict.name] = frameDataDict;
                SpriteMetaData spriteMetaData = SpriteFrame.CreateWithFrameDict(frameDataDict, bigTexture);
                lstSprite.Add(spriteMetaData);
            }

            textureImporter.textureType = TextureImporterType.Sprite;
            textureImporter.spriteImportMode = SpriteImportMode.Multiple;
            textureImporter.spritesheet = lstSprite.ToArray();

            AssetDatabase.ImportAsset(path);
            return path;
        }
    }
}
