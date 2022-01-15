using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ShotSnooker : Snooker
{
    public static bool overrideReady = false;

    [HideInInspector]
    public bool canShot = true;
    [HideInInspector]
    public bool isWaiting = false;
	public float thurstCap = 3f;
	public float cancelCap = 0.2f;

    [Tooltip("Preventing player from using the same shot angle everytime")]
    public Vector2 deviation = new Vector2(-0.1f, 0.1f);

    [HideInInspector]
    public SnookerPivot pivotObject;

    [SerializeField]
    private SnookerPivot pivotPrefab;
    [SerializeField]
    private LineRenderer lineRenderer;
    [SerializeField]
    private Gradient fireGradient;
    [SerializeField]
    private Gradient delayGradient;
    [SerializeField]
    private Gradient cancelGradient;
    [SerializeField]
    private AnimationCurve ac;

    float getForwardAngle(float angle)
    {
        angle = -angle - 90f; // Change the angle system to clockwise, forward
        if (angle < 0f) angle = 360f + angle; // Normalize angle in range (0, 360)
        if (angle > 180f) angle = -(360f - angle); // Split angle into negative (left) and positive (right) parts
        return angle;
    }

    // Use this for initialization
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        transform.position = new Vector3(
            transform.position.x + Random.Range(-deviation.x, deviation.x), 
            transform.position.y + Random.Range(-deviation.y, deviation.y),
            transform.position.z
        );
        lineRenderer.positionCount = 2;
        lineRenderer.useWorldSpace = true;
		lineRenderer.numCapVertices = 3;
        lineRenderer.numCornerVertices = 2;
        lineRenderer.widthCurve = ac;
        lineRenderer.colorGradient = delayGradient;
    }

    private void Update()
    {
        if (GlobalVar.isWin) return;

        bool isShotAvailable = GlobalProc.CheckCueBallsThreshold(this) && canShot && velocityLimitState;
        if (isWaiting && isShotAvailable)
        {
            isWaiting = false;
            if (!overrideReady) 
            {
                GlobalProc.EditMessage(GlobalProc.GetGlobalVar().messageText, "Ready for next shot.");
            }
            else overrideReady = false;
        }

        if (Input.GetMouseButton(0))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // Is the line renderer initialized?
            if (Input.GetMouseButtonDown(0))
            {
                lineRenderer.enabled = true;
                lineRenderer.SetPosition(0, new Vector3(mousePos.x, mousePos.y, -3f));
                lineRenderer.SetPosition(1, new Vector3(mousePos.x, mousePos.y, -3f));
                pivotObject = Instantiate(pivotPrefab, new Vector3(mousePos.x, mousePos.y, -3f), Quaternion.identity);
                pivotObject.owner = this;
            }
            else
            {
                thrust = Mathf.Clamp(Vector3.Distance(new Vector3(mousePos.x, mousePos.y, -3f), pivotObject.transform.position), 0f, thurstCap);
                var dir = new Vector3(mousePos.x, mousePos.y, -3f) - pivotObject.transform.position;
                var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                if (isShotAvailable)
                {
					bool isEnoughForce = thrust >= cancelCap;
                    if (isEnoughForce)
					{
						lineRenderer.colorGradient = fireGradient;
                        pivotObject.sprite.color = fireGradient.colorKeys[0].color;
					}
					else
					{
						lineRenderer.colorGradient = cancelGradient;
                        pivotObject.sprite.color = cancelGradient.colorKeys[0].color;
					}
					string degreeText = "Aiming at " + System.Math.Round(getForwardAngle(angle), GlobalVar.precision) + " degrees";
					string powerText = (isEnoughForce) ? ", " + System.Math.Round(thrust, GlobalVar.precision) + " power" : "";
                    GlobalProc.EditMessage(GlobalProc.GetGlobalVar().messageText, degreeText + powerText);
                }
				else 
				{
					lineRenderer.colorGradient = delayGradient;
                    pivotObject.sprite.color = delayGradient.colorKeys[0].color;
				}
                pivotObject.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
                lineRenderer.SetPosition(1, new Vector3(mousePos.x, mousePos.y, -3f));
            }
        }
        else if (Input.GetMouseButtonUp(0) && !GlobalVar.isWin)
        {
            if (isShotAvailable)
            {
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                var dir = new Vector3(mousePos.x, mousePos.y, -3f) - pivotObject.transform.position;
                var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                if (thrust >= cancelCap && isShotAvailable) 
                {
                    FireSnooker(Quaternion.AngleAxis(angle + 90f, Vector3.forward), angle);
                    GlobalProc.ResetDeltaScore(GlobalProc.GetGlobalVar().scoreText);             
                }
                isWaiting = true;
            }

            Destroy(pivotObject.gameObject);
            lineRenderer.enabled = false;
        }
    }
}
