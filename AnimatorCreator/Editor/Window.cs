using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

namespace BuildAnimation
{
	public class Window : EditorWindow
	{

		private static readonly float WIDTH = 500f;
		private static readonly float HEIGHT = 400f;

		private string PlistAndPngPath;

		private PageType selectPageIndex = PageType.ParseCocosPlist;

		private enum PageType
        {
			BuildAnimation = 0,
			ParseCocosPlist = 1,
        }

		static void Init(PageType pageType)
        {
			Window window = GetWindow<Window>(true, "AnimationCreator");
			window.minSize = new Vector2(WIDTH, HEIGHT);
			window.selectPageIndex = pageType;
			window.Show();
        }
		
		[MenuItem("Assets/BuildAnimation/ParseCocosToAnimation")]
		static void InitCocosToAnimation()
        {
			Init(PageType.ParseCocosPlist);
        }

        private void Awake()
        {
			PlistAndPngPath = "";
        }

        private void OnGUI()
        {
			selectPageIndex = (PageType)GUILayout.Toolbar((int)selectPageIndex, new string[] { "解析Cocos2dx大图并生成Animator" });
            switch (selectPageIndex)
            {
				case PageType.ParseCocosPlist:
					DrawParseCocosPlist();
					break;
				default:
					break;
            }
        }

		private void DrawParseCocosPlist()
        {
			GUILayout.Label("要解析的Cocos2dx资源根路径");
			PlistAndPngPath = TextField(PlistAndPngPath);

            string[] guids = Selection.assetGUIDs;
            if (guids.Length > 0)
            {
                PlistAndPngPath = AssetDatabase.GUIDToAssetPath(guids[0]);
            }

            GUILayout.Label("开始生成");
            if (GUILayout.Button("BuildAnimation", GUILayout.Width(100)))
            {
                if (PlistAndPngPath == "")
                {
                    EditorUtility.DisplayDialog("错误信息", "路径不能为空！", "确认");
                    return;
                }

                BuildAnimation.SetPlistPath(PlistAndPngPath);
                BuildAnimation.SetAnimationPath(PlistAndPngPath + "/Animation");
                BuildAnimation.SetAnimatorPath(PlistAndPngPath + "/AnimationController");
                BuildAnimation.ParseCocosPlist();
            }
        }

        /// <summary>
        /// TextField复制粘贴的实现
        /// </summary>
        public static string TextField(string value, params GUILayoutOption[] options)
        {
            int textFieldID = GUIUtility.GetControlID("TextField".GetHashCode(), FocusType.Keyboard) + 1;
            if (textFieldID == 0)
                return value;
            //处理复制粘贴的操作
            value = HandleCopyPaste(textFieldID) ?? value;
            return GUILayout.TextField(value, options);
        }

        public static string HandleCopyPaste(int controlID)
        {
            if (controlID == GUIUtility.keyboardControl)
            {
                if (Event.current.type == UnityEngine.EventType.KeyUp && (Event.current.modifiers == EventModifiers.Control || Event.current.modifiers == EventModifiers.Command))
                {
                    if (Event.current.keyCode == KeyCode.C)
                    {
                        Event.current.Use();
                        TextEditor editor = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl);
                        editor.Copy();
                    }
                    else if (Event.current.keyCode == KeyCode.V)
                    {
                        Event.current.Use();
                        TextEditor editor = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl);
                        editor.Paste();
#if UNITY_5_3_OR_NEWER || UNITY_5_3
                        return editor.text; //以及更高的unity版本中editor.content.text已经被废弃，需使用editor.text代替
#else
                    return editor.content.text;
#endif
                    }
                    else if (Event.current.keyCode == KeyCode.A)
                    {
                        Event.current.Use();
                        TextEditor editor = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl);
                        editor.SelectAll();
                    }
                }
            }
            return null;
        }
    }

}

