using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

[RequireComponent(typeof(Collider2D))]
public class PoolHoleCollider : MonoBehaviour
{
    [SerializeField]
    private List<int> penaltyCues;
    [SerializeField]
    private Rigidbody2D snookerRigidbody;
    [SerializeField]
    private GameObject snookerObject;
    [SerializeField]
    private GameObject restartPanel;
    [SerializeField]
    private GameObject winPanel;
    private static int instanceCount = 0;

    // Use this for initialization  
    void Start()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other) {
        GameObject colObj = other.gameObject;
        colObj.GetComponent<Rigidbody2D>().Sleep();
        colObj.transform.position = other.transform.position;
        if (colObj.GetComponent<CueBall>() != null)
        {
            int score = colObj.GetComponent<CueBall>().awardedScore;

            Destroy(colObj.GetComponent<CueBall>());
            Destroy(colObj.GetComponent<Rigidbody2D>());
            Destroy(colObj.GetComponent<CircleCollider2D>());

            StartCoroutine(DestroyCueBall(colObj, 1f, score));
        }
        else if (colObj.GetComponent<ShotSnooker>() != null)
        {
            colObj.GetComponent<ShotSnooker>().canShot = false;
            colObj.GetComponent<Rigidbody2D>().velocity = new Vector2(0f, 0f);
            colObj.GetComponent<CircleCollider2D>().enabled = false;

            StartCoroutine(ToggleSnooker(colObj.GetComponent<ShotSnooker>(), 1f));
        }
    }

    private IEnumerator DestroyCueBall(GameObject obj, float duration, int scoreAmount = 0)
    {
        instanceCount++;

        yield return StartCoroutine(DisappearTransition(obj, duration));

        Destroy(obj);

        string cueMessage = "Got ";
        if (penaltyCues.Contains(scoreAmount))
        {
            scoreAmount = -scoreAmount;
            cueMessage =  "Penalty cue - Lose ";
            if (GlobalVar.isPenaltyCuesObjective) GlobalVar.cueBallObjective--;
        } 
        else GlobalVar.cueBallObjective--;

        if (GlobalVar.cueBallObjective <= 0)
        {
            GlobalVar.isWin = true;
            StartCoroutine(WaitForSnookerStop());
        }

        GlobalProc.AddScore(GlobalProc.GetGlobalVar().scoreText, scoreAmount);
        GlobalProc.EditMessage(GlobalProc.GetGlobalVar().messageText, cueMessage + Mathf.Abs(scoreAmount) + " points!");

        instanceCount--;
    }

    private IEnumerator ToggleSnooker(ShotSnooker obj, float duration, int scoreAmount = 0)
    {
        instanceCount++;

        yield return StartCoroutine(DisappearTransition(obj.gameObject, duration));

        obj.GetComponent<LineRenderer>().enabled = false;
        obj.pivotObject = null;
        obj.enabled = false;
        obj.gameObject.SetActive(false);
        GlobalProc.EditMessage(GlobalProc.GetGlobalVar().messageText, "A foul shot! Waiting for cues to settle...");
        yield return StartCoroutine(WaitForCueBallsSettle(obj));
        yield return StartCoroutine(WaitForChooseSnookerSpawn(obj.snookerSpawnPoint));

        if (GlobalVar.score > 10)
        {
            scoreAmount = Mathf.CeilToInt(-GlobalVar.score / 2);
            GlobalProc.AddScore(GlobalProc.GetGlobalVar().scoreText, scoreAmount);
            GlobalProc.EditMessage(GlobalProc.GetGlobalVar().messageText, "Foul shot - Lose half of points!");
        }
        else
        {
            GlobalProc.AddScore(GlobalProc.GetGlobalVar().scoreText, -5);
            GlobalProc.EditMessage(GlobalProc.GetGlobalVar().messageText, "Foul shot! Lose 5 points!");
        }

        // Reset scale (from Vector3.zero)
        obj.transform.localScale = new Vector3(0.5f, 0.5f, 1f);
        obj.transform.position = obj.snookerSpawnPoint.transform.position;
        obj.GetComponent<CircleCollider2D>().enabled = true;
        obj.GetComponent<Rigidbody2D>().WakeUp();
        obj.GetComponent<ShotSnooker>().canShot = true;       
        obj.gameObject.SetActive(true);

        while (Input.GetMouseButton(0)) yield return new WaitForEndOfFrame();
        obj.enabled = true;
        ShotSnooker.overrideReady = true;

        instanceCount--;
    }

    private IEnumerator DisappearTransition(GameObject obj, float duration)
    {
        Vector3 defaultScale = obj.transform.localScale;
        float t = 0f;

        while (obj.transform.localScale != new Vector3(0f, 0f, 0f))
        {
            t += Time.deltaTime;

            obj.transform.localScale = Vector3.Lerp(
                defaultScale,
                new Vector3(0f, 0f, 0f),
                Mathf.Clamp(t / duration, 0, 1)
            );

            yield return new WaitForSeconds(Time.deltaTime);
        }       

        yield return 0;
    }

    private IEnumerator WaitForCueBallsSettle(Snooker sn)
    {
        while (!GlobalProc.CheckCueBallsThreshold(sn))
        {
            yield return new WaitForFixedUpdate();
        }
    }

    private IEnumerator WaitForChooseSnookerSpawn(SnookerSpawner spawnObj)
    {
        spawnObj.gameObject.SetActive(true);

        bool isHold = Input.GetMouseButton(0);

        GlobalProc.EditMessage(GlobalProc.GetGlobalVar().messageText, "Choose a new position for your snooker...");

        bool isReversedMouse = false;
        bool isCorrectPosition = false;
        while (!isCorrectPosition)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = -3f;
            spawnObj.transform.position = mousePos;

            isReversedMouse = Input.GetMouseButton(0);
            if (isHold) isReversedMouse = !isReversedMouse;

            if (spawnObj.isCorrectlyAlign && spawnObj.isInRange && isReversedMouse) isCorrectPosition = true;
            else yield return new WaitForEndOfFrame();
        }

        spawnObj.gameObject.SetActive(false);
    }

    private IEnumerator WaitForSnookerStop()
    {
        while (snookerRigidbody.velocity.magnitude > 0f)
        {
            yield return new WaitForFixedUpdate();
        }

        while (instanceCount > 0) yield return new WaitForEndOfFrame();

        int hs = GlobalProc.LoadHighscore();
        if (GlobalVar.score > hs)
        {
            GlobalProc.SetHighscore(GlobalVar.score);
            GlobalProc.EditMessage(GlobalProc.GetGlobalVar().messageText, "New Highscore!");
        }
        else if (GlobalVar.score == hs)
        {
            GlobalProc.EditMessage(GlobalProc.GetGlobalVar().messageText, "Tie with Highscore!");
        }
        else if (GlobalVar.score > 0)
        {
            int delta = hs - GlobalVar.score;
            GlobalProc.EditMessage(GlobalProc.GetGlobalVar().messageText, delta + " points to go!");
        }
        else
        {
            GlobalProc.EditMessage(GlobalProc.GetGlobalVar().messageText, "Try harder next time...");
        }
        restartPanel.SetActive(false);
        winPanel.SetActive(true);
    }
}
