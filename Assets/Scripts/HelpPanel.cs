using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HelpPanel : MonoBehaviour
{
    public List<GameObject> pages;
    public List<int> checkpointsNum;
    public List<string> checkpointsName;

    [SerializeField]
    private Text checkpointText;

    private int curPage = 0;
    private int curCheckpoint = 0;

    public void reset()
    {
        foreach (var page in pages)
        {
            page.SetActive(false);
        }
    }

    protected void updateCheckpoint()
    {
        for (int i = 0; i < checkpointsNum.Count; i++)
        {
            if (curPage >= checkpointsNum[i])
            {
                if (i < checkpointsNum.Count - 1 && curPage >= checkpointsNum[i+1]) continue;
                checkpointText.text = checkpointsName[i];
                curCheckpoint = i;
            }
        }
    }

    public void goToPage(int page)
    {
        if (page < 0 || page > pages.Count - 1) return;

        curPage = page;
        reset();
        pages[curPage].SetActive(true);
        updateCheckpoint();
    }

    public void goToCheckpoint(int checkpoint)
    {
        int newCheckpoint = 0;
        if (curCheckpoint + 1 < checkpointsNum.Count) newCheckpoint = curCheckpoint + 1;
        goToPage(checkpointsNum[newCheckpoint]);
    }

    public void prevPage()
    {
        goToPage(curPage - 1);
    }

    public void nextPage()
    {
        goToPage(curPage + 1);
    }

    public void nextCheckpoint()
    {
        goToCheckpoint(curCheckpoint + 1);
    }

    public void prevCheckpoint()
    {
        goToCheckpoint(curCheckpoint - 1);
    }

    // Start is called before the first frame update
    void Start()
    {
        reset();
        goToPage(0);
    }
}
