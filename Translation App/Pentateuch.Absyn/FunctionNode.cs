using System.Collections.Generic;

namespace Pentateuch.Absyn;

/// <summary>
/// A GENESYS function in the neutral form both programs share (the "Absyn" in the design document).
/// Kept to plain classes and lists so it works on both .NET Framework 4.8 and 10.
/// </summary>
public class FunctionNode
{
    public string Id { get; set; } = "";

    public string Name { get; set; } = "";

    /// <summary>The functions that this node calls in its behavior diagram.</summary>
    public List<FunctionNode> Calls { get; set; } = new List<FunctionNode>();
}
