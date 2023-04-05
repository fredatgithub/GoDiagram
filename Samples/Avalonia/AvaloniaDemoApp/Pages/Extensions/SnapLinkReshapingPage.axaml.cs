/* Copyright 1998-2023 by Northwoods Software Corporation. */

using Northwoods.Go.Tools.Extensions;

namespace Demo.Extensions.SnapLinkReshaping {
  public partial class SnapLinkReshaping : DemoControl {
    // This is a stub for the designer.
    // See the SharedSamples project for sample implementation.

    private void _InitCheckBoxes() {
      avoidsNodesCb.IsCheckedChanged += (e, obj) => (_Diagram.ToolManager.LinkReshapingTool as SnapLinkReshapingTool).AvoidsNodes = avoidsNodesCb.IsChecked ?? false;
    }
  }
}
