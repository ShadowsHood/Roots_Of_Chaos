using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{

    public Transform ZoomPoint;
    public Image fadePanel;
    public float fadeDuration = 3.0f;

    public void Start()
    {
        MusicManager.Instance.PlayMusic("Trailer", 1f);
    }
    public void Play()
    {
        StartCoroutine(TransitionAndLoad());
    }

    IEnumerator TransitionAndLoad()
    {
        SoundManager.Instance.PlaySound3D("Start", transform.position);

        float timer = 0;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(0, 1, timer / fadeDuration);

            if (fadePanel != null)
                fadePanel.color = new Color(0, 0, 0, alpha);

            yield return null;
        }
        // yield return new WaitForSeconds(0.5f);

        SceneManager.LoadScene("Game");
    }

    public void Quit()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}