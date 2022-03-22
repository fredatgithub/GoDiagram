﻿using System.Collections.Generic;
using System.ComponentModel;
using Northwoods.Go;
using Northwoods.Go.Models;

namespace WinFormsSampleControls.Spreadsheet {
  [ToolboxItem(false)]
  public partial class SpreadsheetControl : System.Windows.Forms.UserControl {
    private Diagram myDiagram;

    public SpreadsheetControl() {
      InitializeComponent();

      Setup();

      goWebBrowser1.Html = @"
        <p>
      An example of a single node containing nested Auto Panels surrounding Table Panels whose <a>Panel.ItemList</a> is bound to a list of numbers.
        </p>
";

    }

    private void Setup() {

      myDiagram = diagramControl1.Diagram;

      myDiagram.NodeSelectionAdornmentTemplate = new Adornment(PanelLayoutAuto.Instance).Add(
        new Shape { Fill = null, Stroke = "orange", StrokeWidth = 4 },
        new Placeholder { Margin = 2 }
      );

      var aggregatedValue = new Panel(PanelLayoutAuto.Instance) {
        Margin = 5
      }.Add(
        new Shape {
          Fill = "cornflowerblue",
          Stroke = "blue",
          StrokeWidth = 2
        },
        new TextBlock {
          Stroke = "White",
          Font = new Font("Segoe UI", 10),
          Alignment = Spot.Left,
          Margin = 10,
          Width = 50
        }.Bind("Text", "", (d, _) => d.ToString())
      ); // assume just a number or a string

      var aggregatedListH = new Panel(PanelLayoutAuto.Instance) {
        Margin = 4,
        Stretch = Stretch.Fill
      }.Add(
        new Shape {
          Fill = "cornflowerblue",
          Stroke = "blue",
          StrokeWidth = 2
        },
        new Panel(PanelLayoutTable.Instance) {
          Padding = 5, Alignment = Spot.Left, Stretch = Stretch.Fill,
          DefaultAlignment = Spot.Left, DefaultStretch = Stretch.Horizontal
        }.Add(
          new TextBlock {
            Stroke = "white",
            Font = new Font("Segoe UI", 10),
            Alignment = Spot.Left,
            Row = 0
          }.Bind("Text", "Header"),
          new Panel(PanelLayoutHorizontal.Instance) {
            Row = 1,
            ItemTemplate = aggregatedValue
          }.Bind("ItemList", "Values")
        )
      );

      var aggregatedListV = new Panel(PanelLayoutAuto.Instance) {
        Margin = 4,
        Stretch = Stretch.Fill
      }.Add(
        new Shape {
          Fill = "cornflowerblue",
          Stroke = "blue",
          StrokeWidth = 2
        },
        new Panel(PanelLayoutVertical.Instance) {
          Padding = 5,
          Alignment = Spot.Top,
          DefaultAlignment = Spot.Left
        }.Add(
          new TextBlock {
            Stroke = "white",
            Font = new Font("Segoe UI", 10),
            Alignment = Spot.Left,
            MaxSize = new Size(70, double.NaN)
          }.Bind("Text", "Header"),
          new Panel(PanelLayoutVertical.Instance) {
            ItemTemplate = aggregatedValue
          }.Bind("ItemList", "Values")
        )
      );

      var checkBoxTemplate = Builder.Make<Panel>("CheckBox", "Checked").Add(
        new TextBlock {
          Stroke = "white",
          Font = new Font("Segoe UI", 10),
          Alignment = Spot.Left
        }.Bind("Text", "Label")
      );

      myDiagram.NodeTemplate = new Part(PanelLayoutTable.Instance).Add(
        // the title
        new Panel(PanelLayoutAuto.Instance) {
          Row = 0, Column = 0, ColumnSpan = 3, Stretch = Stretch.Fill
        }.Add(
          new Shape {
            Fill = "cornflowerblue",
            Stroke = "blue",
            StrokeWidth = 2
          },
          new TextBlock {
            Stroke = "white",
            Font = new Font("Segoe UI", 16),
            Alignment = Spot.Center,
            Margin = 10
          }.Bind("Text", "Title")
        ),
        // insert an empty row and an empty column
        new Panel { Row = 1, Height = 8 },
        new Panel { Column = 1, Width = 8 },

        // the aggregated values
        new Panel(PanelLayoutAuto.Instance) {
          Row = 2,
          Column = 0,
          Stretch = Stretch.Fill
        }.Add(
          new Shape {
            Fill = "cornflowerblue",
            Stroke = "blue",
            StrokeWidth = 2
          },
          new Panel(PanelLayoutTable.Instance) {
            Padding = 4
          }.Add(
            new TextBlock {
              Stroke = "white",
              Font = new Font("Segoe UI", 16),
              Alignment = Spot.Left
            }.Bind("Text", "AggTitle"),

            // The B aggregated Values
            new Panel(PanelLayoutAuto.Instance) {
              Column = 0,
              Row = 1,
              Stretch = Stretch.Fill,
              Margin = new Margin(10, 0)
            }.Add(
              new Shape {
                Fill = "cornflowerblue",
                Stroke = "blue",
                StrokeWidth = 2
              },
              new Panel(PanelLayoutTable.Instance) {
                Padding = 4,
                Stretch = Stretch.Fill,
                DefaultStretch = Stretch.Fill
              }.Add(
                new TextBlock {
                  Stroke = "white",
                  Font = new Font("Segoe UI", 16),
                  Alignment = Spot.Left,
                  Row = 0
                }.Bind("Text", "AggHeaderB"),
                new Panel(PanelLayoutHorizontal.Instance) {
                  Row = 1,
                  ItemTemplate = aggregatedValue
                }.Bind("ItemList", "AggValuesB")
              )
            ),

            // Now the C Aggregated Values
            new Panel(PanelLayoutAuto.Instance) {
              Column = 0,
              Row = 2,
              Alignment = Spot.TopLeft
            }.Add(
              new Shape {
                Fill = "cornflowerblue",
                Stroke = "blue",
                StrokeWidth = 2
              },
              new Panel(PanelLayoutTable.Instance) {
                Padding = 4,
                Stretch = Stretch.Fill,
                DefaultStretch = Stretch.Fill
              }.Add(
                new TextBlock {
                  Stroke = "white",
                  Font = new Font("Segoe UI", 16),
                  Alignment = Spot.Left,
                  Row = 0,
                  Column = 0,
                  ColumnSpan = 2
                }.Bind("Text", "AggSubtitle"),
                new Panel(PanelLayoutVertical.Instance) {
                  Row = 1,
                  Column = 0,
                  Alignment = Spot.Left,
                  ItemTemplate = aggregatedListH
                }.Bind("ItemList", "AggValuesH"),
                new Panel(PanelLayoutHorizontal.Instance) {
                  Row = 1, Column = 1,
                  Alignment = Spot.Left,
                  ItemTemplate = aggregatedListV
                }.Bind("ItemList", "AggValuesV")
              )
            ),

            // the checkboxes
            new Panel(PanelLayoutAuto.Instance) {
              Row = 2,
              Column = 2,
              Stretch = Stretch.Fill
            }.Add(
              new Shape {
                Fill = "cornflowerblue",
                Stroke = "blue",
                StrokeWidth = 2
              },
              new Panel(PanelLayoutVertical.Instance) {
                Padding = 4,
                Alignment = Spot.Top,
                Stretch = Stretch.Vertical,
                DefaultAlignment = Spot.Left
              }.Add(
                new TextBlock {
                  Stroke = "white",
                  Font = new Font("Segoe UI", 16),
                  Alignment = Spot.Left
                }.Bind("Text", "Choices"),
                new Panel(PanelLayoutVertical.Instance) {
                  ItemTemplate = checkBoxTemplate
                }.Bind("ItemList", "CheckBoxes")
              )
            )
          )
        )
      );

      myDiagram.Model = new Model();

      myDiagram.Model.NodeDataSource = new List<NodeData> {
        new NodeData {
          Title = "The Main Title",

          AggTitle = "A-Aggregated Values (from B and C)",

          AggHeaderB = "B-Aggregated Values (from B1-B2)",
          AggValuesB = new List<double> { 101.01, 102.02 },

          AggSubtitle = "C-Aggregated Values (from D, E, F, and G)",
          AggValuesH = new List<AggValueData> {
            new AggValueData {
              Header = "D-Aggregated Values (from D1..Dx)",
              Values = new List<double> { 1.01, 2.02, 3.03, 4.04 }
            },
            new AggValueData {
              Header = "E-Aggregated Values (from E1..Ex)",
              Values = new List<double> { 11.01, 12.02 }
            },
            new AggValueData {
              Header = "F-Aggregated Values (from F1..Fx)",
              Values = new List<double> { 21.01, 22.02, 23.03, 24.04, 25.05 }
            }
          },

          AggValuesV = new List<AggValueData> {
            new AggValueData {
              Header = "G-Aggregated Values (from G1..Gx)",
              Values = new List<double> { 31.01, 32.02, 33.03 }
            }
          },

          Choices = "Check Boxes",
          CheckBoxes = new List<CheckBoxData> {
            new CheckBoxData { Label = "Checkbox 1", Checked = true },
            new CheckBoxData { Label = "Checkbox 2", Checked = false },
            new CheckBoxData { Label = "Checkbox 3", Checked = true },
            new CheckBoxData { Label = "Checkbox 4", Checked = false }
          }
        }
      };

    }

  }

  public class Model : GraphLinksModel<NodeData, int, object, LinkData, int, string> { }

  public class NodeData : Model.NodeData {
    public string Title { get; set; }
    public string AggTitle { get; set; }
    public string AggHeaderA { get; set; }
    public string AggHeaderB { get; set; }
    public List<double> AggValuesB { get; set; } = new List<double>();
    public string AggSubtitle { get; set; }
    public string Choices { get; set; }

    public List<AggValueData> AggValuesH { get; set; }
    public List<AggValueData> AggValuesV { get; set; }
    public List<CheckBoxData> CheckBoxes { get; set; }
  }

  public class LinkData : Model.LinkData { }

  public class AggValueData {
    public string Header { get; set; }
    public List<double> Values { get; set; }
  }

  public class CheckBoxData {
    public string Label { get; set; }
    public bool Checked { get; set; }
  }

}
