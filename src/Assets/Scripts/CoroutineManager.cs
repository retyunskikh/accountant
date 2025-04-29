using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoroutineManager : MonoBehaviour
{
    private List<Coroutine> activeCoroutines = new List<Coroutine>();
    private List<IEnumerator> routines = new List<IEnumerator>();

    private static CoroutineManager _instance;
    public static CoroutineManager Instance
    {
        get
        {
            if (_instance == null)
            {
                var obj = new GameObject("CoroutineManager");
                _instance = obj.AddComponent<CoroutineManager>();
                DontDestroyOnLoad(obj);
            }
            return _instance;
        }
    }

    public Coroutine StartManagedCoroutine(IEnumerator routine)
    {
        routines.Add(routine);
        var coroutine = StartCoroutine(RunAndTrack(routine));
        activeCoroutines.Add(coroutine);
        return coroutine;
    }

    public void StopAllManagedCoroutines()
    {
        foreach (var coroutine in activeCoroutines)
        {
            if (coroutine != null)
                StopCoroutine(coroutine);
        }
        activeCoroutines.Clear();
        routines.Clear();
    }

    private IEnumerator RunAndTrack(IEnumerator routine)
    {
        yield return routine;
        routines.Remove(routine);
    }
}