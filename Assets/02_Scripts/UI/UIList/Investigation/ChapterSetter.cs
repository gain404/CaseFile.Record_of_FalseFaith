using System;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChapterSetter : MonoBehaviour
{
    public int SetChapter()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        string digitsOnly = new string(sceneName.Where(char.IsDigit).ToArray());

        if (string.IsNullOrEmpty(digitsOnly))
        {
            return 1;
        }
        else if (int.TryParse(digitsOnly, out int result))
        {
            return result;
        }
        else
        {
            return 1;
        }
    }
}
