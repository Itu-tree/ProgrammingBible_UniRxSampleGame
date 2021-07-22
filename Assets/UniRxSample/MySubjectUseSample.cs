using UnityEngine;

namespace UniRxSample
{
    public sealed class MySubjectUseSample : MonoBehaviour
    {
        private void Start()
        {   
            //MySubject作成
            var mySubject = new MySubject<string>();

            // Observer登録
            var disposable1 = mySubject.Subscribe(new DisplayLogObserver(1));
            var disposable2 = mySubject.Subscribe(new DisplayLogObserver(2));

            //メッセージ送信
            mySubject.OnNext("Hello");

            //購読中止
            disposable1.Dispose();
            mySubject.OnNext("World");

            //送信の終了
            mySubject.OnCompleted();
            mySubject.Dispose();

        }
    }
}
