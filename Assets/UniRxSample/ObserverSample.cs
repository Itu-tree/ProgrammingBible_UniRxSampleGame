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
        public void OnCompleted()
        {
            Debug.Log("ログの発行が完了しました。");
        }

        public void OnError(Exception error)
        {
            Debug.Log($"例外が発生しました:{error}");
        }

        public void OnNext(string value)
        {
            Debug.Log($"メッセージが発行されました：{value}");
        }
    }
}
