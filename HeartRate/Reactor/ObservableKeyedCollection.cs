using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reactor
{
    // TODO: transfer to neurUL.Common
    // http://geekswithblogs.net/NewThingsILearned/archive/2010/01/12/make-keyedcollectionlttkey-titemgt-to-work-properly-with-wpf-data-binding.aspx
    public abstract class ObservableKeyedCollection<TKey, TItem> : KeyedCollection<TKey, TItem>, INotifyCollectionChanged
    {
        private bool deferNotifyCollectionChanged = false;

        // Overrides a lot of methods that can cause collection change
        protected override void SetItem(int index, TItem item)
        {
            var oldItem = base[index];
            base.SetItem(index, item);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, item, oldItem, index));
        }

        protected override void InsertItem(int index, TItem item)
        {
            base.InsertItem(index, item);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
        }

        protected override void ClearItems()
        {
            base.ClearItems();
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        protected override void RemoveItem(int index)
        {
            TItem item = this[index];
            base.RemoveItem(index);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));
        }

        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (!this.deferNotifyCollectionChanged)
                this.CollectionChanged?.Invoke(this, e);
        }

        #region INotifyCollectionChanged Members

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        #endregion
    }
}
