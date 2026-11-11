using System.Collections.Generic;

namespace Pentateuch.Absyn;
// Currently only a draft of the shared Absyn Model between GENESYS and Roslyn

/// <summary>Everything read in one run.</summary>
public class AbsynModel
{
    /// <summary>ID of the function the run started from.</summary>
    public string RootFunctionId { get; set; } = "";

    public List<Function> Functions { get; set; } = new();

    /// <summary>Things the reader couldn't represent, for the review step to show.</summary>
    public List<Note> Notes { get; set; } = new();
}

/// <summary>A link to a GENESYS entity. The name is cosmetic.</summary>
public class Ref
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
}

public class Function
{
    public string Id { get; set; } = "";             // GENESYS ID
    public string Name { get; set; } = "";            // GENESYS name
    public string? CodeName { get; set; }             // its name in the code, once there is code (for push/pull)
    public string? Number { get; set; }               // GENESYS number
    public string? Description { get; set; }          // GENESYS description
    public List<Ref> Inputs { get; set; } = new();    // items it takes in
    public List<Ref> Triggers { get; set; } = new();  // the inputs that start it, subset of Inputs
    public List<Ref> Outputs { get; set; } = new();
    public Branch? Body { get; set; }                 // its behavior diagram, null if it has none
}

/// <summary>A sequence of steps, in diagram order.</summary>
public class Branch
{
    public string? Annotation { get; set; }           // text from GENESYS, e.g. a branch's condition
    public List<Construct> Steps { get; set; } = new();
}

/// <summary>One step in a behavior diagram.</summary>
public abstract class Construct
{
    public string Id { get; set; } = "";              // GENESYS ID of the construct
}

/// <summary>Runs another function. If that function has several exits, each exit leads to its own path.</summary>
public class Call : Construct
{
    public Ref Function { get; set; } = new();
    public List<ExitPath> ExitPaths { get; set; } = new();
}

public class ExitPath
{
    public Ref? Exit { get; set; }
    public double? Probability { get; set; }
    public Branch Body { get; set; } = new();
}

/// <summary>Branches that run at the same time (GENESYS branch).</summary>
public class ParallelBranches : Construct
{
    public List<Branch> Branches { get; set; } = new();
}

/// <summary>Exactly one option runs (GENESYS Select). An option's condition is its branch annotation.</summary>
public class Choice : Construct
{
    public List<Option> Options { get; set; } = new();
}

public class Option
{
    public double? Probability { get; set; }
    public Branch Body { get; set; } = new();
}

/// <summary>Repeats its body until a LoopExit (GENESYS loop).</summary>
public class Loop : Construct
{
    public string? Condition { get; set; }            // the loop's annotation, e.g. "forever"
    public Branch Body { get; set; } = new();
}

/// <summary>Leaves the innermost loop (GENESYS Loop Exit).</summary>
public class LoopExit : Construct { }

/// <summary>Runs its body once per member of a set (GENESYS Iterate, or Replicate when InParallel).</summary>
public class ForEach : Construct
{
    public Ref? DomainSet { get; set; }
    public bool InParallel { get; set; }
    public Branch Body { get; set; } = new();
}

/// <summary>Ends the current function through one of its exits (GENESYS Exit).</summary>
public class Exit : Construct
{
    public Ref? ExitTaken { get; set; }
}

public class Note
{
    public string? Id { get; set; }
    public string Message { get; set; } = "";
}
