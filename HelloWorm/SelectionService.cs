using System.Collections;
using System.ComponentModel.Design;

namespace ei8.Prototypes.HelloWorm
{
    internal class SelectionService : ISelectionService
    {
        private ICollection? selectedComponents;
        private SelectionTypes selectionType;

        public object? PrimarySelection => 
            this.selectedComponents != null ? 
            this.ToEnumerable(this.selectedComponents).FirstOrDefault() : 
            null;

        public int SelectionCount => 
            this.selectedComponents != null ? 
            this.selectedComponents.Count : 
            0;

        public event EventHandler? SelectionChanged;
        public event EventHandler? SelectionChanging;

        public bool GetComponentSelected(object component)
        {
            bool result;

            if (this.selectedComponents == null)
                result = false;
            else
            {
                var e = this.ToEnumerable(this.selectedComponents);
                result = e.Any(a => a == component);
                return result;
            }

            return result;
        }

        private IEnumerable<object> ToEnumerable(ICollection collection)
        {
            var result = new object[collection.Count];
            collection.CopyTo(result, 0);
            return result;
        }

        public ICollection GetSelectedComponents()
        {
            ICollection result;

            if (this.selectedComponents == null)
                result = Array.Empty<object>();
            else
                result = this.selectedComponents;

            return result;
        }

        public void SetSelectedComponents(ICollection? components) =>
            this.SetSelectedComponents(components, SelectionTypes.Primary);

        public void SetSelectedComponents(ICollection? components, SelectionTypes selectionType)
        {
            this.SelectionChanging?.Invoke(this, EventArgs.Empty);

            this.selectedComponents = components;
            this.selectionType = selectionType;

            this.SelectionChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
