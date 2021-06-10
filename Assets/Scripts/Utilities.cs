using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utillities : MonoBehaviour
{
    // _dataArray re-shuffle, disrupt the order
    public static Coord[] ShuffleCoords(Coord[] _dataArray)
    {
        for (int i = 0; i < _dataArray.Length; i++)
        {
            int randomNum = Random.Range(i, _dataArray.Length);

            // Swap
            Coord temp = _dataArray[randomNum];
            _dataArray[randomNum] = _dataArray[i];
            _dataArray[i] = temp;
        }

        return _dataArray;
    }
}

// public class FisherYatesShuffle_Algorithm : MonoBehaviour
// {
//     public List<int> dataList = new List<int>() { 1, 2, 3, 4, 5, 10, 20, 30, 40, 50 };

//     private void Start()
//     {
//         FisherList<int>(dataList);
//     }

//     public void FisherList<T>(List<T> _dataList)
//     {
//         List<T> cache = new List<T>();

//         while (_dataList.Count > 0)
//         {
//             int randomIndex = Random.Range(0, _dataList.Count);
//             cache.Add(_dataList[randomIndex]);
//             _dataList.RemoveAt(randomIndex);
//             Debug.Log(_dataList.Count);
//         }

//         for (int i = 0; i < cache.Count; i++)
//         {
//             _dataList.Add(cache[i]);
//         }
//     }
// }

// public class KnuthDurstenfeldShuffle_Algorithm : MonoBehaviour
// {
//     public List<int> dataList = new List<int>() { 1, 2, 3, 4, 5, 10, 20, 30, 40, 50 };

//     private void Start()
//     {
//         FisherList<int>(dataList);
//     }

//     public void FisherList<T>(List<T> _dataList)
//     {
//         for (int i = 0; i < _dataList.Count - 1; i++)
//         {
//             int randomIndex = Random.Range(i, _dataList.Count);

//             T temp = _dataList[randomIndex];
//             _dataList[randomIndex] = _dataList[i];
//             _dataList[i] = temp;
//             Debug.Log(i);
//         }
//     }
// }