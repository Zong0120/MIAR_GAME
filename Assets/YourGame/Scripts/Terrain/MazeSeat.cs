using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MazeSeat : MonoBehaviour
{
    [SerializeField] private Image OverEffectImage;
    [SerializeField] private GameObject maze;
    [SerializeField] private GameObject seat;
    
    void OnTriggerEnter2D(Collider2D other)
    {
        //side text
    }

    void OnTriggerExit2D(Collider2D other)
    {
        StartCoroutine(SeatChage());
    }

    IEnumerator SeatChage()
    {
        OverEffectImage.gameObject.SetActive(true);
        float elapsedTime = 0f;
        float fadeDuration = 0.5f;
        while (elapsedTime < fadeDuration)
        {
            float newOpacity = Mathf.Lerp(0, 1, elapsedTime / fadeDuration);
            OverEffectImage.color = new Color(0, 0, 0, newOpacity);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        OverEffectImage.color = new Color(0, 0, 0, 1);
        seat.SetActive(true);
        GetComponent<SpriteRenderer>().enabled = false;

        elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            float newOpacity = Mathf.Lerp(1, 0, elapsedTime / fadeDuration);
            OverEffectImage.color = new Color(0, 0, 0, newOpacity);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        OverEffectImage.color = new Color(0, 0, 0, 0);
        OverEffectImage.gameObject.SetActive(false);

        Destroy(maze);
    }

}
