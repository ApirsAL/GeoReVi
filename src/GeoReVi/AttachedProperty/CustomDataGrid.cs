
namespace GeoReVi
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Input;

    /// <summary>
    /// FROM https://stackoverflow.com/questions/4118617/wpf-datagrid-pasting
    /// </summary>
    public class CustomDataGrid : DataGrid
    {
        public event ExecutedRoutedEventHandler ExecutePasteEvent;
        public event CanExecuteRoutedEventHandler CanExecutePasteEvent;

        // ******************************************************************
        static CustomDataGrid()
        {
            CommandManager.RegisterClassCommandBinding(
                typeof(CustomDataGrid),
                new CommandBinding(ApplicationCommands.Paste,
                    new ExecutedRoutedEventHandler(OnExecutedPasteInternal),
                    new CanExecuteRoutedEventHandler(OnCanExecutePasteInternal)));
        }

        // ******************************************************************
        #region Clipboard Paste

        // ******************************************************************
        private static void OnCanExecutePasteInternal(object target, CanExecuteRoutedEventArgs args)
        {
            ((CustomDataGrid)target).OnCanExecutePaste(target, args);
        }

        // ******************************************************************
        /// <summary>
        /// This virtual method is called when ApplicationCommands.Paste command query its state.
        /// </summary>
        /// <param name="args"></param>
        protected virtual void OnCanExecutePaste(object target, CanExecuteRoutedEventArgs args)
        {
            if (CanExecutePasteEvent != null)
            {
                CanExecutePasteEvent(target, args);
                if (args.Handled)
                {
                    return;
                }
            }

            args.CanExecute = CurrentCell != null;
            args.Handled = true;
        }

        // ******************************************************************
        private static void OnExecutedPasteInternal(object target, ExecutedRoutedEventArgs args)
        {
            ((CustomDataGrid)target).OnExecutedPaste(target, args);
        }

        // ******************************************************************
        /// <summary>
        /// This virtual method is called when ApplicationCommands.Paste command is executed.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="args"></param>
        protected virtual void OnExecutedPaste(object sender, ExecutedRoutedEventArgs args)
        {
            // parse the clipboard data
            List<string[]> rowData = ClipboardHelper.ParseClipboardData();
            bool hasAddedNewRow = false;

            // call OnPastingCellClipboardContent for each cell
            int minRowIndex = Math.Max(Items.IndexOf(CurrentItem), 0);
            int maxRowIndex = Items.Count - 1;
            int minColumnDisplayIndex = (SelectionUnit != DataGridSelectionUnit.FullRow) ? Columns.IndexOf(CurrentColumn) : 0;
            int maxColumnDisplayIndex = Columns.Count - 1;

            int rowDataIndex = 0;
            for (int i = minRowIndex; i <= maxRowIndex && rowDataIndex < rowData.Count; i++, rowDataIndex++)
            {
                if (CanUserAddRows && i == maxRowIndex)
                {
                    // add a new row to be pasted to
                    ICollectionView cv = CollectionViewSource.GetDefaultView(Items);
                    IEditableCollectionView iecv = cv as IEditableCollectionView;
                    if (iecv != null)
                    {
                        hasAddedNewRow = true;
                        iecv.AddNew();
                        if (rowDataIndex + 1 < rowData.Count)
                        {
                            // still has more items to paste, update the maxRowIndex
                            maxRowIndex = Items.Count - 1;
                        }
                    }
                }
                else if (i == maxRowIndex)
                {
                    continue;
                }

                int columnDataIndex = 0;
                for (int j = minColumnDisplayIndex; j < maxColumnDisplayIndex && columnDataIndex < rowData[rowDataIndex].Length; j++, columnDataIndex++)
                {
                    DataGridColumn column = ColumnFromDisplayIndex(j);
                    string propertyName = ((column as DataGridBoundColumn).Binding as Binding).Path.Path;
                    object item = Items[i];
                    object value = rowData[rowDataIndex][columnDataIndex];
                    PropertyInfo pi = item.GetType().GetProperty(propertyName);
                    if (pi != null)
                    {
                        object convertedValue = Convert.ChangeType(value, pi.PropertyType);
                        item.GetType().GetProperty(propertyName).SetValue(item, convertedValue, null);
                    }

                    column.OnPastingCellClipboardContent(item, rowData[rowDataIndex][columnDataIndex]);
                }
            }
        }

        // ******************************************************************
        /// <summary>
        ///     Whether the end-user can add new rows to the ItemsSource.
        /// </summary>
        public bool CanUserPasteToNewRows
        {
            get { return (bool)GetValue(CanUserPasteToNewRowsProperty); }
            set { SetValue(CanUserPasteToNewRowsProperty, value); }
        }

        // ******************************************************************
        /// <summary>
        ///     DependencyProperty for CanUserAddRows.
        /// </summary>
        public static readonly DependencyProperty CanUserPasteToNewRowsProperty =
            DependencyProperty.Register("CanUserPasteToNewRows",
                                        typeof(bool), typeof(CustomDataGrid),
                                        new FrameworkPropertyMetadata(true, null, null));

        // ******************************************************************
        #endregion Clipboard Paste

        private void SetGridToSupportManyEditEitherWhenValidationErrorExists()
        {
            this.Items.CurrentChanged += Items_CurrentChanged;


            //Type DatagridType = this.GetType().BaseType;
            //PropertyInfo HasCellValidationProperty = DatagridType.GetProperty("HasCellValidationError", BindingFlags.NonPublic | BindingFlags.Instance);
            //HasCellValidationProperty.
        }

        void Items_CurrentChanged(object sender, EventArgs e)
        {
            //this.Items[0].
            //throw new NotImplementedException();
        }

        // ******************************************************************
        private void SetGridWritable()
        {
            Type DatagridType = this.GetType().BaseType;
            PropertyInfo HasCellValidationProperty = DatagridType.GetProperty("HasCellValidationError", BindingFlags.NonPublic | BindingFlags.Instance);
            if (HasCellValidationProperty != null)
            {
                HasCellValidationProperty.SetValue(this, false, null);
            }
        }

        // ******************************************************************
        public void SetGridWritableEx()
        {
            BindingFlags bindingFlags = BindingFlags.FlattenHierarchy | BindingFlags.NonPublic | BindingFlags.Instance;
            PropertyInfo cellErrorInfo = this.GetType().BaseType.GetProperty("HasCellValidationError", bindingFlags);
            PropertyInfo rowErrorInfo = this.GetType().BaseType.GetProperty("HasRowValidationError", bindingFlags);
            cellErrorInfo.SetValue(this, false, null);
            rowErrorInfo.SetValue(this, false, null);
        }

        // ******************************************************************
    }
}
