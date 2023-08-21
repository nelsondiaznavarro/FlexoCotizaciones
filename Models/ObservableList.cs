using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlexoCotizaciones.Models;


public class ObservableList<T> : List<T>, INotifyCollectionChanged
{
    public event NotifyCollectionChangedEventHandler CollectionChanged;

    public new void Add(T item)
    {
        base.Add(item);
        CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
    }

    public new void Remove(T item)
    {
        if (base.Remove(item))
        {
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));
        }
    }

    // Implement other methods like Clear, Insert, etc., following the same pattern.
}
