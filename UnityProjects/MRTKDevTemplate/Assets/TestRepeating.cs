using System.Collections;
using UnityEngine;

public class TestRepeating : MonoBehaviour
{
    private static int waitForSecondsCount = 1;
    private float nextTime = 0;

    protected void Start()
    {
        for (int i = 0; i < 10000; i++)
        {
            StartCoroutine(TestCoroutine());
            InvokeRepeating(nameof(TestInvokeRepeating), 0.02f, waitForSecondsCount);
        }
    }

    protected void Update()
    {
        if (Time.time >= nextTime)
        {
            for (int i = 0; i < 10000; i++)
            {
                int okay = 5;
            }
            nextTime = Mathf.FloorToInt(Time.time) + waitForSecondsCount;
        }
    }

    private const string Repeating = "InvokeRepeating running...";

    private void TestInvokeRepeating()
    {
        //Debug.Log(Repeating);
        for (int i = 0; i < 1000; i++)
        {
            int okay = 5;
        }
    }

    private const string Coroutine = "Coroutine running...";
    private static readonly WaitForSeconds Wait = new WaitForSeconds(waitForSecondsCount);

    private IEnumerator TestCoroutine()
    {
        while (true)
        {
            //Debug.Log(Coroutine);
            for (int i = 0; i < 1000; i++)
            {
                int okay = 5;
            }
            yield return Wait;
        }
    }
}
