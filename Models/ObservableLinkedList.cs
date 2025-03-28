﻿using System.Collections.Generic;
using System.Collections.Specialized;

namespace Chess_game.Models
{
    public class ObservableLinkedList<T> : LinkedList<T>, INotifyCollectionChanged
    {
        public event NotifyCollectionChangedEventHandler? CollectionChanged;

        public new void AddLast(T item)
        {
            base.AddLast(item);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
        }

        public new void RemoveLast()
        {
            if(base.Count > 0)
            {
                T item = base.Last!.Value;
                base.RemoveLast();
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));

            }
        }

        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            CollectionChanged?.Invoke(this, e);
        }
    }
}
