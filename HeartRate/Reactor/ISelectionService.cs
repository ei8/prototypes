using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reactor
{
    public interface ISelectionService
    {
        event EventHandler SelectionChanged;

        IEnumerable<object> SelectedObjects { get; }

        void SetSelectedObjects(IEnumerable<object> selection);
    }
}
