using System.Collections;
using UnityEngine;
using static EasingFunction;

public class TweenDropBall : MonoBehaviour
{
    [SerializeField] private Ease ballBounceEase = Ease.EaseOutBounce;
    [SerializeField] private GameObject ballPrefab;
    [SerializeField] private RectTransform canvasTransform;
    [SerializeField] private float bounceTime = 2f;
    [SerializeField] private float disappearDelay = 0.4f;
    [SerializeField] private float ballYOffset = 5f;
    
    public void DropBall()
    {
        StartCoroutine(BallBounce());
    }

    private IEnumerator BallBounce()
    {
        float timer = 0;
        float t;
        Function func = GetEasingFunction(ballBounceEase);
        float newY = 0;
        RectTransform ballRect = Instantiate(ballPrefab, canvasTransform).GetComponent<RectTransform>();
        Vector2 startPos = new(Random.Range(0f, canvasTransform.rect.width * canvasTransform.localScale.x), 0f);

        while (timer < bounceTime)
        {
            timer += Time.deltaTime;

            t = timer / bounceTime;
            t = func(1f, 0f, t);
            newY = t * canvasTransform.rect.height;
            ballRect.position = new Vector2(startPos.x, (newY + ballRect.rect.height / 2) * canvasTransform.localScale.y - ballYOffset);

            yield return null;
        }

        ballRect.position = new Vector2(startPos.x, (0f + ballRect.rect.height / 2) * canvasTransform.localScale.y);

        yield return new WaitForSeconds(disappearDelay);
        Destroy(ballRect.gameObject);
    }
}