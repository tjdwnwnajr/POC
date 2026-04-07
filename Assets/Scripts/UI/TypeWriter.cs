using System.Collections;
using UnityEngine;
using TMPro;

public class TypeWriterFade : MonoBehaviour
{
    public TextMeshProUGUI textUI;

    public float fadeInDuration = 0.3f;
    public float delayBetweenChars = 0.05f;
    public float stayDuration = 1.5f;
    public float fadeOutDuration = 0.5f;

    [TextArea]
    public string[] lines;

    private int currentLine = 0;

    void Start()
    {
        //StartCoroutine(PlayDialogue());
    }
    public void StartDialogue()
    {
        StartCoroutine(PlayDialogue());
    }

    IEnumerator PlayDialogue()
    {
        while (currentLine < lines.Length)
        {
            yield return StartCoroutine(FadeText(lines[currentLine]));
            yield return new WaitForSeconds(stayDuration);
            yield return StartCoroutine(FadeOutText());

            currentLine++;
        }

        textUI.text = "";
    }

    //  글자 하나씩 페이드 인
    IEnumerator FadeText(string line)
    {
        textUI.text = line;
        textUI.ForceMeshUpdate();

        TMP_TextInfo textInfo = textUI.textInfo;

        // 전체 알파 0으로 초기화
        for (int i = 0; i < textInfo.characterCount; i++)
        {
            if (!textInfo.characterInfo[i].isVisible) continue;

            int matIndex = textInfo.characterInfo[i].materialReferenceIndex;
            int vertIndex = textInfo.characterInfo[i].vertexIndex;

            Color32[] colors = textInfo.meshInfo[matIndex].colors32;

            for (int j = 0; j < 4; j++)
                colors[vertIndex + j].a = 0;
        }

        textUI.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);

        // 한 글자씩 등장
        for (int i = 0; i < textInfo.characterCount; i++)
        {
            if (!textInfo.characterInfo[i].isVisible) continue;

            StartCoroutine(FadeCharacter(i));
            yield return new WaitForSeconds(delayBetweenChars);
        }
    }

    IEnumerator FadeCharacter(int index)
    {
        TMP_TextInfo textInfo = textUI.textInfo;

        int matIndex = textInfo.characterInfo[index].materialReferenceIndex;
        int vertIndex = textInfo.characterInfo[index].vertexIndex;

        Color32[] colors = textInfo.meshInfo[matIndex].colors32;

        float time = 0f;

        while (time < fadeInDuration)
        {
            byte alpha = (byte)Mathf.Lerp(0, 255, time / fadeInDuration);

            for (int j = 0; j < 4; j++)
                colors[vertIndex + j].a = alpha;

            textUI.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);

            time += Time.deltaTime;
            yield return null;
        }

        for (int j = 0; j < 4; j++)
            colors[vertIndex + j].a = 255;

        textUI.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
    }

    //  전체 텍스트 페이드 아웃
    IEnumerator FadeOutText()
    {
        TMP_TextInfo textInfo = textUI.textInfo;

        float time = 0f;

        while (time < fadeOutDuration)
        {
            byte alpha = (byte)Mathf.Lerp(255, 0, time / fadeOutDuration);

            for (int i = 0; i < textInfo.characterCount; i++)
            {
                if (!textInfo.characterInfo[i].isVisible) continue;

                int matIndex = textInfo.characterInfo[i].materialReferenceIndex;
                int vertIndex = textInfo.characterInfo[i].vertexIndex;

                Color32[] colors = textInfo.meshInfo[matIndex].colors32;

                for (int j = 0; j < 4; j++)
                    colors[vertIndex + j].a = alpha;
            }

            textUI.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);

            time += Time.deltaTime;
            yield return null;
        }
    }
}