using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Xml.Linq;
using System;

namespace BuildAnimation.Parser
{
    public class PlistDictionary : Dictionary<string, object>
    {
        private string plist;

        public void LoadWithFile(string plist)
        {
            this.plist = plist;
            TextAsset textAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(plist);
            if (textAsset == null)
            {
                Debug.LogError("Cannot load file:" + plist);
                return;
            }
            string str = textAsset.text;

            //remove header
            int index = str.IndexOf("<!DOCTYPE");
            if (index != -1)
            {
                str = str.Remove(index, str.IndexOf("\n", index) - index);
            }
            LoadWithString(str);
        }

        private void LoadWithString(string str)
        {
            var doc = XDocument.Parse(str);
            var dict = doc.Element("plist").Element("dict");

            IEnumerable<XElement> iter = dict.Elements();
            Parse(iter);
        }


        private void Parse(IEnumerable<XElement> iter)
        {
            for (int i = 0; i < iter.Count(); i += 2)
            {
                XElement key = iter.ElementAt(i);
                XElement value = iter.ElementAt(i + 1);
                var v = ParseValue(value);
                this[key.Value] = v;
            }
        }

        private object ParseValue(XElement ele)
        {
            string name = ele.Name.ToString();
            if (name == "string")
            {
                return ele.Value;
            }
            else if (name == "integer")
            {
                return int.Parse(ele.Value);
            }
            else if (name == "true")
            {
                return true;
            }
            else if (name == "false")
            {
                return false;
            }
            else if (name == "dict")
            {
                PlistDictionary ret = new PlistDictionary();
                ret.Parse(ele.Elements());
                return ret;
            }
            else
            {
                return null;
            }
        }

        public static RectInt ParseRectInt(string s)
        {
            RectInt ret = new RectInt();
            s = s.Replace("{", "");
            s = s.Replace("}", "");

            string[] lstStr = s.Split(',');

            if (lstStr.Length != 4)
            {
                throw new Exception("ParseRectInt, length error");
            }

            ret.x = int.Parse(lstStr[0]);
            ret.y = int.Parse(lstStr[1]);
            ret.width = int.Parse(lstStr[2]);
            ret.height = int.Parse(lstStr[3]);

            return ret;
        }

        public static Vector2 ParseVector2(string s)
        {
            Vector2 vector = new Vector2();
            s = s.Replace("{", "");
            s = s.Replace("}", "");

            string[] lstStr = s.Split(',');

            if (lstStr.Length != 2)
            {
                throw new Exception("ParseVector2, length error");
            }

            vector.x = float.Parse(lstStr[0]);
            vector.y = float.Parse(lstStr[1]);

            return vector;
        }
    }
    /// <summary>
    /// data structure for plist file parse
    /// </summary>
    public sealed class FrameDataDict
    {
        public string name = "";
        // 小图在大图起始位置及小图大小
        public int x;
        public int y;
        public int width;
        public int height;

        //原始小图大小
        public int sourceSizeWidth;
        public int sourceSizeHeight;

        //偏移量
        public int offsetWidth;
        public int offsetHeight;

        //是否旋转
        public bool rotated;
    }

    public sealed class RectInt
    {
        public int x;
        public int y;
        public int width;
        public int height;
    }

    public sealed class PlistMetaData
    {
        public int format;
        public string realTextureFileName;
        public Vector2 size;
        public string smartpdate;
        public string textureFileName;
    }

}

