using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;

namespace Data
{
    public static class SCR
    {
        public const float _basewidth = 1280;
        public const float _baseheight = 720;
        public const int Padding = 30;
    }
    public class Convert
    {
        /// <summary>
        /// 半角の' ','/','0','1','2','3','4','5','6','7','8','9'を全角に変換する
        /// </summary>
        /// <param name="_sauce">変換する半角文字列</param>
        /// <returns>変換後の全角文字列</returns>
        public static string HanToZenConvert(string _sauce)
        {
            string _re = "";
            for (int i = 0; i < _sauce.Length; i++)
            {
                switch (_sauce[i])
                {
                    case ' ':
                        _re += "　";
                        break;
                    case '/':
                        _re += "／";
                        break;
                    case '0':
                        _re += "０";
                        break;
                    case '1':
                        _re += "１";
                        break;
                    case '2':
                        _re += "２";
                        break;
                    case '3':
                        _re += "３";
                        break;
                    case '4':
                        _re += "４";
                        break;
                    case '5':
                        _re += "５";
                        break;
                    case '6':
                        _re += "６";
                        break;
                    case '7':
                        _re += "７";
                        break;
                    case '8':
                        _re += "８";
                        break;
                    case '9':
                        _re += "９";
                        break;
                    default:
                        break;
                }
            }
            return _re;
        }



        /// <summary>
        /// UI座標（RectTransform）を画面サイズに合わせて拡縮する
        /// </summary>
        /// <param name="_rectTransform"></param>
        public static void Correction(RectTransform _rectTransform)
        {
            Vector2 _magnification;
            _magnification.x = Screen.width / SCR._basewidth;
            _magnification.y = Screen.height / SCR._baseheight;
            Vector3 scale;
            scale.x = _rectTransform.localScale.x * _magnification.x;
            scale.y = _rectTransform.localScale.y * _magnification.y;
            scale.z = _rectTransform.localScale.z;
            _rectTransform.localScale = scale;
        }
    }


}
[Serializable]
public class Position
{
    public Scene scene;
    public List<Vector3> pos;
}


[Serializable]
public enum ClearConditions
{
    /// <summary>
    /// 対象の討伐
    /// </summary>
    TargetSubjugation,
    /// <summary>
    /// 採集
    /// </summary>
    Gathering
}
[Serializable]
public enum FailureConditions
{
    OneDown,
    TwoDown,
    ThreeDown,
    FourDown,
    FiveDown
}

public enum ItemStack
{
    Box,
    Poach
}

public enum QuestStatus
{
    quest,
    clear,
    failure
}