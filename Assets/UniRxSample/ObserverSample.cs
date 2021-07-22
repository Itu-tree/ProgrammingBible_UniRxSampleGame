using System;
using UnityEngine;

namespace UniRxSample
{
    class ObserverSample :MonoBehaviour
    {
        private void Start()
        {
            //Obserberを手動生成
            IObserver<string> observer = new DisplayLogObserver();

            observer.OnNext("Hello");
            observer.OnNext("World");
            observer.OnCompleted();
        }
    }

    public sealed class DisplayLogObserver : IObserver<string>
    {
        private int _id;
        public DisplayLogObserver(int id = 1)
        {
            _id = id;
        }
        public void OnCompleted()
        {
            Debug.Log($"Observer{_id}のログの発行が完了しました。");
        }

        public void OnError(Exception error)
        {
            Debug.Log($"Observer{_id}に例外が発生しました:{error}");
        }

        public void OnNext(string value)
        {
            Debug.Log($"Observer{_id}からメッセージが発行されました：{value}");
        }
    }
}
