using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace BabelIm.Controls.PivotControl {
    public abstract class PivotItemCollectionAdaptor<T> : ObservableCollection<PivotItem> {
        public PivotItemCollectionAdaptor() {
        }

        public PivotItemCollectionAdaptor(IList<T> list) {
            foreach (T item in list)
            {
                Add(item);
            }
        }

        public new IList<T> Items {
            get {
                var list = new List<T>();
                foreach (PivotItem pivot in base.Items)
                {
                    T item = OnGetItem(pivot);
                    list.Add(item);
                }
                return list;
            }
        }

        public new T this[int index] {
            get { return GetItem(base[index]); }
            set { SetItem(base[index], value); }
        }

        public T this[PivotItem pivot] {
            get { return GetItem(pivot); }
            set { SetItem(pivot, value); }
        }

        protected abstract T OnGetItem(PivotItem pivot);
        protected abstract void OnSetItem(PivotItem pivot, T item);

        protected virtual void OnCreateItem(PivotItem pivot, T item) {
            OnSetItem(pivot, item);
        }

        public T GetItem(PivotItem pivot) {
            if (!base.Contains(pivot))
                throw new KeyNotFoundException();

            return OnGetItem(pivot);
        }

        public void SetItem(PivotItem pivot, T item) {
            if (!base.Contains(pivot))
                throw new KeyNotFoundException();

            OnSetItem(pivot, item);
        }

        public void InsertItem(int index, T item) {
            var pivot = new PivotItem();
            OnCreateItem(pivot, item);
            base.InsertItem(index, pivot);
        }

        public void Add(T item) {
            var pivot = new PivotItem();
            OnCreateItem(pivot, item);
            base.Add(pivot);
        }

        public void Remove(T item) {
            foreach (PivotItem pivot in base.Items)
            {
                T val = OnGetItem(pivot);
                if (item.Equals(val))
                {
                    base.Remove(pivot);
                    return;
                }
            }
        }
    }
}