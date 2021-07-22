using System;
using UniRx;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniRxSample
{
    // Subjectのサンプル実装
    public sealed class MySubject<T> : ISubject<T>, IDisposable
    {
        //排他ロック用のオブジェクト
        private readonly object _gate = new object();

        //登録されたObserver一覧
        private List<IObserver<T>> _observers;

        // このSubjectが破棄されたか
        private bool _isDisposed;

        //このSubjectが停止状態となっているか
        private bool _isStopped;

        //最後に発行された例外
        private Exception _error;

        #region IObserver<T>
        public void OnCompleted()
        {
            lock (_gate)
            {
                ThrowIfDisposed();
                if (_isStopped) return;
                if(_observers != null)
                {
                    foreach (var observer in _observers)
                    {
                        observer.OnCompleted();
                    }
                    _observers.Clear();
                    _observers = null;
                }
                // 動作停止
                _isStopped = true;
            }
        }

        public void OnError(Exception error)
        {
            lock (_gate)
            {
                ThrowIfDisposed();
                if (_isStopped) return;
                _error = error;
                if(_observers != null)
                {
                    foreach (var observer in _observers)
                    {
                        observer.OnError(error);
                    }
                    _observers.Clear();
                    _observers = null;
                }
                // 動作停止
                _isStopped = true;
            }
        }


        public void OnNext(T value)
        {
            lock(_gate)
            {
                ThrowIfDisposed();
                if (_isStopped) return;
                if(_observers != null)
                {
                    foreach (var observer in _observers)
                    {
                        observer.OnNext(value);
                    }
                }
            }
        }
        #endregion

        #region IObservable<T>
        // IObserver<T>をSubjectに登録する処理
        public IDisposable Subscribe(IObserver<T> observer)
        {
            if (observer == null) throw new ArgumentNullException();
            lock (_gate)
            {
                ThrowIfDisposed();
                if(!_isStopped)
                {
                    //Observerのリストに登録する
                    if (_observers == null) _observers = new List<IObserver<T>>();
                    _observers.Add(observer);

                    //IDisposable.Dispose()で購読中断できるように inner classでラップする

                    return new Subscription(observer, this);
                }
                else
                {
                    //異常終了/正常終了しているならそれを伝え終了
                    if (_error != null)
                    {
                        observer.OnError(_error);
                    }
                    else
                    {
                        observer.OnCompleted();
                    }

                    return Disposable.Empty;
                }
            }
        }
        #endregion

        private void ThrowIfDisposed()
        {
            lock (_gate)
            {
                if (_isDisposed) throw new ObjectDisposedException(nameof(GetType));
            }
        }
        public void Dispose()
        {
            lock (_gate)
            {
                if (_isDisposed) return;
                _isDisposed = true;
                _observers = null;
            }            
        }

        /// <summary>
        /// 購読状態を管理する innner class
        /// Dispose()を呼ぶとSubjectからObserverを削除する
        /// </summary>
        private sealed class Subscription : IDisposable
        {
            private readonly IObserver<T> _observer;
            private MySubject<T> _parent;
            private readonly object _gate = new object();
            public Subscription(IObserver<T> observer, MySubject<T> parent)
            {
                _observer = observer;
                _parent = parent;
            }

            public void Dispose()
            {
                lock (_gate)
                {
                    if (_parent == null) return;
                    lock (_parent._gate)
                    {
                        // Observerの登録解除
                        _parent._observers?.Remove(_observer);
                        _parent = null;
                    }

                }
            }
        }


    }
}
