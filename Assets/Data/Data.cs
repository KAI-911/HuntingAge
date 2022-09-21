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
        /// ”¼Šp‚Ì' ','/','0','1','2','3','4','5','6','7','8','9'‚ğ‘SŠp‚É•ÏŠ·‚·‚é
        /// </summary>
        /// <param name="_sauce">•ÏŠ·‚·‚é”¼Šp•¶š—ñ</param>
        /// <returns>•ÏŠ·Œã‚Ì‘SŠp•¶š—ñ</returns>
        public static string HanToZenConvert(string _sauce)
        {
            string _re = "";
            for (int i = 0; i < _sauce.Length; i++)
            {
                switch (_sauce[i])
                {
                    case ' ':
                        _re += "@";
                        break;
                    case '/':
                        _re += "^";
                        break;
                    case '0':
                        _re += "‚O";
                        break;
                    case '1':
                        _re += "‚P";
                        break;
                    case '2':
                        _re += "‚Q";
                        break;
                    case '3':
                        _re += "‚R";
                        break;
                    case '4':
                        _re += "‚S";
                        break;
                    case '5':
                        _re += "‚T";
                        break;
                    case '6':
                        _re += "‚U";
                        break;
                    case '7':
                        _re += "‚V";
                        break;
                    case '8':
                        _re += "‚W";
                        break;
                    case '9':
                        _re += "‚X";
                        break;
                    default:
                        break;
                }
            }
            return _re;
        }



        /// <summary>
        /// UIÀ•WiRectTransformj‚ğ‰æ–ÊƒTƒCƒY‚É‡‚í‚¹‚ÄŠgk‚·‚é
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
    /// ‘ÎÛ‚Ì“¢”°
    /// </summary>
    TargetSubjugation,
    /// <summary>
    /// ÌW
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