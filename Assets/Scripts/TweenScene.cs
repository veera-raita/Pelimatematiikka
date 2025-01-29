using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static EasingFunction;

public class TweenScene : MonoBehaviour
{
    [SerializeField] EasingFunction.Ease textFadeEase = EasingFunction.Ease.EaseInOutQuad;
    [SerializeField] EasingFunction.Ease circleBounceEase = EasingFunction.Ease.EaseInOutQuad;
    [SerializeField] EasingFunction.Ease buttonRotateEase = EasingFunction.Ease.EaseInOutQuad;
    private const float fadeTime = 3f;
    private const float fastFadeTime = 1f;
    private Coroutine textFadeLooper;
    private Coroutine buttonRotater;
    private Coroutine buttonMover;
    private bool buttonMoving = false;
    [SerializeField] private GameObject titleMenu;
    [SerializeField] private TextMeshProUGUI firstText;
    [SerializeField] private GameObject wowObject;
    [SerializeField] private Button wowButton;
    [SerializeField] private TextMeshProUGUI wowText;
    [SerializeField] private GameObject secondMenu;
    [SerializeField] private TextMeshProUGUI secondText;
    [SerializeField] private Color secondTextColorNormal;
    [SerializeField] private GameObject button1Obj;
    [SerializeField] private TextMeshProUGUI button1Text;
    [SerializeField] private GameObject button2Obj;
    [SerializeField] private TextMeshProUGUI button2Text;
    [SerializeField] private GameObject button3Obj;
    [SerializeField] private TextMeshProUGUI button3Text;

    // Start is called before the first frame update
    void Start()
    {
        secondTextColorNormal = secondText.color;
        secondTextColorNormal.a = 1f;
    }

    public void ToggleMovingButton()
    {
        if (!buttonMoving) buttonMover = StartCoroutine(MoveButton());
    }

    private IEnumerator MoveButton()
    {
        float timer = 0;
        float t = 0;
        bool fadeBack = false;
        EasingFunction.Function func = GetEasingFunction(buttonRotateEase);
        float newY = 0;
        RectTransform buttonRect = button2Obj.GetComponent<RectTransform>();
        Vector2 startPos = buttonRect.position;

        while (true)
        {
            if (timer < fastFadeTime && !fadeBack)
            {
                timer += Time.deltaTime;
            }
            else
            {
                timer -= Time.deltaTime;
            }

            t = timer / fastFadeTime;
            t = func(0f, 1f, t);
            newY = t * 200;
            buttonRect.position = new Vector2(startPos.x, startPos.y + newY);
            if (!fadeBack && timer >= fastFadeTime) fadeBack = true;
            else if (fadeBack && timer <= 0)
            {
                buttonMoving = false;
                break;
            }
            yield return null;
        }
    }

    public void ToggleRotatingButton()
    {
        if (buttonRotater == null) buttonRotater = StartCoroutine(RotateButton());
        else
        {
            StopCoroutine(buttonRotater);
            button3Obj.GetComponent<RectTransform>().eulerAngles = Vector3.zero;
            buttonRotater = null;
        }
    }

    private IEnumerator RotateButton()
    {
        float timer = 0;
        float t = 0;
        EasingFunction.Function func = GetEasingFunction(buttonRotateEase);
        float newRotation = 0;
        RectTransform buttonRect = button3Obj.GetComponent<RectTransform>();

        while (true)
        {
            timer += Time.deltaTime;

            t = timer / fastFadeTime;
            t = func(0f, 1f, t);
            newRotation = t * 360;
            buttonRect.eulerAngles = new Vector3(0, 0, newRotation);
            if (timer >= fastFadeTime) timer = 0;
            yield return null;
        }
    }

    public void ToggleFadingText()
    {
        if (textFadeLooper == null) textFadeLooper = StartCoroutine(TextFadeLoop());
        else
        {
            secondText.color = secondTextColorNormal;
            StopCoroutine(textFadeLooper);
            textFadeLooper = null;
        }
    }

    private IEnumerator TextFadeLoop()
    {
        float timer = 0;
        float t = 0;
        bool fadeIn = false;
        EasingFunction.Function func = GetEasingFunction(textFadeEase);
        Color newColor = secondText.color;

        while (true)
        {
            if (timer < fastFadeTime && !fadeIn)
            {
                timer += Time.deltaTime;
            }
            else
            {
                timer -= Time.deltaTime;
            }

            t = timer / fastFadeTime;
            t = func(0f, 1f, t);
            newColor.a = t;
            secondText.color = newColor;
            if (!fadeIn && timer >= fastFadeTime) fadeIn = true;
            else if (fadeIn && timer <= 0) fadeIn = false;
            yield return null;
        }
    }

    public void TriggerFirstFade()
    {
        StartCoroutine(FadeItem(titleMenu));
        StartCoroutine(FadeItem(wowObject));
        StartCoroutine(FadeText(firstText));
        StartCoroutine(FadeTimeout());
        wowButton.interactable = false;
    }

    private IEnumerator FadeTimeout()
    {
        yield return new WaitForSeconds(fadeTime);
        secondMenu.SetActive(true);
        StartCoroutine(FadeInText(secondText));
        StartCoroutine(FadeInItem(button1Obj));
        StartCoroutine(FadeInItem(button2Obj));
        StartCoroutine(FadeInItem(button3Obj));
        StartCoroutine(FadeInText(button1Text));
        StartCoroutine(FadeInText(button2Text));
        StartCoroutine(FadeInText(button3Text));
        yield return null;
    }

    private IEnumerator FadeItem(GameObject fadeObject)
    {
        float timer = 0;
        Image fadeImage = fadeObject.GetComponent<Image>();
        Color fadeColor = fadeImage.color;

        while (timer < fadeTime)
        {
            timer += Time.deltaTime;
            fadeColor.a = Mathf.Lerp(1f, 0f, timer / fadeTime);
            fadeImage.color = fadeColor;
            yield return null;
        }
        fadeObject.SetActive(false);
    }

    private IEnumerator FadeText(TextMeshProUGUI fadeText)
    {
        float timer = 0;
        Color fadeColor = fadeText.color;

        while (timer < fadeTime)
        {
            timer += Time.deltaTime;
            fadeColor.a = Mathf.Lerp(1f, 0f, timer / fadeTime);
            fadeText.color = fadeColor;
            yield return null;
        }
        fadeText.gameObject.SetActive(false);
    }

    private IEnumerator FadeInItem(GameObject fadeObject)
    {
        fadeObject.SetActive(true);
        float timer = 0;
        Image fadeImage = fadeObject.GetComponent<Image>();
        Color fadeColor = fadeImage.color;

        while (timer < fadeTime)
        {
            timer += Time.deltaTime;
            fadeColor.a = Mathf.Lerp(0f, 1f, timer / fadeTime);
            fadeImage.color = fadeColor;
            yield return null;
        }
    }

    private IEnumerator FadeInText(TextMeshProUGUI fadeText)
    {
        fadeText.gameObject.SetActive(true);
        float timer = 0;
        Color fadeColor = fadeText.color;

        while (timer < fadeTime)
        {
            timer += Time.deltaTime;
            fadeColor.a = Mathf.Lerp(0f, 1f, timer / fadeTime);
            fadeText.color = fadeColor;
            yield return null;
        }
    }
}
