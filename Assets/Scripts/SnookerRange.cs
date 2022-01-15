using UnityEngine;

public class SnookerRange : MonoBehaviour {
    public Vector2 offset = new Vector2(2f, 3f);

    public bool CheckOffset(Vector2 obj)
    {
        Vector2 minV = new Vector2(transform.position.x - offset.x, transform.position.y - offset.y);
        Vector2 maxV = new Vector2(transform.position.x + offset.x, transform.position.y + offset.y);

        bool checkMin = obj.x < minV.x || obj.y < minV.y;
        bool checkMax = obj.x > maxV.x || obj.y > maxV.y;

        if (checkMin || checkMax) return false;
        else return true;
    }
}