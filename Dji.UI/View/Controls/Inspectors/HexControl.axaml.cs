using Dji.UI.ViewModels.Controls.Inspectors;
using System.Collections.Generic;
using Avalonia.Markup.Xaml;
using Avalonia.Controls;
using System;
using System.Linq;

namespace Dji.UI.View.Controls.Inspectors
{
    public class HexControl : UserControl
    {
        private const double GRID_PLACEHOLDER_MARGIN = 10d;

        public HexControl() => InitializeComponent();

        private void InitializeComponent() => AvaloniaXamlLoader.Load(this);

        protected override void OnInitialized() => BuildUserInterfaces();

        protected override void OnDataContextChanged(EventArgs e) => BuildUserInterfaces();

        private void BuildUserInterfaces()
        {
            var hexGrid = this.FindControl<Grid>("HexGrid");
            var viewModel = this.DataContext as HexControlViewModel;

            if (hexGrid == null || viewModel == null) return;
            else if (hexGrid.Children.Count > 1) return;

            hexGrid.RowDefinitions = CalculateAndBuildRowDefinitions(viewModel);
            hexGrid.ColumnDefinitions = CalculateBuildColumnDefinitions(viewModel);
            hexGrid.Children.AddRange(BuildRowDescriptions(hexGrid.RowDefinitions.Count));
            hexGrid.Children.AddRange(BuildColumnDescriptions(hexGrid.ColumnDefinitions.Count));
            hexGrid.Children.AddRange(BuildHexValueEntries(viewModel, hexGrid.RowDefinitions.Count, hexGrid.ColumnDefinitions.Count));
        }

        private RowDefinitions CalculateAndBuildRowDefinitions(HexControlViewModel viewModel)
        {
            RowDefinitions rowDefinition = new RowDefinitions();

            // 1. the first row is the description row
            rowDefinition.Add(new RowDefinition(GridLength.Auto));
            // 2. the second row is a placeholder
            rowDefinition.Add(new RowDefinition(GRID_PLACEHOLDER_MARGIN, GridUnitType.Pixel));

            // 3. all the other rows are actually data-related rows
            for (int index = 0; index < (int)Math.Ceiling(viewModel.Data.Length / 16d); index++)
                rowDefinition.Add(new RowDefinition(GridLength.Auto));

            return rowDefinition;
        }

        private ColumnDefinitions CalculateBuildColumnDefinitions(HexControlViewModel viewModel)
        {
            ColumnDefinitions columnDefinitions = new ColumnDefinitions();

            // 1. the first column is the description column
            columnDefinitions.Add(new ColumnDefinition(GridLength.Auto));
            // 2. the second column is a placeholder
            columnDefinitions.Add(new ColumnDefinition(GRID_PLACEHOLDER_MARGIN, GridUnitType.Pixel));

            // 3. add 16 columns for the data
            for (int index = 0; index < Math.Min(16, viewModel.HexValueViewModels.Count); index++)
                columnDefinitions.Add(new ColumnDefinition(GridLength.Auto));
            
            return columnDefinitions;
        }

        private IEnumerable<TextBlock> BuildRowDescriptions(int rowCount)
        {
            for (int index = 2; index < rowCount; index++)
            {
                var description = new TextBlock();
                Grid.SetRow(description, index);
                Grid.SetColumn(description, 0);
                description.Text = $"{index - 2:X}0";
                yield return description;
            }
        }

        private IEnumerable<TextBlock> BuildColumnDescriptions(int columnCount)
        {
            for (int index = 2; index < columnCount; index++)
            {
                var description = new TextBlock();
                Grid.SetRow(description, 0);
                Grid.SetColumn(description, index);
                description.Text = $"0{index - 2:X}";
                yield return description;
            }
        }

        private IEnumerable<IControl> BuildHexValueEntries(HexControlViewModel viewModel, int rowCount, int columnCount)
        {
            columnCount -= 2;
            rowCount -= 2;

            for (int row = 0; row < rowCount; row++)
            {
                for(int column = 0; column < columnCount; column++)
                {
                    int idx = (row * columnCount) + column;

                    if (viewModel.HexValueViewModels.Count - 1 < idx) break;

                    var hexValueControl = new HexValueControl();
                    hexValueControl.DataContext = viewModel.HexValueViewModels[idx];
                    Grid.SetRow(hexValueControl, row + 2);
                    Grid.SetColumn(hexValueControl, column + 2);
                    yield return hexValueControl;
                }
            }
        }
    }
}