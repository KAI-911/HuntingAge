using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;

namespace Data
{
    public static class SCR
    {
        public const int Width = 1280;
        public const int Height = 720;
        public const int Padding = 30;
    }
    public class Convert
    {
        public static string HanToZenConvert(string _sauce)
        {
            string _re = "";
            for (int i = 0; i < _sauce.Length; i++)
            {
                switch (_sauce[i])
                {
                    case ' ':
                        _re += "Å@";
                    break;
                    case '/':
                        _re += "Å^";
                        break;
                    case '1':
                        _re += "ÇP";
                        break;
                    case '2':
                        _re += "ÇQ";
                        break;
                    case '3':
                        _re += "ÇR";
                        break;
                    case '4':
                        _re += "ÇS";
                        break;
                    case '5':
                        _re += "ÇT";
                        break;
                    case '6':
                        _re += "ÇU";
                        break;
                    case '7':
                        _re += "ÇV";
                        break;
                    case '8':
                        _re += "ÇW";
                        break;
                    case '9':
                        _re += "ÇX";
                        break;
                    default:
                        break;
                }
            }
            return _re;
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
    /// ëŒè€ÇÃì¢î∞
    /// </summary>
    TargetSubjugation,
    /// <summary>
    /// çÃèW
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