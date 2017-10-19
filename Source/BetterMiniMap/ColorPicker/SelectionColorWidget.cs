using UnityEngine;

namespace ColorPicker.Dialog
{
    delegate void SelectionChange(SelectionColorWidget selectionColorWidget);

    class SelectionColorWidget
    {
        public SelectionChange selectionChange;

        public readonly Color originalColor;

        private Color selectedColor;

        public Color SelectedColor
        {
            get { return this.selectedColor; }
            set
            {
                if (!this.selectedColor.Equals(value))
                {
                    this.selectedColor = value;
                    this.selectionChange?.Invoke(this);
                }
            }
        }

        public SelectionColorWidget(Color color)
        {
            this.originalColor = color;
            this.selectedColor = color;
        }

        public void ResetToDefault()
        {
            this.selectedColor = this.originalColor;
        }
    }
}