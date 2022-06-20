using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using System.IO;
using System.Xml;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor.Animations;

namespace BuildAnimation
{
    public class BuildAnimation : Editor
    {
        //生成的AnimationController路径
        private static string AnimationControllerPath = "";
        //生成的Animation路径
        private static string AnimationPath = "";
        //要解析的图片和plist路径
        public static string PlistPath = "";

        public static void SetAnimatorPath(string path)
        {
            AnimationControllerPath = DataPathToAssetPath(path);
        }

        public static void SetAnimationPath(string path)
        {
            AnimationPath = DataPathToAssetPath(path);
        }

        public static void SetPlistPath(string path)
        {
            PlistPath = DataPathToAssetPath(path);
        }

        public static string DataPathToAssetPath(string path)
        {
            path = path.Replace("\\", "/");
            return path.Substring(path.IndexOf("Assets/"));
        }

        public static void BuildAnimationWithSprite(Dictionary<string, Dictionary<string, Sprite>> dSprite, Dictionary<string, AnimationInfo> animationInfos)
        {
            List<AnimationClip> clips = new List<AnimationClip>();
            //每个名称为一个动画
            foreach(KeyValuePair<string, AnimationInfo> animation in animationInfos)
            {
                string animationName = animation.Key;
                Dictionary<string, Sprite> sprites = dSprite[animationName];
                clips.Add(BuildAnimationClipWithSprite(sprites, animation.Value));
            }
            AnimatorController controller = BuildAnimationController(clips);
            BuildPrefab(controller);
        }

        static AnimationClip BuildAnimationClipWithSprite(Dictionary<string, Sprite> sprites, AnimationInfo animationInfo)
        {
            string animationName = animationInfo.Name;
            AnimationClip clip = new AnimationClip();
            int frameCount = animationInfo.Frames.Count + 1;

            string showImage = "showImage";
            //bindImage
            EditorCurveBinding curveBinding = new EditorCurveBinding();
            curveBinding.type = typeof(Image);
            curveBinding.path = showImage;
            curveBinding.propertyName = "m_Sprite";
            ObjectReferenceKeyframe[] keyFrames = new ObjectReferenceKeyframe[frameCount];

            //bind RectTransform
            EditorCurveBinding curveBindingRectx = new EditorCurveBinding();
            curveBindingRectx.type = typeof(RectTransform);
            curveBindingRectx.path = showImage;
            curveBindingRectx.propertyName = "m_AnchoredPosition.x";
            Keyframe[] keyFramesRectx = new Keyframe[frameCount];
            AnimationCurve animationCurvex = new AnimationCurve();

            EditorCurveBinding curveBindingRecty = new EditorCurveBinding();
            curveBindingRecty.type = typeof(RectTransform);
            curveBindingRecty.path = showImage;
            curveBindingRecty.propertyName = "m_AnchoredPosition.y";
            Keyframe[] keyFramesRecty = new Keyframe[frameCount];
            AnimationCurve animationCurvey = new AnimationCurve();

            //bind Size
            EditorCurveBinding curveBindingRectSizex = new EditorCurveBinding();
            curveBindingRectSizex.type = typeof(RectTransform);
            curveBindingRectSizex.path = showImage;
            curveBindingRectSizex.propertyName = "m_SizeDelta.x";
            Keyframe[] keyFramesRectSizew = new Keyframe[frameCount];
            AnimationCurve animationCurveSizew = new AnimationCurve();

            EditorCurveBinding curveBindingRectSizey = new EditorCurveBinding();
            curveBindingRectSizey.type = typeof(RectTransform);
            curveBindingRectSizey.path = showImage;
            curveBindingRectSizey.propertyName = "m_SizeDelta.y";
            Keyframe[] keyFramesRectSizeh = new Keyframe[frameCount];
            AnimationCurve animationCurveSizeh = new AnimationCurve();

            int frameRate = animationInfo.FrameRate;
            //动画长度是按秒为单位, 时间要根据帧率和帧数来算
            float currentframeTime = 0;

            for(int i=0; i<animationInfo.Frames.Count; i++)
            {
                FrameInfo frameInfo = animationInfo.Frames[i];
                Parser.FrameDataDict frameDataDict = Parser.SpriteFrameMgr.frameInfos[frameInfo.Name];

                if (i == 0)
                {
                    //设第一帧参数
                    ObjectReferenceKeyframe firstObjectReferenceKeyframe = new ObjectReferenceKeyframe();
                    firstObjectReferenceKeyframe.time = 0;
                    firstObjectReferenceKeyframe.value = sprites[frameInfo.Name];
                    keyFrames[i] = firstObjectReferenceKeyframe;

                    Keyframe firstKeyframex = new Keyframe();
                    firstKeyframex.time = 0;
                    firstKeyframex.value = frameDataDict.offsetWidth;
                    keyFramesRectx[i] = firstKeyframex;

                    Keyframe firstKeyframey = new Keyframe();
                    firstKeyframey.time = 0;
                    firstKeyframey.value = frameDataDict.offsetHeight;
                    keyFramesRecty[i] = firstKeyframey;

                    Keyframe firstKeyframew = new Keyframe();
                    firstKeyframew.time = 0;
                    firstKeyframew.value = frameDataDict.width;
                    keyFramesRectSizew[i] = firstKeyframew;

                    Keyframe firstKeyframeh = new Keyframe();
                    firstKeyframeh.time = 0;
                    firstKeyframeh.value = frameDataDict.height;
                    keyFramesRectSizeh[i] = firstKeyframeh;
                }

                ObjectReferenceKeyframe objectReferenceKeyframe = new ObjectReferenceKeyframe();
                int circleCount = frameInfo.CircleCount;
                float time = circleCount / (float)frameRate + currentframeTime;
                objectReferenceKeyframe.time = time;
                objectReferenceKeyframe.value = sprites[frameInfo.Name];
                keyFrames[i + 1] = objectReferenceKeyframe;

                currentframeTime = time;

                Keyframe Keyframex = new Keyframe();
                Keyframex.time = time;
                Keyframex.value = frameDataDict.offsetWidth; // 以锚点构建坐标系
                keyFramesRectx[i + 1] = Keyframex;

                Keyframe Keyframey = new Keyframe();
                Keyframey.time = time;
                Keyframey.value = frameDataDict.offsetHeight;
                keyFramesRecty[i + 1] = Keyframey;

                Keyframe Keyframew = new Keyframe();
                Keyframew.time = time;
                Keyframew.value = frameDataDict.width;
                keyFramesRectSizew[i + 1] = Keyframew;

                Keyframe Keyframeh = new Keyframe();
                Keyframeh.time = time;
                Keyframeh.value = frameDataDict.height;
                keyFramesRectSizeh[i + 1] = Keyframeh;
            }

            animationCurvex.keys = keyFramesRectx;
            animationCurvey.keys = keyFramesRecty;
            animationCurveSizew.keys = keyFramesRectSizew;
            animationCurveSizeh.keys = keyFramesRectSizeh;

            for(int j = 0; j < animationCurvex.keys.Length; j++)
            {
                AnimationUtility.SetKeyRightTangentMode(animationCurvex, j, AnimationUtility.TangentMode.Constant);
            }

            for (int j = 0; j < animationCurvey.keys.Length; j++)
            {
                AnimationUtility.SetKeyRightTangentMode(animationCurvey, j, AnimationUtility.TangentMode.Constant);
            }

            for (int j = 0; j < animationCurveSizew.keys.Length; j++)
            {
                AnimationUtility.SetKeyRightTangentMode(animationCurveSizew, j, AnimationUtility.TangentMode.Constant);
            }

            for (int j = 0; j < animationCurveSizeh.keys.Length; j++)
            {
                AnimationUtility.SetKeyRightTangentMode(animationCurveSizeh, j, AnimationUtility.TangentMode.Constant);
            }

            // 动画帧率
            clip.frameRate = frameRate;
            // 动画自动循环
            AnimationClipSettings clipSettings = AnimationUtility.GetAnimationClipSettings(clip);
            clipSettings.loopTime = true;
            AnimationUtility.SetAnimationClipSettings(clip, clipSettings);

            if (!Directory.Exists(AnimationPath))
            {
                Directory.CreateDirectory(AnimationPath);
            }
            AnimationUtility.SetObjectReferenceCurve(clip, curveBinding, keyFrames);

            AnimationUtility.SetEditorCurve(clip, curveBindingRectx, animationCurvex);
            AnimationUtility.SetEditorCurve(clip, curveBindingRecty, animationCurvey);
            AnimationUtility.SetEditorCurve(clip, curveBindingRectSizex, animationCurveSizew);
            AnimationUtility.SetEditorCurve(clip, curveBindingRectSizey, animationCurveSizeh);

            AssetDatabase.CreateAsset(clip, AnimationPath + "/" + animationName + ".anim");
            AssetDatabase.SaveAssets();
            return clip;
        }

        static AnimatorController BuildAnimationController(List<AnimationClip> clips)
        {
            if (!Directory.Exists(AnimationControllerPath))
            {
                Directory.CreateDirectory(AnimationControllerPath);
            }
            string path = AnimationControllerPath + "/" + "root.controller";
            AnimatorController animatorController = AnimatorController.CreateAnimatorControllerAtPath(path);

            AnimatorControllerLayer layer = animatorController.layers[0];
            AnimatorStateMachine rootStateMachine = layer.stateMachine;
            foreach(AnimationClip newClip in clips)
            {
                AnimatorState state = rootStateMachine.AddState(newClip.name);
                state.motion = newClip;
            }
            AssetDatabase.SaveAssets();
            return animatorController;
        }

        public static string GetAnimationName(string sFrame)
        {
            if (sFrame.IndexOf("_") != -1)
            {
                sFrame = sFrame.Substring(0, sFrame.IndexOf("_"));
            }
            return sFrame;
        }

        public static void ParseCocosPlist()
        {
            Dictionary<string, AnimationInfo> AnimationInfo = new Dictionary<string, AnimationInfo>();
            ParseXML(AnimationInfo);
            DirectoryInfo directory = new DirectoryInfo(PlistPath + "/plist");
            FileInfo[] plists = directory.GetFiles("*.xml");
            Dictionary<string, Dictionary<string, Sprite>> animations = new Dictionary<string, Dictionary<string, Sprite>>();
            foreach(FileInfo plist in plists)
            {
                string path = Parser.SpriteFrameMgr.AddSpriteFrameWithPlist(DataPathToAssetPath(plist.FullName));
                UnityEngine.Object[] sprites = AssetDatabase.LoadAllAssetRepresentationsAtPath(DataPathToAssetPath(path));
                foreach(Sprite sprite in sprites)
                {
                    string animationname = GetAnimationName(sprite.name);
                    if (!animations.ContainsKey(animationname))
                    {
                        Dictionary<string, Sprite> animation = new Dictionary<string, Sprite>();
                        animation[sprite.name] = sprite;
                        animations[animationname] = animation;
                    }
                    else
                    {
                        Dictionary<string, Sprite> animation = animations[animationname];
                        animation[sprite.name] = sprite;
                    }
                }
            }
            BuildAnimationWithSprite(animations, AnimationInfo);
        }

        public static void ParseXML(Dictionary<string, AnimationInfo> animationInfos)
        {
            DirectoryInfo xml = new DirectoryInfo(PlistPath + "/xml");
            //查找所有xml
            FileInfo[] xmls = xml.GetFiles("*.xml");
            for (int i = 0; i < xmls.Length; i++)
            {
                TextAsset textAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(DataPathToAssetPath(xmls[i].FullName));
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(textAsset.text);
                XmlNodeList xmlNodeList = xmlDoc.SelectSingleNode("Animations").ChildNodes;
                foreach(XmlElement node in xmlNodeList)
                {
                    if (node.Name == "Anim")
                    {
                        AnimationInfo animationInfo = new AnimationInfo();
                        string name = node.GetAttribute("name");
                        animationInfo.Name = name;
                        int frameRate = int.Parse(node.GetAttribute("framesPerSecond"));
                        animationInfo.FrameRate = frameRate;
                        animationInfos[name] = animationInfo;

                        List<FrameInfo> Frames = new List<FrameInfo>();
                        foreach(XmlElement frame in node.ChildNodes)
                        {
                            FrameInfo frameInfo = new FrameInfo();
                            string framename = frame.GetAttribute("id");
                            int count = int.Parse(frame.GetAttribute("circlecount"));
                            frameInfo.Name = framename;
                            frameInfo.CircleCount = count;
                            Frames.Add(frameInfo);
                        }
                        animationInfo.Frames = Frames;
                    }
                }
            }
        }

        public static void BuildPrefab(AnimatorController animatorController)
        {
            string prefabPath = PlistPath + "/prefab";
            if (!Directory.Exists(prefabPath))
            {
                Directory.CreateDirectory(prefabPath);
            }

            GameObject go = new GameObject();
            go.name = "Animation";
            Animator animator = go.AddComponent<Animator>();
            animator.runtimeAnimatorController = animatorController;
            GameObject showImage = new GameObject();
            showImage.name = "showImage";
            showImage.AddComponent<Image>();
            showImage.transform.SetParent(go.transform);
            PrefabUtility.CreatePrefab(prefabPath + "/" + go.name + ".prefab", go);
            DestroyImmediate(go);
        }

    }

    public class AnimationInfo
    {
        public string Name { get; set; }
        //帧率
        public int FrameRate { get; set; }
        //帧数
        public List<FrameInfo> Frames { get; set; }
    }

    public class FrameInfo
    {
        //第几帧开始切图
        public int CircleCount { get; set; }
        public string Name { get; set; }
    }
}
